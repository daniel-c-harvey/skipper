using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class SlipClassificationRepository : Repository<SkipperContext, SlipClassificationEntity>
{
    public SlipClassificationRepository(SkipperContext context, ILogger<SlipClassificationRepository> logger) : base(context, logger) { }
    
    protected override void UpdateModel(SlipClassificationEntity target, SlipClassificationEntity source)
    {
        base.UpdateModel(target, source);
        target.BasePrice = source.BasePrice;
        target.Description = source.Description;
        target.MaxLength = source.MaxLength;
        target.MaxBeam = source.MaxBeam;
        target.Name = source.Name;
    }

    public async Task<SlipClassificationEntity?> GetByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
            
        var results = await FindAsync(sc => sc.Name == name);
        return results.FirstOrDefault();
    }

    public async Task<IEnumerable<SlipClassificationEntity>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return Enumerable.Empty<SlipClassificationEntity>();
            
        return await FindAsync(sc => sc.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<PagedResult<SlipClassificationEntity>> SearchByNamePagedAsync(string searchTerm, PagingParameters<SlipClassificationEntity> pagingParameters)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return new PagedResult<SlipClassificationEntity>(Enumerable.Empty<SlipClassificationEntity>(), 0, pagingParameters.Page, pagingParameters.PageSize);
            
        return await GetPagedAsync(sc => sc.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase), pagingParameters);
    }

    public async Task<IEnumerable<SlipClassificationEntity>> GetByPriceRangeAsync(int minPrice, int maxPrice)
    {
        return await FindAsync(sc => sc.BasePrice >= minPrice && sc.BasePrice <= maxPrice);
    }

    public async Task<PagedResult<SlipClassificationEntity>> GetByPriceRangePagedAsync(int minPrice, int maxPrice, PagingParameters<SlipClassificationEntity> pagingParameters)
    {
        return await GetPagedAsync(sc => sc.BasePrice >= minPrice && sc.BasePrice <= maxPrice, pagingParameters);
    }

    public async Task<IEnumerable<SlipClassificationEntity>> GetCompatibleForVesselAsync(decimal vesselLength, decimal vesselBeam)
    {
        return await FindAsync(sc => sc.MaxLength >= vesselLength && sc.MaxBeam >= vesselBeam);
    }

    public async Task<PagedResult<SlipClassificationEntity>> GetCompatibleForVesselPagedAsync(decimal vesselLength, decimal vesselBeam, PagingParameters<SlipClassificationEntity> pagingParameters)
    {
        return await GetPagedAsync(sc => sc.MaxLength >= vesselLength && sc.MaxBeam >= vesselBeam, pagingParameters);
    }
} 