using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperDataAgent;

public static class GenerateVessels
{
    private static readonly string[] NamePrefixes =
    [
        "Sea", "Ocean", "Wave", "Wind", "Storm", "Thunder", "Lightning", "Coral", "Pearl", "Crystal", 
        "Golden", "Silver", "Blue", "Deep", "Royal", "Majestic", "Swift", "Mighty", "Bold", "Brave", 
        "Wild", "Free", "Grand", "Noble", "Proud", "Strong", "Fast", "True", "Pure", "Bright", 
        "Ancient", "Modern", "Classic", "Vintage", "Elite", "Prime", "Ultra", "Super", "Mega", "Epic",
        "Pacific", "Atlantic", "Arctic", "Caribbean", "Mediterranean", "Indian", "Southern", "Northern",
        "Eastern", "Western", "Central", "Coastal", "Maritime", "Nautical", "Naval", "Fishing", "Trading",
        "Explorer", "Adventure", "Discovery", "Voyage", "Journey", "Quest", "Mission", "Expedition",
        "Pioneer", "Trailblazer", "Pathfinder", "Navigator", "Captain", "Commander", "Admiral", "Commodore",
        "Skipper", "Helmsman", "Pilot", "Steersman", "Mariner", "Sailor", "Seafarer", "Seaman",
        "Fisherman", "Whaler", "Trader", "Merchant", "Privateer", "Pirate", "Corsair", "Buccaneer",
        "Raider", "Hunter", "Chaser", "Pursuer", "Tracker", "Scout", "Spy", "Lookout", "Watchman",
        "Guardian", "Protector", "Defender", "Warrior", "Fighter", "Champion", "Hero", "Legend",
        "Myth", "Fable", "Tale", "Story", "Saga", "Chronicle", "Annals", "History", "Heritage", "Legacy",
        "Tradition", "Custom", "Culture", "Civilization", "Empire", "Kingdom", "Realm", "Domain", "Territory",
        "Province", "Region", "District", "Zone", "Area", "Sector", "Quarter", "Section", "Part", "Piece",
        "Fragment", "Element", "Component", "Factor", "Aspect", "Feature", "Characteristic", "Quality", "Property",
        "Attribute", "Trait", "Nature", "Essence", "Spirit", "Soul", "Heart", "Core", "Center", "Middle", "Midst",
        "Interior", "Inside", "Internal", "Inner", "Inward", "Central", "Focal", "Primary", "Main", "Principal",
        "Chief", "Leading", "Foremost", "First", "Top", "Best", "Finest", "Greatest", "Largest", "Biggest",
        "Strongest", "Fastest", "Swiftest", "Quickest", "Rapid", "Speedy", "Fleet", "Agile", "Nimble",
        "Light", "Bright", "Shiny", "Gleaming", "Glowing", "Radiant", "Brilliant", "Dazzling",
        "Stunning", "Amazing", "Wonderful", "Fantastic", "Incredible", "Unbelievable", "Extraordinary",
        "Remarkable", "Notable", "Distinguished", "Eminent", "Prominent", "Famous", "Renowned",
        "Celebrated", "Honored", "Respected", "Esteemed", "Venerated", "Revered", "Worshipped",
        "Adored", "Beloved", "Cherished", "Treasured", "Valued", "Precious", "Rare", "Scarce",
        "Unique", "Singular", "Solitary", "Lonely", "Isolated", "Remote", "Distant", "Faraway",
        "Faroff", "Farflung", "Widespread", "Extensive", "Vast", "Immense", "Enormous", "Gigantic",
        "Colossal", "Massive", "Huge", "Tremendous", "Monumental", "Titanic", "Herculanean", "Cyclopean",
        "Gargantuan", "Mammoth", "Elephantine", "Whale", "Leviathan", "Kraken", "Serpent", "Dragon",
        "Phoenix", "Griffin", "Pegasus", "Unicorn", "Centaur", "Siren", "Mermaid", "Nymph", "Dryad",
        "Naiad", "Oceanid", "Nereid", "Triton", "Poseidon", "Neptune", "Amphitrite", "Thetis", "Galatea", "Calypso",
        "Circe", "Scylla", "Charybdis", "Harpy", "Gorgon", "Medusa", "Stheno", "Euryale", "Hydra", "Cerberus",
        "Chimera", "Minotaur", "Cyclops", "Polyphemus", "Aeolus", "Zephyr", "Boreas", "Notus", "Eurus", "Aquilo"
    ];

