using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class SlipReservationRepository : Repository<SkipperContext, SlipReservationEntity>
{
    public SlipReservationRepository(SkipperContext context, ILogger<SlipReservationRepository> logger) : base(context, logger) { }
    
    protected override void UpdateModel(SlipReservationEntity target, SlipReservationEntity source)
    {
        base.UpdateModel(target, source);
        target.EndDate = source.EndDate;
        target.StartDate = source.StartDate;
        target.Status = source.Status;
        target.SlipId = source.SlipId;
        target.VesselId = source.VesselId;
        target.PriceUnit = source.PriceUnit;
        target.PriceRate = source.PriceRate;
    }

    public async Task<IEnumerable<SlipReservationEntity>> GetByStatusAsync(RentalStatus status)
    {
        return await FindAsync(ra => ra.Status == status);
    }

    public async Task<PagedResult<SlipReservationEntity>> GetByStatusPagedAsync(RentalStatus status, PagingParameters<SlipReservationEntity> pagingParameters)
    {
        return await GetPagedAsync(ra => ra.Status == status, pagingParameters);
    }

    public async Task<IEnumerable<SlipReservationEntity>> GetActiveAsync()
    {
        return await GetByStatusAsync(RentalStatus.Active);
    }

    public async Task<PagedResult<SlipReservationEntity>> GetActivePagedAsync(PagingParameters<SlipReservationEntity> pagingParameters)
    {
        return await GetByStatusPagedAsync(RentalStatus.Active, pagingParameters);
    }

    public async Task<IEnumerable<SlipReservationEntity>> GetBySlipIdAsync(long slipId)
    {
        return await FindAsync(ra => ra.SlipId == slipId);
    }

    public async Task<PagedResult<SlipReservationEntity>> GetBySlipIdPagedAsync(long slipId, PagingParameters<SlipReservationEntity> pagingParameters)
    {
        return await GetPagedAsync(ra => ra.SlipId == slipId, pagingParameters);
    }

    public async Task<IEnumerable<SlipReservationEntity>> GetByVesselIdAsync(long vesselId)
    {
        return await FindAsync(ra => ra.VesselId == vesselId);
    }

    public async Task<PagedResult<SlipReservationEntity>> GetByVesselIdPagedAsync(long vesselId, PagingParameters<SlipReservationEntity> pagingParameters)
    {
        return await GetPagedAsync(ra => ra.VesselId == vesselId, pagingParameters);
    }

    public async Task<bool> HasConflictAsync(long slipId, DateTime startDate, DateTime endDate)
    {
        var conflicts = await FindAsync(ra => 
            ra.SlipId == slipId && 
            ra.Status == RentalStatus.Active &&
            ra.StartDate < endDate && 
            ra.EndDate > startDate);
        
        return conflicts.Any();
    }
} 