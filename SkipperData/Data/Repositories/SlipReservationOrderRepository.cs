using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperModels.Entities;
using SkipperModels;
using Models.Shared.Common;
using NetBlocks.Models;

namespace SkipperData.Data.Repositories;

public class SlipReservationOrderRepository : OrderRepository<SlipReservationOrderEntity, VesselOwnerCustomerEntity>
{
    public SlipReservationOrderRepository(SkipperContext context, ILogger<SlipReservationOrderRepository> logger) 
        : base(
            context,
            logger, 
            q => q
                .Include(o => o.VesselEntity)
                .Include(o => o.SlipEntity))
    {
    }
    
    protected override void UpdateEntity(SlipReservationOrderEntity target, SlipReservationOrderEntity source)
    {
        base.UpdateEntity(target, source);
        target.EndDate = source.EndDate;
        target.StartDate = source.StartDate;
        target.Status = source.Status;
        target.SlipId = source.SlipId;
        target.VesselId = source.VesselId;
        target.PriceUnit = source.PriceUnit;
        target.PriceRate = source.PriceRate;
    }

    #region Slip Reservation Specific Methods

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetSlipReservationsAsync()
    {
        return await Query
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetReservationsBySlipAsync(long slipId)
    {
        return await Query
            .Where(o => o.SlipId == slipId)
            .OrderBy(o => o.StartDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetReservationsByVesselAsync(long vesselId)
    {
        return await Query
            .Where(o => o.VesselId == vesselId)
            .OrderByDescending(o => o.StartDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Query
            .Where(o => o.StartDate <= endDate && o.EndDate >= startDate)
            .OrderBy(o => o.StartDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetActiveReservationsAsync()
    {
        var currentDate = DateTime.UtcNow.Date;
        
        return await Query
            .Where(o => o.StartDate <= currentDate && 
                       o.EndDate >= currentDate && 
                       o.RentalStatus == RentalStatus.Active &&
                       !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetOverlappingReservationsAsync(
        long slipId, DateTime startDate, DateTime endDate, long? excludeOrderId = null)
    {
        var query = Query
            .Where(o => o.SlipId == slipId && 
                       o.StartDate < endDate && 
                       o.EndDate > startDate &&
                       o.RentalStatus != RentalStatus.Cancelled &&
                       !o.IsDeleted);

        if (excludeOrderId.HasValue)
            query = query.Where(o => o.Id != excludeOrderId.Value);

        return await query.ToListAsync();
    }

    public virtual async Task<bool> IsSlipAvailableAsync(long slipId, DateTime startDate, DateTime endDate, long? excludeOrderId = null)
    {
        var overlapping = await GetOverlappingReservationsAsync(slipId, startDate, endDate, excludeOrderId);
        return !overlapping.Any();
    }

    public virtual async Task<decimal> GetRevenueBySlipAsync(long slipId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = Query
            .Where(o => o.SlipId == slipId && 
                       o.Status == OrderStatus.Completed);

        if (startDate.HasValue)
            query = query.Where(o => o.StartDate >= startDate.Value);
            
        if (endDate.HasValue)
            query = query.Where(o => o.EndDate <= endDate.Value);

        return await query.SumAsync(o => o.TotalAmount) / 100m; // Convert from cents
    }

    public virtual async Task<PagedResult<SlipReservationOrderEntity>> GetSlipReservationsPagedAsync(PagingParameters<SlipReservationOrderEntity> pagingParameters)
    {
        return await ExecutePagedQueryAsync(Query, pagingParameters);
    }

    #endregion
}