using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using AuthBlocksWeb.ApiClients;
using AuthBlocksWeb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

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

        // Add custom JWT-based authentication services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<JwtAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());

        services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);
        
        // Add authorization for Blazor components with proper policies
        services.AddAuthorization();
    }
}