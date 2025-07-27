using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperDataAgent;

public static class GenerateCustomers
{
    // Realistic first names with distribution weights
    private static readonly (string Name, int Weight)[] FirstNames =
    [
        ("James", 15), ("Mary", 12), ("John", 14), ("Patricia", 10), ("Robert", 13),
        ("Jennifer", 9), ("Michael", 12), ("Linda", 8), ("William", 11), ("Elizabeth", 8),
        ("David", 10), ("Barbara", 7), ("Richard", 9), ("Susan", 7), ("Joseph", 8),
        ("Jessica", 7), ("Thomas", 8), ("Sarah", 8), ("Christopher", 7), ("Karen", 6),
        ("Charles", 7), ("Nancy", 6), ("Daniel", 7), ("Lisa", 6), ("Matthew", 6),
        ("Betty", 5), ("Anthony", 6), ("Helen", 5), ("Mark", 6), ("Sandra", 5),
        ("Donald", 6), ("Donna", 5), ("Steven", 5), ("Carol", 5), ("Paul", 5),
        ("Ruth", 4), ("Andrew", 5), ("Sharon", 4), ("Joshua", 5), ("Michelle", 4),
        ("Kenneth", 5), ("Laura", 4), ("Kevin", 4), ("Sarah", 4), ("Brian", 4),
        ("Kimberly", 4), ("George", 4), ("Deborah", 4), ("Edward", 4), ("Dorothy", 3)
    ];

    // Realistic last names with distribution weights  
    private static readonly (string Name, int Weight)[] LastNames =
    [
        ("Smith", 25), ("Johnson", 20), ("Williams", 18), ("Brown", 16), ("Jones", 15),
        ("Garcia", 12), ("Miller", 11), ("Davis", 10), ("Rodriguez", 9), ("Martinez", 8),
        ("Hernandez", 8), ("Lopez", 7), ("Gonzalez", 7), ("Wilson", 6), ("Anderson", 6),
        ("Thomas", 6), ("Taylor", 6), ("Moore", 5), ("Jackson", 5), ("Martin", 5),
        ("Lee", 5), ("Perez", 4), ("Thompson", 4), ("White", 4), ("Harris", 4),
        ("Sanchez", 4), ("Clark", 3), ("Ramirez", 3), ("Lewis", 3), ("Robinson", 3),
        ("Walker", 3), ("Young", 3), ("Allen", 3), ("King", 2), ("Wright", 2),
        ("Scott", 2), ("Torres", 2), ("Nguyen", 2), ("Hill", 2), ("Flores", 2),
        ("Green", 2), ("Adams", 2), ("Nelson", 2), ("Baker", 2), ("Hall", 2),
        ("Rivera", 2), ("Campbell", 2), ("Mitchell", 2), ("Carter", 2), ("Roberts", 2)
    ];

    // US Cities with realistic marina locations
    private static readonly (string City, string State, string ZipCode, int Weight)[] Locations =
    [
        ("Miami", "FL", "33101", 15), ("San Diego", "CA", "92101", 12), ("Seattle", "WA", "98101", 10),
        ("Newport Beach", "CA", "92660", 8), ("Key West", "FL", "33040", 8), ("Charleston", "SC", "29401", 7),
        ("Annapolis", "MD", "21401", 6), ("Norfolk", "VA", "23501", 6), ("Boston", "MA", "02101", 8),
        ("Savannah", "GA", "31401", 5), ("San Francisco", "CA", "94101", 7), ("Portland", "OR", "97201", 5),
        ("Tampa", "FL", "33601", 6), ("Jacksonville", "FL", "32201", 5), ("Virginia Beach", "VA", "23451", 5),
        ("Long Beach", "CA", "90801", 4), ("Fort Lauderdale", "FL", "33301", 6), ("Monterey", "CA", "93940", 3),
        ("Galveston", "TX", "77550", 4), ("Mobile", "AL", "36601", 3), ("Pensacola", "FL", "32501", 3),
        ("Wilmington", "NC", "28401", 3), ("Newport", "RI", "02840", 4), ("Bar Harbor", "ME", "04609", 2),
        ("Mystic", "CT", "06355", 2), ("Cape May", "NJ", "08204", 2)
    ];

