using SkipperData.Data;

namespace SkipperDataAgent;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Skipper Vessel Generator");
        Console.WriteLine("========================");
        
        Console.Write("Enter database connection string: ");
        var connectionString = Console.ReadLine();
        
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = "Host=localhost;Database=skipper;Username=postgres;Password=password";
            Console.WriteLine($"Using default: {connectionString}");
        }
        
        Console.Write("Number of vessels to generate (default 100): ");
        var input = Console.ReadLine();
        var count = int.TryParse(input, out var num) ? num : 100;
        
        Console.WriteLine($"Generating {count} vessels...");
        await DatabaseHelper.RunVesselGeneration(connectionString, count);
    }
}