    private static readonly string[] NameSuffixes = 
    {
        "Breeze", "Dream", "Runner", "Glory", "Star", "Belle", "Chaser", "Horizon", "Bay", "Walker", 
        "Reef", "Gate", "Stream", "Bell", "Isle", "Dance", "Magic", "Wind", "Valley", "Sound", 
        "Largo", "Monica", "Peak", "Spirit", "Treasure", "Coast", "Thunder", "Falls", "Pride", "City", 
        "Quest", "Adventure", "Explorer", "Voyager", "Seeker", "Hunter", "Raider", "Warrior", "Guardian", "Champion",
        "Wanderer", "Traveler", "Pilgrim", "Nomad", "Rover", "Rambler", "Stroller", "Hiker", "Climber", "Mountaineer",
        "Summiter", "Peaker", "Ridger", "Valleyer", "Canyoner", "Gorge", "Ravine", "Gully", "Arroyo", "Wash",
        "Creek", "Brook", "River", "Tributary", "Branch", "Fork", "Confluence", "Delta", "Estuary",
        "Inlet", "Cove", "Harbor", "Port", "Dock", "Pier", "Wharf", "Quay", "Berth", "Mooring",
        "Anchor", "Buoy", "Beacon", "Lighthouse", "Tower", "Spire", "Pinnacle", "Summit", "Crest", "Ridge",
        "Slope", "Hillside", "Mountainside", "Cliff", "Bluff", "Escarpment", "Precipice", "Abyss", "Chasm", "Gap",
        "Pass", "Col", "Saddle", "Notch", "Saddleback", "Ridgeback", "Spine", "Backbone", "Spur", "Butte",
        "Mesa", "Plateau", "Tableland", "Highland", "Upland", "Lowland", "Plain", "Prairie", "Steppe", "Savanna",
        "Grassland", "Meadow", "Pasture", "Field", "Clearing", "Glade", "Grove", "Woodland", "Forest", "Jungle",
        "Thicket", "Brush", "Scrub", "Heath", "Moor", "Bog", "Swamp", "Marsh", "Fen", "Wetland",
        "Lagoon", "Pond", "Lake", "Reservoir", "Basin", "Depression", "Hollow", "Dale", "Glen"
    };

    private static readonly string[] GeographicNames = 
    {
        "Miami", "Malibu", "Newport", "Hamptons", "Nantucket", "Martha", "Catalina", "Coronado", 
        "Sausalito", "Monterey", "Carmel", "Bodega", "Mendocino", "Capitola", "Tiburon", "Belvedere", 
        "Annapolis", "Camden", "Bar Harbor", "Kennebunkport", "Mystic", "Greenwich", "Sag Harbor", 
        "Shelter Island", "Block Island", "Hilton Head", "Kiawah", "Amelia", "Marco", "Sanibel",
        "Key West", "Palm Beach", "Fort Lauderdale", "Tampa", "St. Petersburg", "Clearwater", "Sarasota", "Naples",
        "Fort Myers", "Punta Gorda", "Venice", "Bradenton", "St. Augustine", "Daytona Beach", "Cocoa Beach", "Melbourne",
        "Vero Beach", "Stuart", "Jupiter", "Boca Raton", "Delray Beach", "Boynton Beach", "Lake Worth", "West Palm Beach",
        "Riviera Beach", "Palm Beach Gardens", "Juno Beach", "Tequesta", "Hobe Sound", "Port Salerno", "Palm City",
        "Jensen Beach", "Fort Pierce", "Sebastian", "Melbourne Beach", "Indialantic", "Satellite Beach",
        "Cape Canaveral", "Port Canaveral", "Cocoa", "Rockledge", "Palm Bay", "Malabar",
        "Grant", "Micco", "Roseland", "Fellsmere", "Gifford", "Indian River Shores",
        "South Beach", "North Beach", "Mid Beach", "South Pointe", "Fisher Island", "Key Biscayne", "Virginia Key", "Brickell",
        "Downtown", "Midtown", "Upper East Side", "Upper West Side", "Harlem", "Brooklyn", "Queens", "Bronx", "Staten Island",
        "Manhattan", "Long Island", "Fire Island", "Martha's Vineyard",
        "Cape Cod", "Provincetown", "Chatham", "Orleans", "Eastham", "Wellfleet", "Truro", "Brewster", "Dennis", "Yarmouth",
        "Barnstable", "Falmouth", "Mashpee", "Sandwich", "Bourne", "Wareham", "Marion", "Mattapoisett", "Fairhaven", "New Bedford",
        "Fall River", "Somerset", "Swansea", "Warren", "Bristol", "Barrington", "East Providence", "Providence", "Cranston",
        "Warwick", "East Greenwich", "West Greenwich", "North Kingstown", "South Kingstown", "Narragansett", "Westerly", "Charlestown",
        "Richmond", "Hopkinton", "Exeter"
    };

    private static readonly string[] States = 
    {
        "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "FL", "GA",
        "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD",
        "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ",
        "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC",
        "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY",
        "PR", "VI", "AS", "GU", "MP"  // Territories
    };

