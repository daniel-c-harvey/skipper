using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperDataAgent;

public static class GenerateSlips
{
    // Marina location codes (different docks/piers)
    private static readonly string[] LocationCodes =
    [
        "DOCK-A", "DOCK-B", "DOCK-C", "DOCK-D", "DOCK-E", "DOCK-F",
        "PIER-1", "PIER-2", "PIER-3", "PIER-4", "PIER-5", "PIER-6",
        "NORTH", "SOUTH", "EAST", "WEST", "CENTER", "MARINA-1", "MARINA-2"
    ];

    // SlipStatus values with realistic distribution weights
    private static readonly (SlipStatus Status, int Weight)[] StatusDistribution =
    [
        (SlipStatus.Available, 60),
        (SlipStatus.Booked, 25),
        (SlipStatus.InUse, 8),
        (SlipStatus.Maintenance, 4),
        (SlipStatus.Sold, 2),
        (SlipStatus.Archived, 1)
    ];

    public static async Task GenerateAsync(SkipperContext dbContext, ILogger<SlipRepository> logger, int totalRecords = 250, int batchSize = 50)
    {
        logger.LogInformation("Starting slip generation: {TotalRecords} records in batches of {BatchSize}", totalRecords, batchSize);
        
        var repository = new SlipRepository(dbContext, logger);
        var random = new Random(42); // Fixed seed for reproducible results
        
        // Query existing slip classifications from database
        var slipClassifications = await GetSlipClassificationsAsync(dbContext, logger);
        if (!slipClassifications.Any())
        {
            logger.LogError("No slip classifications found in database. Please generate slip classifications first.");
            return;
        }
        
        logger.LogInformation("Found {ClassificationCount} slip classifications to use for slip generation", slipClassifications.Count());
        
        var usedSlipNumbers = new HashSet<string>();
        var totalBatches = (int)Math.Ceiling((double)totalRecords / batchSize);
        
        for (int batch = 1; batch <= totalBatches; batch++)
        {
            var recordsInBatch = Math.Min(batchSize, totalRecords - (batch - 1) * batchSize);
            logger.LogInformation("Generating batch {Batch}/{TotalBatches}: {RecordsInBatch} records", batch, totalBatches, recordsInBatch);
            
            var slips = new List<SlipEntity>();
            
            for (int i = 0; i < recordsInBatch; i++)
            {
                var slip = GenerateSlip(random, batch, i + 1, slipClassifications, usedSlipNumbers);
                slips.Add(slip);
            }
            
            Task[] slipTasks = slips.Select(Task (slip) => repository.AddAsync(slip)).ToArray();
            Task.WaitAll(slipTasks);
            
            logger.LogInformation("Batch {Batch} completed: {RecordsInserted} slips inserted", batch, slips.Count);
        }
        
        logger.LogInformation("Slip generation completed: {TotalRecords} slips generated across {ClassificationCount} classifications", 
            totalRecords, slipClassifications.Count());
    }

    private static async Task<IEnumerable<SlipClassificationEntity>> GetSlipClassificationsAsync(SkipperContext dbContext, ILogger logger)
    {
        try
        {
            var classifications = await dbContext.SlipClassifications
                .Where(sc => !sc.IsDeleted)
                .ToListAsync();
            
            logger.LogInformation("Retrieved {Count} active slip classifications from database", classifications.Count);
            return classifications;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving slip classifications from database");
            return Enumerable.Empty<SlipClassificationEntity>();
        }
    }

    private static SlipEntity GenerateSlip(Random random, int batch, int recordNum, IEnumerable<SlipClassificationEntity> classifications, HashSet<string> usedSlipNumbers)
    {
        var slipClassification = classifications.ElementAt(random.Next(classifications.Count()));
        var slipNumber = GenerateUniqueSlipNumber(random, batch, recordNum, usedSlipNumbers);
        var locationCode = LocationCodes[random.Next(LocationCodes.Length)];
        var status = GetWeightedStatus(random);
        var timestamp = GenerateTimestamp(random);

        return new SlipEntity
        {
            SlipClassificationId = slipClassification.Id,
            SlipNumber = slipNumber,
            LocationCode = locationCode,
            Status = status,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
            IsDeleted = false
        };
    }

    private static string GenerateUniqueSlipNumber(Random random, int batch, int recordNum, HashSet<string> usedSlipNumbers)
    {
        string slipNumber;
        int attempts = 0;
        
        do
        {
            slipNumber = GenerateSlipNumber(random, batch, recordNum);
            attempts++;
            
            if (attempts > 100)
            {
                // Fallback to sequential approach if too many collisions
                slipNumber = $"SLIP-{batch}{usedSlipNumbers.Count:D4}{random.Next(100, 999)}";
                break;
            }
        } while (usedSlipNumbers.Contains(slipNumber));
        
        usedSlipNumbers.Add(slipNumber);
        return slipNumber;
    }

    private static string GenerateSlipNumber(Random random, int batch, int recordNum)
    {
        var locationCode = LocationCodes[random.Next(LocationCodes.Length)];
        var slipNum = random.Next(1, 999);
        
        if (locationCode.StartsWith("DOCK"))
        {
            return $"{locationCode}-{slipNum:D3}";
        }
        else if (locationCode.StartsWith("PIER"))
        {
            return $"{locationCode}-{slipNum:D2}";
        }
        else
        {
            return $"{locationCode}-{slipNum:D3}";
        }
    }

    private static SlipStatus GetWeightedStatus(Random random)
    {
        var totalWeight = StatusDistribution.Sum(s => s.Weight);
        var randomValue = random.Next(totalWeight);
        var currentWeight = 0;
        
        foreach (var (status, weight) in StatusDistribution)
        {
            currentWeight += weight;
            if (randomValue < currentWeight)
            {
                return status;
            }
        }
        
        // Fallback to Available if something goes wrong
        return SlipStatus.Available;
    }

    private static DateTime GenerateTimestamp(Random random)
    {
        var startDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        var timeSpan = endDate - startDate;
        var randomTimeSpan = TimeSpan.FromTicks((long)(random.NextDouble() * timeSpan.Ticks));
        return startDate + randomTimeSpan;
    }
}