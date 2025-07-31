using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperModels.Entities;
using SkipperModels;
using Models.Shared.Common;
using NetBlocks.Models;

namespace SkipperData.Data.Repositories;

public interface ISlipReservationOrderRepository : IOrderRepository<SlipReservationOrderEntity>
{
    // Slip reservation specific methods
    Task<IEnumerable<SlipReservationOrderEntity>> GetSlipReservationsAsync();
    Task<IEnumerable<SlipReservationOrderEntity>> GetReservationsBySlipAsync(long slipId);
    Task<IEnumerable<SlipReservationOrderEntity>> GetReservationsByVesselAsync(long vesselId);
    Task<IEnumerable<SlipReservationOrderEntity>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<SlipReservationOrderEntity>> GetActiveReservationsAsync();
    Task<IEnumerable<SlipReservationOrderEntity>> GetOverlappingReservationsAsync(long slipId, DateTime startDate, DateTime endDate, long? excludeOrderId = null);
    Task<bool> IsSlipAvailableAsync(long slipId, DateTime startDate, DateTime endDate, long? excludeOrderId = null);
    Task<decimal> GetRevenueBySlipAsync(long slipId, DateTime? startDate = null, DateTime? endDate = null);
    Task<PagedResult<SlipReservationOrderEntity>> GetSlipReservationsPagedAsync(PagingParameters<SlipReservationOrderEntity> pagingParameters);
}

public class SlipReservationOrderRepository : OrderRepository<SlipReservationOrderEntity, VesselOwnerCustomerEntity>, ISlipReservationOrderRepository
{
    public SlipReservationOrderRepository(SkipperContext context, ILogger<SlipReservationOrderRepository> logger) 
        : base(context, logger)
    {
    }

    #region Slip Reservation Specific Methods

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetSlipReservationsAsync()
    {
        return await Context.Orders
            .OfType<SlipReservationOrderEntity>()
            .Include(o => o.Customer)
            .Include(o => o.SlipEntity)
            .Include(o => o.VesselEntity)
            .Where(o => !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetReservationsBySlipAsync(long slipId)
    {
        return await Context.Orders
            .OfType<SlipReservationOrderEntity>()
            .Include(o => o.Customer)
            .Include(o => o.SlipEntity)
            .Include(o => o.VesselEntity)
            .Where(o => o.SlipId == slipId && !o.IsDeleted)
            .OrderBy(o => o.StartDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetReservationsByVesselAsync(long vesselId)
    {
        return await Context.Orders
            .OfType<SlipReservationOrderEntity>()
            .Include(o => o.Customer)
            .Include(o => o.SlipEntity)
            .Include(o => o.VesselEntity)
            .Where(o => o.VesselId == vesselId && !o.IsDeleted)
            .OrderByDescending(o => o.StartDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Context.Orders
            .OfType<SlipReservationOrderEntity>()
            .Include(o => o.Customer)
            .Include(o => o.SlipEntity)
            .Include(o => o.VesselEntity)
            .Where(o => o.StartDate <= endDate && o.EndDate >= startDate && !o.IsDeleted)
            .OrderBy(o => o.StartDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetActiveReservationsAsync()
    {
        var currentDate = DateTime.UtcNow.Date;
        
        return await Context.Orders
            .OfType<SlipReservationOrderEntity>()
            .Include(o => o.Customer)
            .Include(o => o.SlipEntity)
            .Include(o => o.VesselEntity)
            .Where(o => o.StartDate <= currentDate && 
                       o.EndDate >= currentDate && 
                       o.RentalStatus == RentalStatus.Active &&
                       !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<SlipReservationOrderEntity>> GetOverlappingReservationsAsync(
        long slipId, DateTime startDate, DateTime endDate, long? excludeOrderId = null)
    {
        var query = Context.Orders
            .OfType<SlipReservationOrderEntity>()
            .Include(o => o.Customer)
            .Include(o => o.SlipEntity)
            .Include(o => o.VesselEntity)
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
        var query = Context.Orders
            .OfType<SlipReservationOrderEntity>()
            .Where(o => o.SlipId == slipId && 
                       o.Status == OrderStatus.Completed && 
                       !o.IsDeleted);

        if (startDate.HasValue)
            query = query.Where(o => o.StartDate >= startDate.Value);
            
        if (endDate.HasValue)
            query = query.Where(o => o.EndDate <= endDate.Value);

        return await query.SumAsync(o => o.TotalAmount) / 100m; // Convert from cents
    }

    public virtual async Task<PagedResult<SlipReservationOrderEntity>> GetSlipReservationsPagedAsync(PagingParameters<SlipReservationOrderEntity> pagingParameters)
    {
        var query = Context.Orders
            .OfType<SlipReservationOrderEntity>()
            .Include(o => o.Customer)
            .Include(o => o.SlipEntity)
            .Include(o => o.VesselEntity)
            .Where(o => !o.IsDeleted);
            
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    #endregion
}