using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperData.Data.Repositories;
using SkipperDataAgent;

Console.WriteLine("================================================================");
Console.WriteLine("              Skipper Marina Data Generator                   ");
Console.WriteLine("        Comprehensive Marina Management System Dataset        ");
Console.WriteLine("================================================================");
Console.WriteLine();

// Get database connection string
Console.Write("Enter database connection string (or press Enter for default): ");
var connectionString = Console.ReadLine();

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = "Host=localhost;Database=skipper;Username=postgres;Password=password";
    Console.WriteLine($"   Using default: {connectionString}");
}
Console.WriteLine();

// Main menu
while (true)
{
    Console.WriteLine("Choose generation option:");
    Console.WriteLine("   1. Full Marina Generation (recommended for complete dataset)");
    Console.WriteLine("   2. Customer & Reservations Only (requires existing infrastructure)");
    Console.WriteLine("   3. Individual Component Generation");
    Console.WriteLine("   4. Quick Demo Dataset");
    Console.WriteLine("   5. Exit");
    Console.WriteLine();
    Console.Write("Enter your choice (1-5): ");
    
    var choice = Console.ReadLine();
    Console.WriteLine();
    
    try
    {
        switch (choice)
        {
            case "1":
                await RunFullGeneration(connectionString);
                break;
            case "2":
                await RunCustomersAndReservations(connectionString);
                break;
            case "3":
                await RunIndividualComponents(connectionString);
                break;
            case "4":
                await RunQuickDemo(connectionString);
                break;
            case "5":
                Console.WriteLine("Goodbye!");
                return;
            default:
                Console.WriteLine("Invalid choice. Please select 1-5.");
                continue;
        }
        
        Console.WriteLine();
        Console.WriteLine("Generation completed! Press any key to return to menu...");
        Console.ReadKey();
        Console.Clear();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }
}

static async Task RunFullGeneration(string connectionString)
{
    Console.WriteLine("FULL MARINA GENERATION");
    Console.WriteLine("================================================================");
    Console.WriteLine("This will generate a complete marina management dataset:");
    Console.WriteLine("* Slip classifications and infrastructure");
    Console.WriteLine("* Vessels with realistic specifications");
    Console.WriteLine("* Customers with complete profiles and vessel ownership");
    Console.WriteLine("* Slip reservation orders with business logic");
    Console.WriteLine();

    var vesselCount = GetIntInput("Number of vessels", 100, 1, 10000);
    var slipCount = GetIntInput("Number of slips", 250, vesselCount, 50000);
    var customerCount = GetIntInput("Number of customers", Math.Min(vesselCount * 2, 2000), 1, vesselCount * 3);
    var reservationCount = GetIntInput("Number of slip reservations", customerCount * 5, 1, 1000000);
    var batchSize = GetIntInput("Batch size (for processing efficiency)", 50, 10, 1000);
    
    Console.WriteLine();
    Console.WriteLine("Generation Summary:");
    Console.WriteLine($"   * Vessels: {vesselCount:N0}");
    Console.WriteLine($"   * Slips: {slipCount:N0}");
    Console.WriteLine($"   * Customers: {customerCount:N0}");
    Console.WriteLine($"   * Reservations: {reservationCount:N0}");
    Console.WriteLine($"   * Batch Size: {batchSize:N0}");
    Console.WriteLine();
    
    if (ConfirmGeneration())
    {
        await DatabaseHelper.RunFullGeneration(
            connectionString, 
            vesselCount, 
            slipCount, 
            customerCount, 
            reservationCount, 
            batchSize);
    }
}

static async Task RunCustomersAndReservations(string connectionString)
{
    Console.WriteLine("CUSTOMERS & RESERVATIONS GENERATION");
    Console.WriteLine("================================================================");
    Console.WriteLine("Generates customers and reservations (requires existing slips and vessels)");
    Console.WriteLine();

    var customerCount = GetIntInput("Number of customers to generate", 2000, 1, 100000);
    var reservationCount = GetIntInput("Number of reservations to generate", customerCount * 5, 1, 1000000);
    var customerBatchSize = GetIntInput("Customer batch size", 500, 10, 1000);
    var reservationBatchSize = GetIntInput("Reservation batch size", 1000, 10, 2000);
    
    Console.WriteLine();
    if (ConfirmGeneration())
    {
        await DatabaseHelper.GenerateCustomersAndSlipReservationsAsync(
            connectionString,
            customerCount,
            reservationCount,
            customerBatchSize,
            reservationBatchSize);
    }
}

