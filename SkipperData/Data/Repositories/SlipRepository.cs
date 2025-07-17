using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class SlipRepository : Repository<SkipperContext, SlipEntity>
{
    public SlipRepository(SkipperContext context, ILogger<SlipRepository> logger) : base(context, logger)
    {
    }

    protected override void UpdateModel(SlipEntity target, SlipEntity source)
    {
        base.UpdateModel(target, source);
        target.LocationCode = source.LocationCode;
        target.Status = source.Status;
        target.SlipNumber = source.SlipNumber;
        target.SlipClassificationId = source.SlipClassificationId;
    }

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