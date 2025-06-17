using Microsoft.Extensions.Logging;
using SkipperModels;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class SlipRepository : Repository<Slip>
{
    public SlipRepository(SkipperContext context, ILogger<SlipRepository> logger) : base(context, logger) { }

    public async Task<IEnumerable<Slip>> GetByStatusAsync(SlipStatus status)
    {
        return await FindAsync(s => s.Status == status);
    }

    public async Task<PagedResult<Slip>> GetByStatusPagedAsync(SlipStatus status, PagingParameters<Slip> pagingParameters)
    {
        return await GetPagedAsync(s => s.Status == status, pagingParameters);
    }

    public async Task<IEnumerable<Slip>> GetAvailableAsync()
    {
        return await GetByStatusAsync(SlipStatus.Available);
    }

    public async Task<PagedResult<Slip>> GetAvailablePagedAsync(PagingParameters<Slip> pagingParameters)
    {
        return await GetByStatusPagedAsync(SlipStatus.Available, pagingParameters);
    }

    public async Task<Slip?> GetBySlipNumberAsync(string slipNumber)
    {
        var results = await FindAsync(s => s.SlipNumber == slipNumber);
        return results.FirstOrDefault();
    }
} 