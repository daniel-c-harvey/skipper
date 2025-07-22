using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperDataAgent;

public static class GenerateSlipReservations
{
    // RentalStatus values with realistic distribution weights for 20-year backlog
    private static readonly (RentalStatus Status, int Weight)[] StatusDistribution =
    [
        (RentalStatus.Expired, 65),      // Most rentals are from the past
        (RentalStatus.Active, 15),       // Some current rentals
        (RentalStatus.Pending, 10),      // Some pending future rentals
        (RentalStatus.Quoted, 7),        // Some quoted rentals
        (RentalStatus.Cancelled, 3)      // Some cancelled rentals
    ];

    // PriceUnit values with realistic distribution weights
    private static readonly (PriceUnit Unit, int Weight)[] PriceUnitDistribution =
    [
        (PriceUnit.PerDay, 50),      // Most common for short-term rentals
        (PriceUnit.PerWeek, 30),     // Common for weekly rentals
        (PriceUnit.PerMonth, 15),    // Less common for monthly rentals
        (PriceUnit.PerYear, 5)       // Rare for annual rentals
    ];

    // Rental duration ranges with weights (in days)
    private static readonly (int MinDays, int MaxDays, int Weight)[] DurationDistribution =
    [
        (1, 3, 25),      // Very short stays (1-3 days)
        (4, 7, 35),      // Short stays (4-7 days)
        (8, 14, 25),     // Medium stays (1-2 weeks)
        (15, 30, 10),    // Long stays (2-4 weeks)
        (31, 90, 5)      // Extended stays (1-3 months)
    ];

    public static async Task GenerateAsync(SkipperContext dbContext, ILogger<SlipReservationRepository> logger, int totalRecords = 50000, int batchSize = 1000)
    {
        logger.LogInformation("Starting rental agreement generation: {TotalRecords} records in batches of {BatchSize}", totalRecords, batchSize);
        
        var repository = new SlipReservationRepository(dbContext, logger);
        var random = new Random(42); // Fixed seed for reproducible results
        
        // Query existing data from database
        var (slips, vessels, slipClassifications) = await GetExistingDataAsync(dbContext, logger);
        if (!slips.Any() || !vessels.Any() || !slipClassifications.Any())
        {
            logger.LogError("Missing required data. Please ensure slips, vessels, and slip classifications exist in database.");
            return;
        }
        
        logger.LogInformation("Found {SlipCount} slips, {VesselCount} vessels, {ClassificationCount} classifications", 
            slips.Count(), vessels.Count(), slipClassifications.Count());
        
        // Find compatible vessel-slip matches
        var compatibleMatches = FindCompatibleMatches(slips, vessels, slipClassifications);
        if (!compatibleMatches.Any())
        {
            logger.LogError("No compatible vessel-slip matches found. Check vessel dimensions vs slip classification constraints.");
            return;
        }
        
        logger.LogInformation("Found {MatchCount} compatible vessel-slip combinations", compatibleMatches.Count());
        
        var totalBatches = (int)Math.Ceiling((double)totalRecords / batchSize);
        
        for (int batch = 1; batch <= totalBatches; batch++)
        {
            var recordsInBatch = Math.Min(batchSize, totalRecords - (batch - 1) * batchSize);
            logger.LogInformation("Generating batch {Batch}/{TotalBatches}: {RecordsInBatch} records", batch, totalBatches, recordsInBatch);
            
            var slipReservationEntities = new List<SlipReservationEntity>();
            
            for (int i = 0; i < recordsInBatch; i++)
            {
                var rentalAgreement = GenerateSlipReservation(random, compatibleMatches, slipClassifications);
                slipReservationEntities.Add(rentalAgreement);
            }
            
            // Insert batch
            foreach (var rental in slipReservationEntities)
            {
                await repository.AddAsync(rental);
            }
            await repository.SaveChangesAsync();
            
            logger.LogInformation("Batch {Batch} completed: {RecordsInserted} rental agreements inserted", batch, slipReservationEntities.Count);
        }
        
        logger.LogInformation("Rental agreement generation completed: {TotalRecords} agreements generated", totalRecords);
    }

    private static async Task<(IEnumerable<SlipEntity> Slips, IEnumerable<VesselEntity> Vessels, IEnumerable<SlipClassificationEntity> Classifications)> 
        GetExistingDataAsync(SkipperContext dbContext, ILogger logger)
    {
        try
        {
            var slips = await dbContext.Slips
                .Where(s => !s.IsDeleted && (s.Status == SlipStatus.Available || s.Status == SlipStatus.Booked || s.Status == SlipStatus.InUse))
                .ToListAsync();
            
            var vessels = await dbContext.Vessels
                .Where(v => !v.IsDeleted)
                .ToListAsync();
            
            var classifications = await dbContext.SlipClassifications
                .Where(sc => !sc.IsDeleted)
                .ToListAsync();
            
            logger.LogInformation("Retrieved {SlipCount} active slips, {VesselCount} vessels, {ClassificationCount} classifications from database", 
                slips.Count, vessels.Count, classifications.Count);
            
            return (slips, vessels, classifications);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving data from database");
            return (Enumerable.Empty<SlipEntity>(), Enumerable.Empty<VesselEntity>(), Enumerable.Empty<SlipClassificationEntity>());
        }
    }

