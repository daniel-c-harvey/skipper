using Models.Shared.Common;
using NetBlocks.Models;
using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class SlipReservationOrderManager : OrderManager<SlipReservationOrderEntity, SlipReservationOrderModel, VesselOwnerCustomerEntity, VesselOwnerCustomerModel, SlipReservationOrderRepository, SlipReservationOrderConverter>
{
    private readonly SlipReservationOrderRepository _repository;
    private readonly VesselOwnerCustomerManager _customerManager;

    public SlipReservationOrderManager(SlipReservationOrderRepository repository, VesselOwnerCustomerManager customerManager) : base(repository)
    {
        _repository = repository;
        _customerManager = customerManager;
    }

    public override async Task<ResultContainer<SlipReservationOrderModel>> Add(SlipReservationOrderModel entity)
    {
        if (await _customerManager.Exists(entity.Customer) is { Value: false })
        {
            var newCustomerResult = await _customerManager.Add(entity.Customer);
            if (newCustomerResult is { Success: true, Value: VesselOwnerCustomerModel customer})
            {
                entity.Customer = customer;
            }
        }
        return await base.Add(entity);
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

    public virtual async Task<PagedResult<SlipReservationOrderModel>> GetSlipReservationsPagedAsync(PagingParameters<SlipReservationOrderEntity> pagingParameters)
    {
        var result = await _repository.GetSlipReservationsPagedAsync(pagingParameters);
        var models = result.Items.Select(SlipReservationOrderConverter.Convert);
        return new PagedResult<SlipReservationOrderModel>(models, result.TotalCount, result.Page, result.PageSize);
    }
}