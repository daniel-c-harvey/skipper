using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using AuthBlocksWeb.ApiClients;
using AuthBlocksWeb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksWeb.HierarchicalAuthorize;

namespace AuthBlocksWeb;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, string apiBaseUrl)
    {
        // Add Blazor authentication state management
        services.AddCascadingAuthenticationState();
        
        // Configure HttpClient for API communication
        services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });
        
        services.AddHttpClient<IUsersApiClient, UsersApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        // Add custom JWT-based authentication services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<JwtAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
        
        // Register the hierarchical role service and authorization handlers
        services.AddScoped<IHierarchicalRoleService, HierarchicalRoleService>();
        services.AddScoped<IAuthorizationHandler, HierarchicalRolesAuthorizationHandler>();
        
        // Add authorization with hierarchical role support
        services.AddAuthentication().AddCookie(IdentityConstants.BearerScheme);
        services.AddAuthorization();
    }
}