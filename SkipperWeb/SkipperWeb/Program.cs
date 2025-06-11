using NetBlocks.Models.Environment;
using NetBlocks.Utilities.Environment;
using SkipperModels.Entities;
using SkipperWeb.ApiClients;
using SkipperWeb.Components;

namespace SkipperWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        LoadSkipperServices(builder.Services);
        
        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();
        app.UseStatusCodePagesWithRedirects("/404");

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.Run();
    }

    private static void LoadSkipperServices(IServiceCollection builderServices)
    {
        ApiEndpoint skipperEndpoint = EndpointsTools.LoadFromFile("environment/endpoints.json", "skipper-api");
        VesselClientConfig vesselClientConfig = new(skipperEndpoint.ApiUrl);
        
        builderServices.AddSingleton(vesselClientConfig);
        builderServices.AddScoped<VesselClient>();
    }
}