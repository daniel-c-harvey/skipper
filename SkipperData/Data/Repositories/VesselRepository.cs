using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class VesselRepository : Repository<SkipperContext, VesselEntity>
{
    public VesselRepository(SkipperContext context, ILogger<VesselRepository> logger) : base(context, logger) { }

    public async Task<VesselEntity?> GetByRegistrationNumberAsync(string registrationNumber)
    {
        var results = await FindAsync(v => v.RegistrationNumber == registrationNumber);
        return results.FirstOrDefault();
    }

    public async Task<IEnumerable<VesselEntity>> GetByTypeAsync(VesselType type)
    {
        return await FindAsync(v => v.VesselType == type);
    }

    public async Task<PagedResult<VesselEntity>> GetByTypePagedAsync(VesselType type, PagingParameters<VesselEntity> pagingParameters)
    {
        return await GetPagedAsync(v => v.VesselType == type, pagingParameters);
    }

    public async Task<IEnumerable<VesselEntity>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return Enumerable.Empty<VesselEntity>();
            
        return await FindAsync(v => v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<PagedResult<VesselEntity>> SearchByNamePagedAsync(string searchTerm, PagingParameters<VesselEntity> pagingParameters)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return new PagedResult<VesselEntity>(Enumerable.Empty<VesselEntity>(), 0, pagingParameters.Page, pagingParameters.PageSize);
            
        return await GetPagedAsync(v => v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase), pagingParameters);
    }
} 