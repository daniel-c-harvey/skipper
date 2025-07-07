using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using AuthBlocksWeb.ApiClients;
using AuthBlocksWeb.Services;

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
        services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
        services.AddScoped<JwtAuthenticationStateProvider, JwtAuthenticationStateProvider>();
        
        // Add authorization
        services.AddAuthorizationCore();
    }
}

// Example usage in Program.cs:
/*
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
NewStartupExample.ConfigureServices(
    builder.Services, 
    builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

// Configure the HTTP request pipeline
await NewStartupExample.ConfigureAppAsync(app);

// Configure the app...
app.Run();
*/ 