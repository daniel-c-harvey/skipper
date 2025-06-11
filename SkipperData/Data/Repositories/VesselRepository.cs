using SkipperModels;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class VesselRepository : Repository<Vessel>
{
    public VesselRepository(SkipperContext context) : base(context) { }

    public async Task<Vessel?> GetByRegistrationNumberAsync(string registrationNumber)
    {
        var results = await FindAsync(v => v.RegistrationNumber == registrationNumber);
        return results.FirstOrDefault();
    }

    public async Task<IEnumerable<Vessel>> GetByTypeAsync(VesselType type)
    {
        return await FindAsync(v => v.VesselType == type);
    }

    public async Task<PagedResult<Vessel>> GetByTypePagedAsync(VesselType type, PagingParameters<Vessel> pagingParameters)
    {
        return await GetPagedAsync(v => v.VesselType == type, pagingParameters);
    }

    public async Task<IEnumerable<Vessel>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return Enumerable.Empty<Vessel>();
            
        return await FindAsync(v => v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<PagedResult<Vessel>> SearchByNamePagedAsync(string searchTerm, PagingParameters<Vessel> pagingParameters)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return new PagedResult<Vessel>(Enumerable.Empty<Vessel>(), 0, pagingParameters.Page, pagingParameters.PageSize);
            
        return await GetPagedAsync(v => v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase), pagingParameters);
    }
} 