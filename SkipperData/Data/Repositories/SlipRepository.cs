using Microsoft.Extensions.Logging;
using SkipperModels;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class SlipRepository : Repository<SlipEntity, SlipModel>
{
    public SlipRepository(SkipperContext context, ILogger<SlipRepository> logger) : base(context, logger) { }

    public async Task<IEnumerable<SlipEntity>> GetByStatusAsync(SlipStatus status)
    {
        return await FindAsync(s => s.Status == status);
    }

    public async Task<PagedResult<SlipEntity>> GetByStatusPagedAsync(SlipStatus status, PagingParameters<SlipEntity> pagingParameters)
    {
        return await GetPagedAsync(s => s.Status == status, pagingParameters);
    }

    public async Task<IEnumerable<SlipEntity>> GetAvailableAsync()
    {
        return await GetByStatusAsync(SlipStatus.Available);
    }

    public async Task<PagedResult<SlipEntity>> GetAvailablePagedAsync(PagingParameters<SlipEntity> pagingParameters)
    {
        return await GetByStatusPagedAsync(SlipStatus.Available, pagingParameters);
    }

    public async Task<SlipEntity?> GetBySlipNumberAsync(string slipNumber)
    {
        var results = await FindAsync(s => s.SlipNumber == slipNumber);
        return results.FirstOrDefault();
    }
} 