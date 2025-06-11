using SkipperModels.Common;
using SkipperModels.Entities;

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

    public async Task<PagedResult<SlipClassification>> SearchByNamePagedAsync(string searchTerm, PagingParameters<SlipClassification> pagingParameters)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return new PagedResult<SlipClassification>(Enumerable.Empty<SlipClassification>(), 0, pagingParameters.Page, pagingParameters.PageSize);
            
        return await GetPagedAsync(sc => sc.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase), pagingParameters);
    }

    public async Task<IEnumerable<SlipClassification>> GetByPriceRangeAsync(int minPrice, int maxPrice)
    {
        return await FindAsync(sc => sc.BasePrice >= minPrice && sc.BasePrice <= maxPrice);
    }

    public async Task<PagedResult<SlipClassification>> GetByPriceRangePagedAsync(int minPrice, int maxPrice, PagingParameters<SlipClassification> pagingParameters)
    {
        return await GetPagedAsync(sc => sc.BasePrice >= minPrice && sc.BasePrice <= maxPrice, pagingParameters);
    }

    public async Task<IEnumerable<SlipClassification>> GetCompatibleForVesselAsync(decimal vesselLength, decimal vesselBeam)
    {
        return await FindAsync(sc => sc.MaxLength >= vesselLength && sc.MaxBeam >= vesselBeam);
    }

    public async Task<PagedResult<SlipClassification>> GetCompatibleForVesselPagedAsync(decimal vesselLength, decimal vesselBeam, PagingParameters<SlipClassification> pagingParameters)
    {
        return await GetPagedAsync(sc => sc.MaxLength >= vesselLength && sc.MaxBeam >= vesselBeam, pagingParameters);
    }
} 