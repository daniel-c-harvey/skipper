using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperData.Data.Repositories;

namespace SkipperDataAgent;

public static class DatabaseHelper
{
    public static async Task RunVesselGeneration(string connectionString, int totalRecords = 100, int batchSize = 50)
    {
        // Simple DbContext setup
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<VesselRepository>();
        
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            await GenerateVessels.GenerateAsync(dbContext, logger, totalRecords, batchSize);
            Console.WriteLine($"Successfully generated {totalRecords} vessels!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating vessels: {ex.Message}");
        }
    }
} 