    private static readonly char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public static async Task GenerateAsync(SkipperContext dbContext, ILogger<VesselRepository> logger, int totalRecords = 5000, int batchSize = 500)
    {
        logger.LogInformation("Starting vessel generation: {TotalRecords} records in batches of {BatchSize}", totalRecords, batchSize);
        
        var repository = new VesselRepository(dbContext, logger);
        var random = new Random(42); // Fixed seed for reproducible results
        
        var usedRegistrations = new HashSet<string>();
        var usedNames = new HashSet<string>();
        
        var totalBatches = (int)Math.Ceiling((double)totalRecords / batchSize);
        
        for (int batch = 1; batch <= totalBatches; batch++)
        {
            var recordsInBatch = Math.Min(batchSize, totalRecords - (batch - 1) * batchSize);
            logger.LogInformation("Generating batch {Batch}/{TotalBatches}: {RecordsInBatch} records", batch, totalBatches, recordsInBatch);
            
            var vessels = new List<VesselEntity>();
            
            for (int i = 0; i < recordsInBatch; i++)
            {
                var vessel = GenerateVessel(random, batch, i + 1, usedRegistrations, usedNames);
                vessels.Add(vessel);
            }
            
            Task[] vesselTasks = vessels.Select(Task (vessel) => repository.AddAsync(vessel)).ToArray();
            Task.WaitAll(vesselTasks);
            
            logger.LogInformation("Batch {Batch} completed: {RecordsInserted} vessels inserted", batch, vessels.Count);
        }
        
        logger.LogInformation("Vessel generation completed: {TotalRecords} vessels generated from {StatesCount} states/territories", 
            totalRecords, States.Length);
    }

    private static VesselEntity GenerateVessel(Random random, int batch, int recordNum, HashSet<string> usedRegistrations, HashSet<string> usedNames)
    {
        var registrationNumber = GenerateUniqueRegistration(random, batch, recordNum, usedRegistrations);
        var name = GenerateUniqueName(random, usedNames);
        var length = GenerateLength(random);
        var beam = GenerateBeam(random);
        var vesselType = recordNum % 2 == 0 ? VesselType.Motorboat : VesselType.Sailboat;
        var timestamp = GenerateTimestamp(random);

        return new VesselEntity
        {
            RegistrationNumber = registrationNumber,
            Name = name,
            Length = length,
            Beam = beam,
            VesselType = vesselType,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
            IsDeleted = false
        };
    }

    private static string GenerateUniqueRegistration(Random random, int batch, int recordNum, HashSet<string> usedRegistrations)
    {
        string registration;
        int attempts = 0;
        
        do
        {
            registration = GenerateRegistration(random, batch, recordNum);
            attempts++;
            
            if (attempts > 100)
            {
                // Fallback to sequential approach if too many collisions
                registration = $"{States[random.Next(States.Length)]}{batch}{usedRegistrations.Count:D4}{Letters[random.Next(Letters.Length)]}{Letters[random.Next(Letters.Length)]}";
                break;
            }
        } while (usedRegistrations.Contains(registration));
        
        usedRegistrations.Add(registration);
        return registration;
    }

    private static string GenerateRegistration(Random random, int batch, int recordNum)
    {
        var state = States[random.Next(States.Length)];
        var letter1 = Letters[random.Next(Letters.Length)];
        var letter2 = Letters[random.Next(Letters.Length)];
        return $"{state}{batch}{recordNum:D3}{letter1}{letter2}";
    }

    private static string GenerateUniqueName(Random random, HashSet<string> usedNames)
    {
        string name;
        int attempts = 0;
        
        do
        {
            name = GenerateName(random);
            attempts++;
            
            if (attempts > 100)
            {
                // Fallback to appending number if too many collisions
                name = $"{name} {usedNames.Count + 1}";
                break;
            }
        } while (usedNames.Contains(name));
        
        usedNames.Add(name);
        return name;
    }

    private static string GenerateName(Random random)
    {
        var nameType = random.Next(3); // 0, 1, or 2
        
        return nameType switch
        {
            0 => $"{NamePrefixes[random.Next(NamePrefixes.Length)]} {NameSuffixes[random.Next(NameSuffixes.Length)]}",
            1 => $"{GeographicNames[random.Next(GeographicNames.Length)]} {NameSuffixes[random.Next(NameSuffixes.Length)]}",
            _ => $"{NamePrefixes[random.Next(NamePrefixes.Length)]} {GeographicNames[random.Next(GeographicNames.Length)]}"
        };
    }

    private static decimal GenerateLength(Random random)
    {
        return Math.Round((decimal)(random.NextDouble() * 32.0 + 18.0), 1);
    }

    private static decimal GenerateBeam(Random random)
    {
        return Math.Round((decimal)(random.NextDouble() * 9.0 + 6.0), 1);
    }

    private static DateTime GenerateTimestamp(Random random)
    {
        var startDate = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        var timeSpan = endDate - startDate;
        var randomTimeSpan = TimeSpan.FromTicks((long)(random.NextDouble() * timeSpan.Ticks));
        return startDate + randomTimeSpan;
    }
} 