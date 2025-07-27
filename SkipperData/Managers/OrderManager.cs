using Data.Shared.Managers;
using Models.Shared.Converters;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class OrderManager<TOrderEntity, TOrderModel, TRepository, TConverter> 
    : Manager<TOrderEntity, TOrderModel, TRepository, TConverter>
    where TOrderEntity : OrderEntity, new()
    where TOrderModel : OrderModel, new()
    where TRepository : IOrderRepository<TOrderEntity>
    where TConverter : IEntityToModelConverter<TOrderEntity, TOrderModel>
{
    protected OrderManager(TRepository repository) : base(repository)
    {
    }

    // Type-specific business logic methods (only methods available in IOrderRepository<TOrderEntity>)
    public virtual async Task<IEnumerable<TOrderModel>> GetOrdersByCustomerAsync(long customerId)
    {
        var entities = await Repository.GetOrdersByCustomerAsync(customerId);
        return entities.Select(TConverter.Convert);
    }

    public virtual async Task<IEnumerable<TOrderModel>> GetOrdersByStatusAsync(OrderStatus status)
    {
        var entities = await Repository.GetOrdersByStatusAsync(status);
        return entities.Select(TConverter.Convert);
    }

    public virtual async Task<IEnumerable<TOrderModel>> GetActiveOrdersAsync()
    {
        var entities = await Repository.GetActiveOrdersAsync();
        return entities.Select(TConverter.Convert);
    }

    public virtual async Task<IEnumerable<TOrderModel>> GetOverdueOrdersAsync()
    {
        var entities = await Repository.GetOverdueOrdersAsync();
        return entities.Select(TConverter.Convert);
    }
}