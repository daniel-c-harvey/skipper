using System.Globalization;
using AuthBlocksAPI.Common;
using AuthBlocksAPI.Models;
using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using AuthBlocksModels.SystemDefinitions;
using NetBlocks.Models.Environment;
using NetBlocks.Utilities.Environment;

namespace AuthBlocksAPI;

internal static class Startup
{
    public static Connection LoadConnection()
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
    
    public static JwtSettings LoadJwtConfig()
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
    
    private static AdminUserSettings LoadAdminConfig()
    {
        const string configPath = "environment/admin.json";
        
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Admin settings configuration file not found at: {configPath}");
        }

        try
        {
            var admin = JsonTools<AdminUserSettings>.LoadFromFile(configPath);
            
            // Validate required settings
            if (string.IsNullOrEmpty(admin.UserName))
                throw new InvalidOperationException("Admin UserName is required but not configured");
            
            if (string.IsNullOrEmpty(admin.Email))
                throw new InvalidOperationException("Admin Email is required but not configured");
            
            if (string.IsNullOrEmpty(admin.Password))
                throw new InvalidOperationException("Admin Password is required but not configured");
            
            return admin;
        }
        catch (Exception ex) when (!(ex is FileNotFoundException || ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Failed to load Admin settings configuration: {ex.Message}", ex);
        }
    }
    
    public static async Task SeedSystemRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        await SeedSystemRoles(scope);
        await SeedAdminUser(scope);
    }
    
    private static async Task SeedSystemRoles(IServiceScope scope)
    {
        var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();
        
        // Create roles in hierarchical order (parents first)
        foreach (var systemRole in SystemRole.GetAll().OrderBy(r => r.Id))
        {
            var existingRoleResult = await roleService.FindByNameAsync(systemRole.Name);
            if (existingRoleResult.Value == null)
            {
                var existingParentRole = (systemRole.ParentRole is not null) 
                                            ? await roleService.FindByNameAsync(systemRole.ParentRole.Name) 
                                            : null;
                var role = new RoleModel
                {
                    Name = systemRole.Name,
                    NormalizedName = systemRole.Name.ToUpperInvariant(),
                    // Only set the ID, not the full ParentRole object to avoid Entity Framework tracking conflicts
                    ParentRole = existingParentRole?.Value != null 
                        ? new RoleModel { Id = existingParentRole.Value.Id } 
                        : null,
                    ConcurrencyStamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await roleService.Add(role);
            }
        }
    }
    
    private static async Task SeedAdminUser(IServiceScope scope)
    {
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var userRoleService = scope.ServiceProvider.GetRequiredService<IUserRoleService>();
        var adminSettings = LoadAdminConfig();
        var existingUser = await userService.FindByNameAsync(adminSettings.UserName);
        if (existingUser is null)
        {
            var user = new UserModel
            {
                UserName = adminSettings.UserName,
                NormalizedUserName = adminSettings.UserName.ToUpperInvariant(),
                Email = adminSettings.Email,
                NormalizedEmail = adminSettings.Email.ToUpperInvariant(),
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var createResult = await userService.Add(user, adminSettings.Password);
            if (createResult.Success && createResult.Value != null)
            {
                await userRoleService.AddUserToRoleAsync(createResult.Value, SystemRoleConstants.Admin);
            }
        }
        else if (existingUser.Email != adminSettings.Email)
        {
            existingUser.Email = adminSettings.Email;
            await userService.Update(existingUser);
        }
        else if (!await userService.CheckPassword(existingUser, adminSettings.Password))
        {
            await userService.UpdatePassword(existingUser, adminSettings.Password);
        }
    }
}