    // Street name components for address generation
    private static readonly string[] StreetNames =
    [
        "Ocean", "Harbor", "Marina", "Bay", "Coastal", "Sunset", "Seaside", "Dockside", "Waterfront", "Pier",
        "Yacht", "Anchor", "Nautical", "Sailboat", "Compass", "Lighthouse", "Seashore", "Wharf", "Port",
        "Marine", "Admiral", "Captain", "Navigator", "Beacon", "Tide", "Wave", "Sea", "Salt", "Coral", "Shell"
    ];

    private static readonly string[] StreetTypes = ["Dr", "St", "Ave", "Blvd", "Way", "Ln", "Ct", "Pl", "Cir"];

    // Email domains for realistic email generation
    private static readonly string[] EmailDomains =
    [
        "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "aol.com", "icloud.com", 
        "comcast.net", "verizon.net", "att.net", "sbcglobal.net"
    ];

    // Vessel ownership patterns - some customers own multiple vessels
    private static readonly (int MinVessels, int MaxVessels, int Weight)[] VesselOwnershipDistribution =
    [
        (1, 1, 70),    // Most customers own 1 vessel
        (2, 2, 20),    // Some own 2 vessels  
        (3, 3, 7),     // Few own 3 vessels
        (4, 5, 3)      // Very few own 4-5 vessels
    ];

    public static async Task GenerateAsync(SkipperContext dbContext, ILogger<VesselOwnerCustomerRepository> logger, int totalCustomers = 10000, int batchSize = 500)
    {
        logger.LogInformation("Starting customer generation: {TotalCustomers} customers in batches of {BatchSize}", totalCustomers, batchSize);
        
        var repository = new VesselOwnerCustomerRepository(dbContext, logger);
        var random = new Random(42); // Fixed seed for reproducible results
        
        // Query existing vessels from database
        var availableVessels = await GetAvailableVesselsAsync(dbContext, logger);
        if (!availableVessels.Any())
        {
            logger.LogError("No vessels found in database. Please ensure vessels exist before generating customers.");
            return;
        }

        logger.LogInformation("Found {VesselCount} available vessels for customer assignment", availableVessels.Count());
        
        var totalBatches = (int)Math.Ceiling((double)totalCustomers / batchSize);
        var vesselsAssigned = new HashSet<long>(); // Track which vessels have been assigned
        
        for (int batch = 1; batch <= totalBatches; batch++)
        {
            var customersInBatch = Math.Min(batchSize, totalCustomers - (batch - 1) * batchSize);
            logger.LogInformation("Generating batch {Batch}/{TotalBatches}: {CustomersInBatch} customers", batch, totalBatches, customersInBatch);
            
            var customerEntities = new List<VesselOwnerCustomerEntity>();
            
            for (int i = 0; i < customersInBatch; i++)
            {
                var customer = GenerateCustomer(random, availableVessels, vesselsAssigned, batch, i);
                customerEntities.Add(customer);
            }
            
            // Insert batch
            foreach (var customer in customerEntities)
            {
                await repository.AddAsync(customer);
            }
            
            logger.LogInformation("Batch {Batch} completed: {CustomersInserted} customers inserted, {VesselsAssigned} vessels assigned", 
                batch, customerEntities.Count, vesselsAssigned.Count);
        }
        
        var unassignedVessels = availableVessels.Count() - vesselsAssigned.Count;
        logger.LogInformation("Customer generation completed: {TotalCustomers} customers generated, {VesselsAssigned} vessels assigned, {UnassignedVessels} vessels unassigned", 
            totalCustomers, vesselsAssigned.Count, unassignedVessels);
    }

    private static async Task<IEnumerable<VesselEntity>> GetAvailableVesselsAsync(SkipperContext dbContext, ILogger logger)
    {
        try
        {
            var vessels = await dbContext.Vessels
                .Where(v => !v.IsDeleted)
                .ToListAsync();
            
            logger.LogInformation("Retrieved {VesselCount} available vessels from database", vessels.Count);
            return vessels;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving vessels from database");
            return Enumerable.Empty<VesselEntity>();
        }
    }

