using SkipperData.Data;

namespace SkipperDataAgent;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Skipper Data Generator");
        Console.WriteLine("======================");
        
        Console.Write("Enter database connection string: ");
        var connectionString = Console.ReadLine();
        
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = "Host=localhost;Database=skipper;Username=postgres;Password=password";
            Console.WriteLine($"Using default: {connectionString}");
        }
        
        Console.WriteLine("\nChoose generation option:");
        Console.WriteLine("1. Generate vessels only");
        Console.WriteLine("2. Generate slip classifications only");
        Console.WriteLine("3. Generate slips only");
        Console.WriteLine("4. Generate rental agreements only");
        Console.WriteLine("5. Generate everything (classifications, slips, vessels)");
        Console.Write("Enter choice (1-5): ");
        
        var choice = Console.ReadLine();
        
        switch (choice)
        {
            case "1":
                await GenerateVesselsOnly(connectionString);
                break;
            case "2":
                await GenerateSlipClassificationsOnly(connectionString);
                break;
            case "3":
                await GenerateSlipsOnly(connectionString);
                break;
            case "4":
                await GenerateRentalAgreementsOnly(connectionString);
                break;
            case "5":
                await GenerateEverything(connectionString);
                break;
            default:
                Console.WriteLine("Invalid choice. Exiting.");
                break;
        }
    }
    
    static async Task GenerateVesselsOnly(string connectionString)
    {
        Console.Write("Number of vessels to generate (default 100): ");
        var input = Console.ReadLine();
        var count = int.TryParse(input, out var num) ? num : 100;
        
        Console.WriteLine($"Generating {count} vessels...");
        await DatabaseHelper.RunVesselGeneration(connectionString, count);
    }
    
    static async Task GenerateSlipClassificationsOnly(string connectionString)
    {
        Console.WriteLine("Generating slip classifications...");
        await DatabaseHelper.RunSlipClassificationGeneration(connectionString);
    }
    
    static async Task GenerateSlipsOnly(string connectionString)
    {
        Console.Write("Number of slips to generate (default 250): ");
        var input = Console.ReadLine();
        var count = int.TryParse(input, out var num) ? num : 250;
        
        Console.WriteLine($"Generating {count} slips...");
        await DatabaseHelper.RunSlipGeneration(connectionString, count);
    }
    
    static async Task GenerateRentalAgreementsOnly(string connectionString)
    {
        Console.Write("Number of rental agreements to generate (default 50000): ");
        var input = Console.ReadLine();
        var count = int.TryParse(input, out var num) ? num : 50000;
        
        Console.WriteLine($"Generating {count} rental agreements...");
        await DatabaseHelper.RunRentalAgreementGeneration(connectionString, count);
    }
    
    static async Task GenerateEverything(string connectionString)
    {
        Console.Write("Number of vessels to generate (default 100): ");
        var vesselInput = Console.ReadLine();
        var vesselCount = int.TryParse(vesselInput, out var vesselNum) ? vesselNum : 100;
        
        Console.Write("Number of slips to generate (default 250): ");
        var slipInput = Console.ReadLine();
        var slipCount = int.TryParse(slipInput, out var slipNum) ? slipNum : 250;
        
        Console.WriteLine($"Generating everything: {vesselCount} vessels, {slipCount} slips, and slip classifications...");
        await DatabaseHelper.RunFullGeneration(connectionString, vesselCount, slipCount);
    }
}