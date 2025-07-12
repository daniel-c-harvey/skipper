using System.Text;
using AuthBlocksAPI.Models;
using AuthBlocksAPI.Services;
using AuthBlocksAPI.HierarchicalAuthorize;
using AuthBlocksData.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace AuthBlocksAPI;

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
            builder.Services.AddOpenApi();

            ConfigureProxyServices(builder);

            // Configure JWT Settings - Load from file and register as singleton
            logger.LogInformation("Loading JWT configuration from file...");
            var jwtSettings = Startup.LoadJwtConfig();
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
            var connection = Startup.LoadConnection();
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

            // Add Hierarchical Role Authorization services
            builder.Services.AddScoped<IHierarchicalRoleService, HierarchicalRoleService>();
            builder.Services.AddScoped<IAuthorizationHandler, HierarchicalRolesAuthorizationHandler>();

            var app = builder.Build();

            ConfigureAppProxy(app);
            
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.MapOpenApi();
                app.MapScalarApiReference();
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
                await Startup.SeedSystemRolesAsync(scope.ServiceProvider);
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
    
    private static void ConfigureProxyServices(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            // Configure forwarded headers
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                // Specify which headers to process
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                                           ForwardedHeaders.XForwardedProto | 
                                           ForwardedHeaders.XForwardedHost;
        
                // For production behind nginx, clear known networks/proxies
                // ONLY if you trust your network infrastructure
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
        
                // Limit processing to prevent abuse
                options.ForwardLimit = 2;
        
                // For APIs: restrict allowed hosts for security
                options.AllowedHosts.Add("api.example.com");
                options.AllowedHosts.Add("*.example.com");
            });
        }
    }
    
    private static void ConfigureAppProxy(WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            // CRITICAL: Use forwarded headers BEFORE other middleware
            app.UseForwardedHeaders();
        }
    }
}