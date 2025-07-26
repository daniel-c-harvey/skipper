using MudBlazor.Services;
using NetBlocks.Models.Environment;
using NetBlocks.Utilities.Environment;
using SkipperModels.Entities;
using SkipperWeb.ApiClients;
using SkipperWeb.Components;
using System.Text.Json;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using AuthBlocksWeb.ApiClients;
using AuthBlocksWeb.Components.Pages.UserAdmin;
using Microsoft.AspNetCore.HttpOverrides;
using SkipperModels.Models;
using SkipperWeb.Components.Pages.Maintenance.SlipReservations;
using SkipperWeb.Components.Pages.Maintenance.SlipClassifications;
using SkipperWeb.Components.Pages.Maintenance.Slips;
using SkipperWeb.Components.Pages.Maintenance.Vessels;
using Web.Shared.Maintenance.Entities;

namespace SkipperWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();
            
        ConfigureProxyServices(builder);
        
        builder.Services
            .AddMudServices();

        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        
        // Application Services
        LoadAuthBlocksServices(builder.Services);
        LoadSkipperServices(builder.Services);
        

        builder.Services.AddLocalization();

        var app = builder.Build();

        ConfigureAppProxy(app);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine("Development Mode");
            app.UseWebAssemblyDebugging();
        }
        else
        {
            Console.WriteLine("Production Mode");
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseAntiforgery();
        app.UseStatusCodePagesWithRedirects("/404");
        app.MapStaticAssets();
        app.UseRequestLocalization("en-US");

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly)
            .AddAdditionalAssemblies(typeof(Web.Shared._Imports).Assembly)
            .AddAdditionalAssemblies(typeof(AuthBlocksWeb._Imports).Assembly)
            .AddAdditionalAssemblies(typeof(AuthBlocksWeb.Client._Imports).Assembly);
        
        app.Run();
    }

    private static void ConfigureAppProxy(WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            // MUST be first in pipeline for Blazor
            app.UseForwardedHeaders();
        }
    }

    private static void ConfigureProxyServices(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                                           ForwardedHeaders.XForwardedProto | 
                                           ForwardedHeaders.XForwardedHost;
        
                // For Blazor with authentication, this is critical
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
        
                // Allow your domain for SignalR hub connections
                // options.AllowedHosts.Add("blazorapp.example.com");
        
                // For SignalR, you may need to allow multiple forwards
                // options.ForwardLimit = 2;
            });
        }
    }

    private static void LoadAuthBlocksServices(IServiceCollection builderServices)
    {
        ApiEndpoint userEndpoint = EndpointsTools.LoadFromFile("environment/endpoints.json", "user-api");
        
        // Critical Authentication Services for the Web App
        AuthBlocksWeb.Startup.ConfigureAuthServices(builderServices, userEndpoint.ApiUrl);
    }

    private static void LoadSkipperServices(IServiceCollection builderServices)
    {
        ApiEndpoint skipperEndpoint = EndpointsTools.LoadFromFile("environment/endpoints.json", "skipper-api");
        
        // Vessel Client
        builderServices.AddSingleton(new VesselClientConfig(skipperEndpoint.ApiUrl));
        builderServices.AddScoped<VesselClient>();
        builderServices.AddScoped<VesselsViewModel>();
        
        // Slip Client
        builderServices.AddSingleton(new SlipClientConfig(skipperEndpoint.ApiUrl));
        builderServices.AddScoped<SlipClient>();
        builderServices.AddScoped<SlipsViewModel>();
        
        // Slip Classification Client
        builderServices.AddSingleton(new SlipClassificationClientConfig(skipperEndpoint.ApiUrl));
        builderServices.AddScoped<SlipClassificationClient>();
        builderServices.AddScoped<SlipClassificationsViewModel>();

        // Slip Reservation Client
        builderServices.AddSingleton(new SlipReservationClientConfig(skipperEndpoint.ApiUrl));
builderServices.AddScoped<SlipReservationClient>();
builderServices.AddScoped<SlipReservationsViewModel>();
    }
}