using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperData.Data.Repositories;

namespace SkipperDataAgent;

public static class DatabaseHelper
{
    public static async Task GenerateCustomersAndSlipReservationsAsync(
        string connectionString, 
        int totalCustomers = 10000, 
        int totalSlipReservations = 50000, 
        int customerBatchSize = 500, 
        int reservationBatchSize = 1000)
    {
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        // Create loggers for each operation
        var customerLogger = loggerFactory.CreateLogger<VesselOwnerCustomerRepository>();
        var reservationLogger = loggerFactory.CreateLogger<SlipReservationOrderRepository>();
        
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            
            Console.WriteLine("=== Customer and Slip Reservation Generation ===");
            
            // Step 1: Generate customers with vessel ownership
            Console.WriteLine("Step 1: Generating customers with vessel ownership...");
            await GenerateCustomers.GenerateAsync(dbContext, customerLogger, totalCustomers, customerBatchSize);
            Console.WriteLine($"Successfully generated {totalCustomers} vessel owner customers!");
            
            // Step 2: Generate slip reservations using the customers
            Console.WriteLine("Step 2: Generating slip reservation orders...");
            await GenerateSlipReservations.GenerateAsync(dbContext, reservationLogger, totalSlipReservations, reservationBatchSize);
            Console.WriteLine($"Successfully generated {totalSlipReservations} slip reservation orders!");
            
            await transaction.CommitAsync();
            Console.WriteLine("=== Generation Complete ===");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error during data generation: {ex.Message}");
            Console.WriteLine("All changes have been rolled back.");
            throw;
        }
    }

    public static async Task RunFullGeneration(
        string connectionString, 
        int vesselCount = 100, 
        int slipCount = 250, 
        int customerCount = 2000,
        int slipReservationCount = 10000,
        int batchSize = 50)
    {
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            
            Console.WriteLine("=== Full Skipper Marina Data Generation ===");
            Console.WriteLine($"Generation Plan:");
            Console.WriteLine($"   * Slip Classifications: Auto-generated standard types");
            Console.WriteLine($"   * Slips: {slipCount:N0}");
            Console.WriteLine($"   * Vessels: {vesselCount:N0}");
            Console.WriteLine($"   * Customers: {customerCount:N0} (with vessel ownership)");
            Console.WriteLine($"   * Slip Reservations: {slipReservationCount:N0}");
            Console.WriteLine();
            
            // Step 1: Generate slip classifications first (foundation)
            Console.WriteLine("Step 1/5: Generating slip classifications...");
            var slipClassLogger = loggerFactory.CreateLogger<SlipClassificationRepository>();
            await GenerateSlipClassifications.GenerateAsync(dbContext, slipClassLogger);
            Console.WriteLine("Slip classifications generated successfully!");
            Console.WriteLine();
            
            // Step 2: Generate slips (depends on classifications)
            Console.WriteLine($"Step 2/5: Generating {slipCount:N0} slips...");
            var slipLogger = loggerFactory.CreateLogger<SlipRepository>();
            await GenerateSlips.GenerateAsync(dbContext, slipLogger, slipCount, batchSize);
            Console.WriteLine($"Successfully generated {slipCount:N0} slips!");
            Console.WriteLine();
            
            // Step 3: Generate vessels (independent)
            Console.WriteLine($"Step 3/5: Generating {vesselCount:N0} vessels...");
            var vesselLogger = loggerFactory.CreateLogger<VesselRepository>();
            await GenerateVessels.GenerateAsync(dbContext, vesselLogger, vesselCount, batchSize);
            Console.WriteLine($"Successfully generated {vesselCount:N0} vessels!");
            Console.WriteLine();
            
            // Step 4: Generate customers with vessel ownership (depends on vessels)
            Console.WriteLine($"Step 4/5: Generating {customerCount:N0} vessel owner customers...");
            var customerLogger = loggerFactory.CreateLogger<VesselOwnerCustomerRepository>(); 
            var customerBatchSize = Math.Min(batchSize * 5, 500); // Larger batches for customers
            await GenerateCustomers.GenerateAsync(dbContext, customerLogger, customerCount, customerBatchSize);
            Console.WriteLine($"Successfully generated {customerCount:N0} vessel owner customers!");
            Console.WriteLine();
            
            // Step 5: Generate slip reservations (depends on customers, vessels, slips)
            Console.WriteLine($"Step 5/5: Generating {slipReservationCount:N0} slip reservation orders...");
            var reservationLogger = loggerFactory.CreateLogger<SlipReservationOrderRepository>();
            var reservationBatchSize = Math.Min(batchSize * 10, 1000); // Larger batches for reservations
            await GenerateSlipReservations.GenerateAsync(dbContext, reservationLogger, slipReservationCount, reservationBatchSize);
            Console.WriteLine($"Successfully generated {slipReservationCount:N0} slip reservation orders!");
            Console.WriteLine();
            
            await transaction.CommitAsync();
            Console.WriteLine("Full generation completed successfully!");
            Console.WriteLine("Generation Summary:");
            Console.WriteLine("   * Complete marina infrastructure (classifications, slips)");
            Console.WriteLine($"   * {vesselCount:N0} vessels with realistic specifications");
            Console.WriteLine($"   * {customerCount:N0} customers with complete profiles and vessel ownership");
            Console.WriteLine($"   * {slipReservationCount:N0} reservation orders with proper business logic");
            Console.WriteLine("   * All entities properly linked with referential integrity");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error during generation: {ex.Message}");
            Console.WriteLine("All changes have been rolled back.");
            throw;
        }
    }

    public static async Task RunVesselGeneration(string connectionString, int totalRecords = 100, int batchSize = 50)
    {
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<VesselRepository>();
        
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            await GenerateVessels.GenerateAsync(dbContext, logger, totalRecords, batchSize);
            await transaction.CommitAsync();
            Console.WriteLine($"Successfully generated {totalRecords} vessels!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error generating vessels: {ex.Message}");
            Console.WriteLine("All changes have been rolled back.");
            throw;
        }
    }

    public static async Task RunSlipClassificationGeneration(string connectionString)
    {
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<SlipClassificationRepository>();
        
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            await GenerateSlipClassifications.GenerateAsync(dbContext, logger);
            await transaction.CommitAsync();
            Console.WriteLine("Successfully generated slip classifications!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error generating slip classifications: {ex.Message}");
            Console.WriteLine("All changes have been rolled back.");
            throw;
        }
    }

    public static async Task RunSlipGeneration(string connectionString, int totalRecords = 250, int batchSize = 50)
    {
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<SlipRepository>();
        
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            await GenerateSlips.GenerateAsync(dbContext, logger, totalRecords, batchSize);
            await transaction.CommitAsync();
            Console.WriteLine($"Successfully generated {totalRecords} slips!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error generating slips: {ex.Message}");
            Console.WriteLine("All changes have been rolled back.");
            throw;
        }
    }

    public static async Task RunRentalAgreementGeneration(string connectionString, int totalRecords = 50000, int batchSize = 1000)
    {
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        using var dbContext = new SkipperContext(options);
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<SlipReservationOrderRepository>();
        
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            await GenerateSlipReservations.GenerateAsync(dbContext, logger, totalRecords, batchSize);
            await transaction.CommitAsync();
            Console.WriteLine($"Successfully generated {totalRecords} rental agreements!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error generating rental agreements: {ex.Message}");
            Console.WriteLine("All changes have been rolled back.");
            throw;
        }
    }
} 