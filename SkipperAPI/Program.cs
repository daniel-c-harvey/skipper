using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
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

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();
            app.UseHttpLogging();
            // Add this to see model binding errors
            app.UseExceptionHandler("/Error");
        }

        // // Custom middleware for request debugging
        // app.Use(async (context, next) =>
        // {
        //     var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        //
        //     logger.LogInformation("Incoming request: {Method} {Path} {QueryString}", 
        //         context.Request.Method, 
        //         context.Request.Path, 
        //         context.Request.QueryString);
        //
        //     // Log request headers in debug mode
        //     foreach (var header in context.Request.Headers)
        //     {
        //         logger.LogDebug("Request Header: {Key} = {Value}", header.Key, header.Value);
        //     }
        //
        //     await next();
        //
        //     logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
        // });
        
        // For now the API is only running locally behind firewall, keep things simple for now
        // app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
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