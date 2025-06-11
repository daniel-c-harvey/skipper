using Skipper.Domain.Entities;

namespace Skipper.Data.Repositories;

public class SlipClassificationRepository : Repository<SlipClassification>
{
    public SlipClassificationRepository(SkipperContext context) : base(context) { }

    public async Task<SlipClassification?> GetByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
            
        var results = await FindAsync(sc => sc.Name == name);
        return results.FirstOrDefault();
    }

    public async Task<IEnumerable<SlipClassification>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return Enumerable.Empty<SlipClassification>();
            
         return await FindAsync(sc => sc.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<SlipClassification>> GetByPriceRangeAsync(int minPrice, int maxPrice)
    {
        return await FindAsync(sc => sc.BasePrice >= minPrice && sc.BasePrice <= maxPrice);
    }

    public async Task<IEnumerable<SlipClassification>> GetCompatibleForVesselAsync(decimal vesselLength, decimal vesselBeam)
    {
        return await FindAsync(sc => sc.MaxLength >= vesselLength && sc.MaxBeam >= vesselBeam);
    }
} 