using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetBlocks.Models.Environment;
using NetBlocks.Utilities.Environment;
using SkipperData.Data;
using SkipperData.Managers;

namespace SkipperAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureProxyServices(builder);
        
        // Add services to the container.
        LoadSkipperServices(builder.Services);

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        
        // Enable HTTP request/response logging (built-in .NET feature)
        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
            options.RequestBodyLogLimit = 4096;
            options.ResponseBodyLogLimit = 4096;
            options.CombineLogs = true;
        });

        
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        ConfigureAppProxy(app);
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();
            app.UseHttpLogging();
            // Add this to see model binding errors
            app.UseExceptionHandler("/Error");
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
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

    private static void LoadSkipperServices(IServiceCollection builderServices)
    {
        Connection connection = LoadConnection();
        builderServices.AddScoped(_ => SkipperContextBuilder.CreateContext(connection.ConnectionString));
        builderServices.AddRepositories();
        builderServices.AddManagers();
    }

    private static Connection LoadConnection()
    {
        Connections? connections = ConnectionStringTools.LoadFromFile("environment/connections.json");
        
        if (connections == null) throw new Exception("No connections configuration found");

        Connection? connection = connections.ConnectionStrings
            .FirstOrDefault(c => c.ID == connections.ActiveConnectionID);
        
        if (connection == null) throw new Exception("Active connection not found");
        
        return connection;
    }
}