    private static VesselOwnerCustomerEntity GenerateCustomer(
        Random random, 
        IEnumerable<VesselEntity> availableVessels, 
        HashSet<long> vesselsAssigned,
        int batchNumber, 
        int customerIndex)
    {
        var firstName = GetWeightedFirstName(random);
        var lastName = GetWeightedLastName(random);
        var location = GetWeightedLocation(random);
        var vesselOwnership = GetWeightedVesselOwnership(random);
        
        // Generate customer data
        var accountNumber = GenerateAccountNumber(batchNumber, customerIndex);
        var customerName = $"{firstName} {lastName}";
        var licenseNumber = GenerateLicenseNumber(random);
        var licenseExpiryDate = GenerateLicenseExpiryDate(random);
        
        // Generate contact data
        var email = GenerateEmail(firstName, lastName, random);
        var phoneNumber = GeneratePhoneNumber(random);
        
        // Generate address data
        var address = GenerateAddress(random, location);
        
        // Generate timestamps
        var createdAt = GenerateCreatedAt(random);
        var updatedAt = GenerateUpdatedAt(random, createdAt);
        
        // Select vessels for this customer
        var customerVessels = SelectVesselsForCustomer(random, availableVessels, vesselsAssigned, vesselOwnership);
        
        var addressEntity = new AddressEntity
        {
            Address1 = address.Address1,
            Address2 = address.Address2,
            City = location.City,
            State = location.State,
            ZipCode = location.ZipCode,
            Country = "USA",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            IsDeleted = false
        };

        var contactEntity = new ContactEntity
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            Address = addressEntity,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            IsDeleted = false
        };

        // Create the TPH customer entity with all properties directly
        var vesselOwnerCustomer = new VesselOwnerCustomerEntity
        {
            AccountNumber = accountNumber,
            Name = customerName,
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            LicenseNumber = licenseNumber,
            LicenseExpiryDate = licenseExpiryDate,
            ContactId = contactEntity.Id, // Will be set after contact is saved
            Contact = contactEntity,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            IsDeleted = false
        };

        // Create vessel relationships pointing to customer (not profile)
        var vesselOwnerVessels = customerVessels.Select(vessel => new VesselOwnerVesselEntity
        {
            VesselId = vessel.Id,
            Vessel = vessel,
            VesselOwnerCustomerId = vesselOwnerCustomer.Id, // Will be set after customer is saved
            VesselOwnerCustomer = vesselOwnerCustomer,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            IsDeleted = false
        }).ToList();

        // Set the vessels collection on the customer
        vesselOwnerCustomer.VesselOwnerVessels = vesselOwnerVessels;