static async Task RunIndividualComponents(string connectionString)
{
    Console.WriteLine("INDIVIDUAL COMPONENT GENERATION");
    Console.WriteLine("================================================================");
    Console.WriteLine("Generate individual components separately:");
    Console.WriteLine("   1. Vessels only");
    Console.WriteLine("   2. Slip classifications only");
    Console.WriteLine("   3. Slips only");
    Console.WriteLine("   4. Customers only");
    Console.WriteLine("   5. Slip reservations only");
    Console.WriteLine();
    
    Console.Write("Choose component (1-5): ");
    var componentChoice = Console.ReadLine();
    Console.WriteLine();
    
    switch (componentChoice)
    {
        case "1":
            var vesselCount = GetIntInput("Number of vessels", 100, 1, 10000);
            var vesselBatchSize = GetIntInput("Batch size", 50, 10, 500);
            if (ConfirmGeneration())
                await DatabaseHelper.RunVesselGeneration(connectionString, vesselCount, vesselBatchSize);
            break;
            
        case "2":
            Console.WriteLine("Generating standard slip classifications...");
            if (ConfirmGeneration())
                await DatabaseHelper.RunSlipClassificationGeneration(connectionString);
            break;
            
        case "3":
            var slipCount = GetIntInput("Number of slips", 250, 1, 50000);
            var slipBatchSize = GetIntInput("Batch size", 50, 10, 500);
            if (ConfirmGeneration())
                await DatabaseHelper.RunSlipGeneration(connectionString, slipCount, slipBatchSize);
            break;
            
        case "4":
            var customerCount = GetIntInput("Number of customers", 1000, 1, 100000);
            var customerBatchSize = GetIntInput("Batch size", 250, 10, 1000);
            Console.WriteLine("Note: This requires existing vessels in the database.");
            if (ConfirmGeneration())
            {
                // Direct customer generation
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                var logger = loggerFactory.CreateLogger<VesselOwnerCustomerRepository>();
                
                var options = new DbContextOptionsBuilder<SkipperData.Data.SkipperContext>()
                    .UseNpgsql(connectionString)
                    .Options;
                    
                using var dbContext = new SkipperData.Data.SkipperContext(options);
                using var transaction = await dbContext.Database.BeginTransactionAsync();
                try
                {
                    await GenerateCustomers.GenerateAsync(dbContext, logger, customerCount, customerBatchSize);
                    await transaction.CommitAsync();
                    Console.WriteLine("Customer generation completed successfully!");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Customer generation failed: {ex.Message}");
                    Console.WriteLine("All changes have been rolled back.");
                    throw;
                }
            }
            break;
            
        case "5":
            var reservationCount = GetIntInput("Number of reservations", 5000, 1, 1000000);
            var reservationBatchSize = GetIntInput("Batch size", 1000, 10, 2000);
            Console.WriteLine("Note: This requires existing customers, vessels, and slips in the database.");
            if (ConfirmGeneration())
            {
                // Direct reservation generation  
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                var logger = loggerFactory.CreateLogger<SlipReservationOrderRepository>();
                
                var options = new DbContextOptionsBuilder<SkipperData.Data.SkipperContext>()
                    .UseNpgsql(connectionString)
                    .Options;
                    
                using var dbContext = new SkipperData.Data.SkipperContext(options);
                using var transaction = await dbContext.Database.BeginTransactionAsync();
                try
                {
                    await GenerateSlipReservations.GenerateAsync(dbContext, logger, reservationCount, reservationBatchSize);
                    await transaction.CommitAsync();
                    Console.WriteLine("Reservation generation completed successfully!");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Reservation generation failed: {ex.Message}");
                    Console.WriteLine("All changes have been rolled back.");
                    throw;
                }
            }
            break;
            
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }
}

static async Task RunQuickDemo(string connectionString)
{
    Console.WriteLine("QUICK DEMO DATASET");
    Console.WriteLine("================================================================");
    Console.WriteLine("Generates a small, complete dataset perfect for demos and testing:");
    Console.WriteLine("* 25 vessels * 50 slips * 100 customers * 500 reservations");
    Console.WriteLine();
    
    if (ConfirmGeneration())
    {
        await DatabaseHelper.RunFullGeneration(
            connectionString,
            vesselCount: 25,
            slipCount: 50, 
            customerCount: 100,
            slipReservationCount: 500,
            batchSize: 25);
    }
}

static int GetIntInput(string prompt, int defaultValue, int min = 1, int max = int.MaxValue)
{
    while (true)
    {
        Console.Write($"   {prompt} (default {defaultValue:N0}): ");
        var input = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;
            
        if (int.TryParse(input.Replace(",", ""), out var value))
        {
            if (value >= min && value <= max)
                return value;
            else
                Console.WriteLine($"   Please enter a value between {min:N0} and {max:N0}.");
        }
        else
        {
            Console.WriteLine("   Please enter a valid number.");
        }
    }
}

static bool ConfirmGeneration()
{
    Console.Write("Proceed with generation? (y/N): ");
    var confirm = Console.ReadLine()?.ToLower();
    return confirm == "y" || confirm == "yes";
}