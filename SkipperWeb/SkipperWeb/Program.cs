using MudBlazor.Services;
using NetBlocks.Models.Environment;
using NetBlocks.Utilities.Environment;
using SkipperModels.Entities;
using SkipperWeb.ApiClients;
using SkipperWeb.Components;
using SkipperWeb.Components.Pages.Entities;
using SkipperWeb.Components.Pages.Vessels;
using System.Text.Json;
using SkipperModels.Models;

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
            
        builder.Services
            .AddMudServices();

        LoadAuthBlocksServices(builder.Services);
        LoadSkipperServices(builder.Services);
        

        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services.AddLocalization();

        var app = builder.Build();

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
            .AddAdditionalAssemblies(typeof(Shared._Imports).Assembly)
            .AddAdditionalAssemblies(typeof(AuthBlocksWeb._Imports).Assembly)
            .AddAdditionalAssemblies(typeof(AuthBlocksWeb.Client._Imports).Assembly);

        app.Run();
    }

    private static void LoadAuthBlocksServices(IServiceCollection builderServices)
    {
        ApiEndpoint userEndpoint = EndpointsTools.LoadFromFile("environment/endpoints.json", "user-api");
        AuthBlocksWeb.Startup.ConfigureServices(builderServices, userEndpoint.ApiUrl);
    }

    private static void LoadSkipperServices(IServiceCollection builderServices)
    {
        ApiEndpoint skipperEndpoint = EndpointsTools.LoadFromFile("environment/endpoints.json", "skipper-api");
        
        builderServices.AddSingleton(new VesselClientConfig(skipperEndpoint.ApiUrl));
        builderServices.AddScoped<VesselClient>();
        builderServices.AddSingleton(new SlipClientConfig(skipperEndpoint.ApiUrl));
        builderServices.AddScoped<SlipClient>();
        builderServices.AddSingleton(new SlipClassificationClientConfig(skipperEndpoint.ApiUrl));
        builderServices.AddScoped<SlipClassificationClient>();
        builderServices.AddSingleton(new RentalAgreementClientConfig(skipperEndpoint.ApiUrl));
        builderServices.AddScoped<RentalAgreementClient>();

        
        // Add ViewModels as scoped services
        builderServices.AddScoped<ModelPageViewModel<VesselModel, VesselEntity, VesselClient, VesselClientConfig>>();
        builderServices.AddScoped<ModelPageViewModel<SlipModel, SlipEntity, SlipClient, SlipClientConfig>>();
        builderServices.AddScoped<ModelPageViewModel<SlipClassificationModel, SlipClassificationEntity, SlipClassificationClient, SlipClassificationClientConfig>>();
        builderServices.AddScoped<ModelPageViewModel<RentalAgreementModel, RentalAgreementEntity, RentalAgreementClient, RentalAgreementClientConfig>>();
    }
}