    private static List<(long VesselId, long SlipId, long ClassificationId)> FindCompatibleMatches(
        IEnumerable<SlipEntity> slips, 
        IEnumerable<VesselEntity> vessels, 
        IEnumerable<SlipClassificationEntity> classifications)
    {
        var matches = new List<(long VesselId, long SlipId, long ClassificationId)>();
        var classificationDict = classifications.ToDictionary(c => c.Id);
        
        foreach (var vessel in vessels)
        {
            foreach (var slip in slips)
            {
                if (classificationDict.TryGetValue(slip.SlipClassificationId, out var classification))
                {
                    // Check if vessel can fit in slip based on dimensions
                    if (vessel.Length <= classification.MaxLength && vessel.Beam <= classification.MaxBeam)
                    {
                        matches.Add((vessel.Id, slip.Id, slip.SlipClassificationId));
                    }
                }
            }
        }
        
        return matches;
    }

    private static SlipReservationEntity GenerateSlipReservation(
        Random random, 
        List<(long VesselId, long SlipId, long ClassificationId)> compatibleMatches,
        IEnumerable<SlipClassificationEntity> classifications)
    {
        var (vesselId, slipId, classificationId) = compatibleMatches[random.Next(compatibleMatches.Count)];
        var classification = classifications.First(c => c.Id == classificationId);
        
        var (startDate, endDate) = GenerateDateRange(random);
        var status = GetWeightedStatus(random, startDate, endDate);
        var (priceRate, priceUnit) = GeneratePriceRate(random, classification);
        var (createdAt, updatedAt) = GenerateTimestamps(random, startDate);

        return new SlipReservationEntity
        {
            SlipId = slipId,
            VesselId = vesselId,
            StartDate = startDate,
            EndDate = endDate,
            PriceRate = priceRate,
            PriceUnit = priceUnit,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            IsDeleted = false
        };
    }

    private static (DateTime StartDate, DateTime EndDate) GenerateDateRange(Random random)
    {
        // Generate dates spanning 20 years (2004-2024)
        var startBase = new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endBase = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        var totalDays = (endBase - startBase).Days;
        
        var startDate = startBase.AddDays(random.Next(totalDays));
        
        // Generate rental duration
        var durationRange = GetWeightedDuration(random);
        var duration = random.Next(durationRange.MinDays, durationRange.MaxDays + 1);
        
        var endDate = startDate.AddDays(duration);
        
        return (startDate, endDate);
    }

    private static RentalStatus GetWeightedStatus(Random random, DateTime startDate, DateTime endDate)
    {
        var now = DateTime.UtcNow;
        
        // Override status based on dates for more realism
        if (endDate < now)
        {
            // Past rentals: mostly expired, some cancelled
            return random.Next(100) < 85 ? RentalStatus.Expired : RentalStatus.Cancelled;
        }
        else if (startDate <= now && now <= endDate)
        {
            // Current rentals: active
            return RentalStatus.Active;
        }
        else
        {
            // Future rentals: quoted or pending
            return random.Next(100) < 60 ? RentalStatus.Pending : RentalStatus.Quoted;
        }
    }

    private static (int PriceRate, PriceUnit PriceUnit) GeneratePriceRate(Random random, SlipClassificationEntity classification)
    {
        var priceUnit = GetWeightedPriceUnit(random);
        
        // Base price from classification (in cents)
        var basePrice = classification.BasePrice;
        
        // Apply pricing multipliers based on unit
        var multiplier = priceUnit switch
        {
            PriceUnit.PerDay => 1.0,
            PriceUnit.PerWeek => 6.5,  // Weekly discount (~7% off daily rate)
            PriceUnit.PerMonth => 25.0, // Monthly discount (~17% off daily rate)
            PriceUnit.PerYear => 280.0, // Annual discount (~23% off daily rate)
            _ => 1.0
        };
        
        // Add random variation (±20%)
        var variation = random.NextDouble() * 0.4 + 0.8; // 0.8 to 1.2
        var priceRate = (int)(basePrice * multiplier * variation);
        
        return (priceRate, priceUnit);
    }

    private static (DateTime CreatedAt, DateTime UpdatedAt) GenerateTimestamps(Random random, DateTime startDate)
    {
        // Created date is typically before the start date
        var daysBeforeStart = random.Next(1, 91); // 1-90 days before start
        var createdAt = startDate.AddDays(-daysBeforeStart);
        
        // Updated date is typically close to created date, but can be later
        var daysAfterCreated = random.Next(0, 31); // 0-30 days after created
        var updatedAt = createdAt.AddDays(daysAfterCreated);
        
        return (createdAt, updatedAt);
    }

    private static (int MinDays, int MaxDays) GetWeightedDuration(Random random)
    {
        var totalWeight = DurationDistribution.Sum(d => d.Weight);
        var randomValue = random.Next(totalWeight);
        var currentWeight = 0;
        
        foreach (var (minDays, maxDays, weight) in DurationDistribution)
        {
            currentWeight += weight;
            if (randomValue < currentWeight)
            {
                return (minDays, maxDays);
            }
        }
        
        // Fallback to short stay
        return (4, 7);
    }

    private static PriceUnit GetWeightedPriceUnit(Random random)
    {
        var totalWeight = PriceUnitDistribution.Sum(p => p.Weight);
        var randomValue = random.Next(totalWeight);
        var currentWeight = 0;
        
        foreach (var (unit, weight) in PriceUnitDistribution)
        {
            currentWeight += weight;
            if (randomValue < currentWeight)
            {
                return unit;
            }
        }
        
        // Fallback to per day
        return PriceUnit.PerDay;
    }
} 