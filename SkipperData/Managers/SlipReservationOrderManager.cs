using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class SlipReservationOrderManager : OrderManager<SlipReservationOrderEntity, SlipReservationOrderModel, SlipReservationOrderRepository, SlipReservationOrderConverter>
{
    private readonly SlipReservationOrderRepository _repository;

    public SlipReservationOrderManager(SlipReservationOrderRepository repository) : base(repository)
    {
        _repository = repository;
    }

    // Slip reservation specific business logic methods
    public virtual async Task<IEnumerable<SlipReservationOrderModel>> GetSlipReservationsAsync()
    {
        var entities = await _repository.GetSlipReservationsAsync();
        return entities.Select(SlipReservationOrderConverter.Convert);
    }

    public virtual async Task<IEnumerable<SlipReservationOrderModel>> GetReservationsBySlipAsync(long slipId)
    {
        var entities = await _repository.GetReservationsBySlipAsync(slipId);
        return entities.Select(SlipReservationOrderConverter.Convert);
    }

    public virtual async Task<IEnumerable<SlipReservationOrderModel>> GetReservationsByVesselAsync(long vesselId)
    {
        var entities = await _repository.GetReservationsByVesselAsync(vesselId);
        return entities.Select(SlipReservationOrderConverter.Convert);
    }

    public virtual async Task<IEnumerable<SlipReservationOrderModel>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var entities = await _repository.GetReservationsByDateRangeAsync(startDate, endDate);
        return entities.Select(SlipReservationOrderConverter.Convert);
    }

    public virtual async Task<IEnumerable<SlipReservationOrderModel>> GetActiveReservationsAsync()
    {
        var entities = await _repository.GetActiveReservationsAsync();
        return entities.Select(SlipReservationOrderConverter.Convert);
    }

    public virtual async Task<bool> IsSlipAvailableAsync(long slipId, DateTime startDate, DateTime endDate, long? excludeOrderId = null)
    {
        return await _repository.IsSlipAvailableAsync(slipId, startDate, endDate, excludeOrderId);
    }

    public virtual async Task<decimal> GetRevenueBySlipAsync(long slipId, DateTime? startDate = null, DateTime? endDate = null)
    {
        return await _repository.GetRevenueBySlipAsync(slipId, startDate, endDate);
    }

    public virtual async Task<IEnumerable<SlipReservationOrderModel>> GetOverlappingReservationsAsync(long slipId, DateTime startDate, DateTime endDate, long? excludeOrderId = null)
    {
        var entities = await _repository.GetOverlappingReservationsAsync(slipId, startDate, endDate, excludeOrderId);
        return entities.Select(SlipReservationOrderConverter.Convert);
    }
}