        return vesselOwnerCustomer;
    }

    private static string GenerateAccountNumber(int batchNumber, int customerIndex)
    {
        // Format: VO-BBBB-CCCC (VesselOwner-Batch-Customer)
        return $"VO-{batchNumber:D4}-{customerIndex + 1:D4}";
    }

    private static string GenerateLicenseNumber(Random random)
    {
        // Generate realistic boat license format: AA-1234-BB
        var letters1 = $"{(char)('A' + random.Next(26))}{(char)('A' + random.Next(26))}";
        var numbers = random.Next(1000, 9999);
        var letters2 = $"{(char)('A' + random.Next(26))}{(char)('A' + random.Next(26))}";
        return $"{letters1}-{numbers}-{letters2}";
    }

    private static DateTime GenerateLicenseExpiryDate(Random random)
    {
        // Licenses expire 1-5 years from now
        var yearsFromNow = random.Next(1, 6);
        return DateTime.UtcNow.AddYears(yearsFromNow).Date;
    }

    private static string GenerateEmail(string firstName, string lastName, Random random)
    {
        var domain = EmailDomains[random.Next(EmailDomains.Length)];
        var emailVariations = new[]
        {
            $"{firstName.ToLower()}.{lastName.ToLower()}@{domain}",
            $"{firstName.ToLower()}{lastName.ToLower()}@{domain}",
            $"{firstName.ToLower()}{lastName.ToLower()}{random.Next(10, 99)}@{domain}",
            $"{firstName.Substring(0, 1).ToLower()}{lastName.ToLower()}@{domain}",
            $"{firstName.ToLower()}.{lastName.Substring(0, 1).ToLower()}@{domain}"
        };
        
        return emailVariations[random.Next(emailVariations.Length)];
    }

    private static string GeneratePhoneNumber(Random random)
    {
        // Generate US phone number format: (XXX) XXX-XXXX
        var areaCode = random.Next(200, 999); // Valid area codes start from 200
        var exchange = random.Next(200, 999); // Valid exchanges start from 200  
        var number = random.Next(1000, 9999);
        return $"({areaCode}) {exchange}-{number}";
    }

    private static (string Address1, string Address2) GenerateAddress(Random random, (string City, string State, string ZipCode, int Weight) location)
    {
        var streetNumber = random.Next(1, 9999);
        var streetName = StreetNames[random.Next(StreetNames.Length)];
        var streetType = StreetTypes[random.Next(StreetTypes.Length)];
        var address1 = $"{streetNumber} {streetName} {streetType}";
        
        // 30% chance of having Address2 (apartment, suite, etc.)
        string address2 = null;
        if (random.Next(100) < 30)
        {
            var unitTypes = new[] { "Apt", "Suite", "Unit", "#" };
            var unitType = unitTypes[random.Next(unitTypes.Length)];
            var unitNumber = random.Next(1, 999);
            address2 = $"{unitType} {unitNumber}";
        }
        
        return (address1, address2);
    }

    private static DateTime GenerateCreatedAt(Random random)
    {
        // Customers created within the last 5 years
        var daysAgo = random.Next(1, 1825); // 1 day to 5 years ago
        return DateTime.UtcNow.AddDays(-daysAgo);
    }

    private static DateTime GenerateUpdatedAt(Random random, DateTime createdAt)
    {
        // Updated date is between created date and now
        var daysSinceCreated = (DateTime.UtcNow - createdAt).Days;
        if (daysSinceCreated <= 0) return createdAt;
        
        var daysAfterCreated = random.Next(0, daysSinceCreated + 1);
        return createdAt.AddDays(daysAfterCreated);
    }

    private static List<VesselEntity> SelectVesselsForCustomer(
        Random random, 
        IEnumerable<VesselEntity> availableVessels, 
        HashSet<long> vesselsAssigned, 
        (int MinVessels, int MaxVessels) ownership)
    {
        var unassignedVessels = availableVessels.Where(v => !vesselsAssigned.Contains(v.Id)).ToList();
        
        // If not enough unassigned vessels, allow some vessels to be shared (rare in real world, but for data generation)
        if (unassignedVessels.Count < ownership.MinVessels)
        {
            unassignedVessels = availableVessels.ToList();
        }
        
        var vesselCount = random.Next(ownership.MinVessels, ownership.MaxVessels + 1);
        vesselCount = Math.Min(vesselCount, unassignedVessels.Count);
        
        var selectedVessels = new List<VesselEntity>();
        for (int i = 0; i < vesselCount; i++)
        {
            var vessel = unassignedVessels[random.Next(unassignedVessels.Count)];
            selectedVessels.Add(vessel);
            vesselsAssigned.Add(vessel.Id);
            unassignedVessels.Remove(vessel); // Don't assign the same vessel twice to the same customer
        }
        
        return selectedVessels;
    }

    private static string GetWeightedFirstName(Random random)
    {
        var totalWeight = FirstNames.Sum(n => n.Weight);
        var randomValue = random.Next(totalWeight);
        var currentWeight = 0;
        
        foreach (var (name, weight) in FirstNames)
        {
            currentWeight += weight;
            if (randomValue < currentWeight)
            {
                return name;
            }
        }
        
        return FirstNames[0].Name; // Fallback
    }

    private static string GetWeightedLastName(Random random)
    {
        var totalWeight = LastNames.Sum(n => n.Weight);
        var randomValue = random.Next(totalWeight);
        var currentWeight = 0;
        
        foreach (var (name, weight) in LastNames)
        {
            currentWeight += weight;
            if (randomValue < currentWeight)
            {
                return name;
            }
        }
        
        return LastNames[0].Name; // Fallback
    }

    private static (string City, string State, string ZipCode, int Weight) GetWeightedLocation(Random random)
    {
        var totalWeight = Locations.Sum(l => l.Weight);
        var randomValue = random.Next(totalWeight);
        var currentWeight = 0;
        
        foreach (var location in Locations)
        {
            currentWeight += location.Weight;
            if (randomValue < currentWeight)
            {
                return location;
            }
        }
        
        return Locations[0]; // Fallback
    }

    private static (int MinVessels, int MaxVessels) GetWeightedVesselOwnership(Random random)
    {
        var totalWeight = VesselOwnershipDistribution.Sum(v => v.Weight);
        var randomValue = random.Next(totalWeight);
        var currentWeight = 0;
        
        foreach (var (minVessels, maxVessels, weight) in VesselOwnershipDistribution)
        {
            currentWeight += weight;
            if (randomValue < currentWeight)
            {
                return (minVessels, maxVessels);
            }
        }
        
        return (1, 1); // Fallback
    }
} 