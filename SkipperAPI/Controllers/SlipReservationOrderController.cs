using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkipperData.Data.Repositories;
using SkipperData.Managers;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SlipReservationOrderController : OrderController<SlipReservationOrderEntity, SlipReservationOrderModel, VesselOwnerCustomerEntity, VesselOwnerCustomerModel, SlipReservationOrderManager, SlipReservationOrderRepository, SlipReservationOrderConverter>
{
    private readonly SlipReservationOrderManager _manager;

    public SlipReservationOrderController(SlipReservationOrderManager manager) : base(manager)
    {
        _manager = manager;
        AddSortExpression(nameof(SlipReservationOrderEntity.SlipEntity.SlipNumber), e => e.SlipEntity.SlipNumber);
        AddSortExpression(nameof(SlipReservationOrderEntity.VesselEntity.RegistrationNumber), e => e.VesselEntity.RegistrationNumber);
        AddSortExpression(nameof(SlipReservationOrderEntity.VesselEntity.Name), e => e.VesselEntity.Name);
        AddSortExpression(nameof(SlipReservationOrderEntity.StartDate), e => e.StartDate);
        AddSortExpression(nameof(SlipReservationOrderEntity.EndDate), e => e.EndDate);
        AddSortExpression(nameof(SlipReservationOrderEntity.PriceRate), e => e.PriceRate);
        AddSortExpression(nameof(SlipReservationOrderEntity.PriceUnit), e => e.PriceUnit);
        AddSortExpression(nameof(SlipReservationOrderEntity.Status), e => e.Status);
    }

    // Slip-specific endpoints
    [HttpGet("slip-reservations")]
    public virtual async Task<ActionResult<IEnumerable<SlipReservationOrderModel>>> GetSlipReservations()
    {
        var results = await _manager.GetSlipReservationsAsync();
        return Ok(results);
    }

    [HttpGet("by-slip/{slipId}")]
    public virtual async Task<ActionResult<IEnumerable<SlipReservationOrderModel>>> GetBySlip(long slipId)
    {
        var results = await _manager.GetReservationsBySlipAsync(slipId);
        return Ok(results);
    }

    [HttpGet("by-vessel/{vesselId}")]
    public virtual async Task<ActionResult<IEnumerable<SlipReservationOrderModel>>> GetByVessel(long vesselId)
    {
        var results = await _manager.GetReservationsByVesselAsync(vesselId);
        return Ok(results);
    }

    [HttpGet("by-date-range")]
    public virtual async Task<ActionResult<IEnumerable<SlipReservationOrderModel>>> GetByDateRange(DateTime startDate, DateTime endDate)
    {
        var results = await _manager.GetReservationsByDateRangeAsync(startDate, endDate);
        return Ok(results);
    }

    [HttpGet("active-reservations")]
    public virtual async Task<ActionResult<IEnumerable<SlipReservationOrderModel>>> GetActiveReservations()
    {
        var results = await _manager.GetActiveReservationsAsync();
        return Ok(results);
    }

    [HttpGet("slip-availability/{slipId}")]
    public virtual async Task<ActionResult<bool>> CheckSlipAvailability(long slipId, DateTime startDate, DateTime endDate, long? excludeOrderId = null)
    {
        var isAvailable = await _manager.IsSlipAvailableAsync(slipId, startDate, endDate, excludeOrderId);
        return Ok(isAvailable);
    }

    [HttpGet("revenue/by-slip/{slipId}")]
    public virtual async Task<ActionResult<decimal>> GetSlipRevenue(long slipId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var revenue = await _manager.GetRevenueBySlipAsync(slipId, startDate, endDate);
        return Ok(revenue);
    }

    // [HttpGet("overlapping-reservations/{slipId}")]
    // public virtual async Task<ActionResult<IEnumerable<SlipReservationOrderModel>>> GetOverlappingReservations(long slipId, DateTime startDate, DateTime endDate, long? excludeOrderId = null)
    // {
    //     var results = await _manager.GetOverlappingReservationsAsync(slipId, startDate, endDate, excludeOrderId);
    //     return Ok(results);
    // }
    
    protected override Expression<Func<SlipReservationOrderEntity, bool>> BuildSearchPredicate(string? search)
        => string.IsNullOrEmpty(search)
            ? s => true
            : s => EF.Functions.Like(s.SlipEntity.SlipNumber, $"%{search}%") ||
                   EF.Functions.Like(s.VesselEntity.RegistrationNumber, $"%{search}%") ||
                   EF.Functions.Like(s.VesselEntity.Name, $"%{search}%") ||
                   EF.Functions.Like(s.Status.ToString(), $"%{search}%");
}