using AuthBlocksAPI.Models;
using AuthBlocksAPI.Services;
using AuthBlocksData.Data;
using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthBlocksAPI.Common;
using NetBlocks.Models.Environment;
using NetBlocks.Utilities.Environment;

internal class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            var logger = new Logger<Program>(LoggerFactory.Create(options =>
            {
                options.AddConsole();
            }));

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Configure JWT Settings - Load from file and register as singleton
            logger.LogInformation("Loading JWT configuration from file...");
            var jwtSettings = LoadJwtConfig();
            logger.LogInformation("JWT configuration loaded successfully. Issuer: {Issuer}, Audience: {Audience}", 
                jwtSettings.Issuer, jwtSettings.Audience);
            
            builder.Services.AddSingleton(jwtSettings);
            builder.Services.Configure<JwtSettings>(options =>
            {
                options.Secret = jwtSettings.Secret;
                options.Issuer = jwtSettings.Issuer;
                options.Audience = jwtSettings.Audience;
                options.ExpiryMinutes = jwtSettings.ExpiryMinutes;
                options.RefreshTokenExpiryDays = jwtSettings.RefreshTokenExpiryDays;
            });

        // Add CORS
        var corsOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorApp", policy =>
            {
                policy.WithOrigins(corsOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Add AuthBlocks data layer
        logger.LogInformation("Loading database connection configuration...");
        var connection = LoadConnection();
        logger.LogInformation("Database connection loaded successfully. Connection ID: {ConnectionId}", connection.ID);
        builder.Services.AddAuthBlocksDataForWebApi(connection.ConnectionString);

        // Add JWT Service
        builder.Services.AddScoped<IJwtService, JwtService>();

        // Add JWT Authentication
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowBlazorApp");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Initialize database and system data
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            await context.Database.EnsureCreatedAsync();
    
            // Add system roles
            await SeedSystemRolesAsync(scope.ServiceProvider);
        }

        app.Run();
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Configuration Error: {ex.Message}");
            Console.WriteLine("Please ensure the required configuration files exist:");
            Console.WriteLine("- environment/jwt_settings.json");
            Console.WriteLine("- environment/connections.json");
            Console.WriteLine("See the .example.json files for the expected format.");
            Environment.Exit(1);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Configuration Error: {ex.Message}");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Startup Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
    private static Connection LoadConnection()
    {
        const string configPath = "environment/connections.json";
        
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Database connection configuration file not found at: {configPath}");
        }

        try
        {
            Connections? connections = ConnectionStringTools.LoadFromFile(configPath);
            
            if (connections == null) 
                throw new InvalidOperationException("No connections configuration found in file");

            Connection? connection = connections.ConnectionStrings
                .FirstOrDefault(c => c.ID == connections.ActiveConnectionID);
            
            if (connection == null) 
                throw new InvalidOperationException($"Active connection with ID '{connections.ActiveConnectionID}' not found in connections file");
            
            return connection;
        }
        catch (Exception ex) when (!(ex is FileNotFoundException || ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Failed to load database connection configuration: {ex.Message}", ex);
        }
    }
    
    private static JwtSettings LoadJwtConfig()
    {
        const string configPath = "environment/jwt_settings.json";
        
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"JWT settings configuration file not found at: {configPath}");
        }

        try
        {
            var jwtSettings = JwtSettingsTools.LoadFromFile(configPath);
            
            // Validate required settings
            if (string.IsNullOrEmpty(jwtSettings.Secret))
                throw new InvalidOperationException("JWT Secret is required but not configured");
            
            if (jwtSettings.Secret.Length < 32)
                throw new InvalidOperationException("JWT Secret must be at least 32 characters long");
            
            if (string.IsNullOrEmpty(jwtSettings.Issuer))
                throw new InvalidOperationException("JWT Issuer is required but not configured");
            
            if (string.IsNullOrEmpty(jwtSettings.Audience))
                throw new InvalidOperationException("JWT Audience is required but not configured");
            
            return jwtSettings;
        }
        catch (Exception ex) when (!(ex is FileNotFoundException || ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Failed to load JWT settings configuration: {ex.Message}", ex);
        }
    }

    static async Task SeedSystemRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();

        var adminRole = new ApplicationRole
        {
            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = DateTime.UtcNow.ToString(),
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow
        };

        var existingRole = await roleService.FindByNameAsync("Admin");
        if (existingRole == null)
        {
            await roleService.CreateRoleAsync(adminRole);
        }
    
    }
}
