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

    public static async Task RunSlipClassificationGeneration(string connectionString)
    {
        // Simple DbContext setup
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<SlipClassificationRepository>();
        
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            await GenerateSlipClassifications.GenerateAsync(dbContext, logger);
            Console.WriteLine("Successfully generated slip classifications!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating slip classifications: {ex.Message}");
        }
    }

    public static async Task RunSlipGeneration(string connectionString, int totalRecords = 250, int batchSize = 50)
    {
        // Simple DbContext setup
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<SlipRepository>();
        
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            await GenerateSlips.GenerateAsync(dbContext, logger, totalRecords, batchSize);
            Console.WriteLine($"Successfully generated {totalRecords} slips!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating slips: {ex.Message}");
        }
    }

    public static async Task RunRentalAgreementGeneration(string connectionString, int totalRecords = 50000, int batchSize = 1000)
    {
        // Simple DbContext setup
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<SlipReservationRepository>();
        
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            await GenerateSlipReservations.GenerateAsync(dbContext, logger, totalRecords, batchSize);
            Console.WriteLine($"Successfully generated {totalRecords} rental agreements!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating rental agreements: {ex.Message}");
        }
    }

    public static async Task RunFullGeneration(string connectionString, int vesselCount = 100, int slipCount = 250, int batchSize = 50)
    {
        // Simple DbContext setup
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            
            // Generate slip classifications first
            Console.WriteLine("Generating slip classifications...");
            var slipLogger = loggerFactory.CreateLogger<SlipClassificationRepository>();
            await GenerateSlipClassifications.GenerateAsync(dbContext, slipLogger);
            Console.WriteLine("Slip classifications generated successfully!");
            
            // Generate slips
            Console.WriteLine($"Generating {slipCount} slips...");
            var slipRepoLogger = loggerFactory.CreateLogger<SlipRepository>();
            await GenerateSlips.GenerateAsync(dbContext, slipRepoLogger, slipCount, batchSize);
            Console.WriteLine($"Successfully generated {slipCount} slips!");
            
            // Generate vessels
            Console.WriteLine($"Generating {vesselCount} vessels...");
            var vesselLogger = loggerFactory.CreateLogger<VesselRepository>();
            await GenerateVessels.GenerateAsync(dbContext, vesselLogger, vesselCount, batchSize);
            Console.WriteLine($"Successfully generated {vesselCount} vessels!");
            
            Console.WriteLine("Full generation completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during generation: {ex.Message}");
        }
    }
} 