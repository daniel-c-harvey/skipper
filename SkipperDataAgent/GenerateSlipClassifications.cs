using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperDataAgent;

public static class GenerateSlipClassifications
{
    // Define slip classification categories with unique dimensions
    private static readonly SlipClassificationData[] SlipCategories =
    [
        // Small Vessels
        new("Compact Runabout", 18.0m, 7.0m, 2500),
        new("Small Bowrider", 20.0m, 7.5m, 2800),
        new("Small Cuddy Cabin", 22.0m, 8.0m, 3200),
        new("Small Cruiser", 24.0m, 8.5m, 3600),
        new("Small Sport Boat", 25.0m, 8.5m, 3800),
        
        // Medium Vessels
        new("Medium Bowrider", 26.0m, 9.0m, 4200),
        new("Medium Express", 28.0m, 9.5m, 4800),
        new("Medium Sport Yacht", 30.0m, 10.0m, 5600),
        
        // Large Vessels
        new("Large Express Cruiser", 33.0m, 10.5m, 6800),
        new("Large Sport Yacht", 34.0m, 11.0m, 7200),
        new("Large Cabin Cruiser", 35.0m, 11.0m, 7600),
        
        // Extra Large Vessels
        new("Extra Large Express", 36.0m, 11.5m, 8200),
        new("Extra Large Sport Yacht", 38.0m, 12.0m, 9200),
        new("Extra Large Cabin Yacht", 40.0m, 12.5m, 10200),
        
        // Premium Vessels
        new("Premium Motor Yacht", 42.0m, 13.0m, 11400),
        new("Premium Sport Yacht", 44.0m, 13.5m, 12600),
        
        // Luxury Vessels
        new("Luxury Express Yacht", 46.0m, 14.0m, 14000),
        new("Luxury Cabin Yacht", 48.0m, 14.5m, 15400),
        new("Luxury Mega Yacht", 50.0m, 15.0m, 16800),
        
        // Super Luxury Vessels
        new("Super Luxury Express", 52.0m, 15.0m, 18200),
        new("Super Luxury Motor Yacht", 54.0m, 15.5m, 19600),
        new("Super Luxury Cabin Yacht", 56.0m, 16.0m, 21200),
        new("Super Luxury Mega Yacht", 60.0m, 16.5m, 24200),
        
        // Mega Yacht Category
        new("Mega Yacht Standard", 65.0m, 17.0m, 27000),
        new("Mega Yacht Deluxe", 70.0m, 17.5m, 30200),
        new("Mega Yacht Premium", 75.0m, 18.0m, 33600),
        
        // Super Yacht Category
        new("Super Yacht Standard", 80.0m, 18.5m, 37500),
        new("Super Yacht Deluxe", 85.0m, 19.0m, 41800),
        new("Super Yacht Premium", 90.0m, 19.5m, 46500),
        new("Super Yacht Elite", 95.0m, 20.0m, 51600),
        new("Super Yacht Ultimate", 100.0m, 20.5m, 57200),
        
        // Ultra Luxury Category
        new("Ultra Luxury Standard", 110.0m, 21.0m, 64500),
        new("Ultra Luxury Deluxe", 120.0m, 22.0m, 72800),
        new("Ultra Luxury Premium", 130.0m, 23.0m, 82200),
        new("Ultra Luxury Elite", 140.0m, 24.0m, 92800),
        new("Ultra Luxury Ultimate", 150.0m, 25.0m, 104500),
        
        // Commercial/Charter Category
        new("Charter Vessel Standard", 80.0m, 18.0m, 35000),
        new("Charter Vessel Large", 100.0m, 20.0m, 55000)
    ];

    public static async Task GenerateAsync(SkipperContext dbContext, ILogger<SlipClassificationRepository> logger)
    {
        logger.LogInformation("Starting slip classification generation");
        
        var repository = new SlipClassificationRepository(dbContext, logger);
        var random = new Random(42); // Fixed seed for reproducible results
        
        // Remove duplicates based on unique max_length/max_beam combinations
        var uniqueCategories = RemoveDuplicates(SlipCategories);
        var totalRecords = uniqueCategories.Length;
        
        logger.LogInformation("Generating {TotalRecords} unique slip classifications", totalRecords);
        
        var slipClassifications = new List<SlipClassificationEntity>();
        
        foreach (var category in uniqueCategories)
        {
            var slipClassification = GenerateSlipClassification(category, random);
            slipClassifications.Add(slipClassification);
        }
        
        // Add all slip classifications in parallel
        Task[] classificationTasks = slipClassifications.Select(Task (slipClassification) => repository.AddAsync(slipClassification)).ToArray();
        Task.WaitAll(classificationTasks);
        await repository.SaveChangesAsync();
        
        logger.LogInformation("Slip classification generation completed: {TotalRecords} classifications inserted", totalRecords);
    }

    private static SlipClassificationData[] RemoveDuplicates(SlipClassificationData[] categories)
    {
        var seen = new HashSet<(decimal maxLength, decimal maxBeam)>();
        var uniqueCategories = new List<SlipClassificationData>();
        
        foreach (var category in categories)
        {
            var key = (category.MaxLength, category.MaxBeam);
            if (seen.Add(key))
            {
                uniqueCategories.Add(category);
            }
        }
        
        return uniqueCategories.ToArray();
    }

    private static SlipClassificationEntity GenerateSlipClassification(SlipClassificationData category, Random random)
    {
        var description = GenerateDescription(category);
        var timestamp = GenerateTimestamp(random);

        return new SlipClassificationEntity
        {
            Name = category.Name,
            MaxLength = category.MaxLength,
            MaxBeam = category.MaxBeam,
            BasePrice = category.BasePrice,
            Description = description,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
            IsDeleted = false
        };
    }

    private static string GenerateDescription(SlipClassificationData category)
    {
        var maxLength = category.MaxLength;
        var maxBeam = category.MaxBeam;
        var pricePerDay = category.BasePrice / 100; // Convert cents to dollars
        
        // Create concise description based on vessel size
        if (maxLength <= 25)
            return $"Compact slip for vessels up to {maxLength}'L x {maxBeam}'B - ${pricePerDay}/day";
        else if (maxLength <= 35)
            return $"Medium slip for vessels up to {maxLength}'L x {maxBeam}'B - ${pricePerDay}/day";
        else if (maxLength <= 50)
            return $"Large slip for vessels up to {maxLength}'L x {maxBeam}'B - ${pricePerDay}/day";
        else if (maxLength <= 75)
            return $"Luxury slip for yachts up to {maxLength}'L x {maxBeam}'B - ${pricePerDay}/day";
        else if (maxLength <= 100)
            return $"Super yacht slip up to {maxLength}'L x {maxBeam}'B - ${pricePerDay}/day";
        else
            return $"Ultra luxury slip up to {maxLength}'L x {maxBeam}'B - ${pricePerDay}/day";
    }

    private static DateTime GenerateTimestamp(Random random)
    {
        var startDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        var timeSpan = endDate - startDate;
        var randomTimeSpan = TimeSpan.FromTicks((long)(random.NextDouble() * timeSpan.Ticks));
        return startDate + randomTimeSpan;
    }

    private record SlipClassificationData(string Name, decimal MaxLength, decimal MaxBeam, int BasePrice);
} 