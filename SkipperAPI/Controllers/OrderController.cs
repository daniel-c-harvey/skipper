using API.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using Models.Shared.Converters;
using SkipperData.Data.Repositories;
using SkipperData.Managers;
using SkipperModels;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers;

public class OrderController<TOrderEntity, TOrderModel, TCustomerEntity, TCustomerModel, TOrderManager, TRepository, TConverter> 
    : ModelController<TOrderEntity, TOrderModel, TOrderManager>
    where TCustomerEntity : CustomerEntity, new()
    where TCustomerModel : CustomerModel, new()
    where TOrderEntity : OrderEntity, new()
    where TOrderModel : OrderModel<TCustomerModel>, new()
    where TOrderManager : OrderManager<TOrderEntity, TOrderModel, TCustomerEntity, TCustomerModel, TRepository, TConverter>
    where TRepository : IOrderRepository<TOrderEntity, TCustomerEntity>
    where TConverter : IEntityToModelConverter<TOrderEntity, TOrderModel>
{
    protected readonly TOrderManager OrderManager;

    public OrderController(TOrderManager manager) : base(manager)
    {
        OrderManager = manager;
        AddSortExpression(nameof(OrderEntity.Customer.Name), entity => entity.Customer.Name);
    }

    // Type-specific endpoints (only work with TOrderEntity)
    [HttpGet("by-customer/{customerId}")]
    public virtual async Task<ActionResult<IEnumerable<TOrderModel>>> GetByCustomer(long customerId)
    {
        var results = await OrderManager.GetOrdersByCustomerAsync(customerId);
        return Ok(results);
    }

    [HttpGet("by-status/{status}")]
    public virtual async Task<ActionResult<IEnumerable<TOrderModel>>> GetByStatus(OrderStatus status)
    {
        var results = await OrderManager.GetOrdersByStatusAsync(status);
        return Ok(results);
    }

    [HttpGet("active")]
    public virtual async Task<ActionResult<IEnumerable<TOrderModel>>> GetActiveOrders()
    {
        var results = await OrderManager.GetActiveOrdersAsync();
        return Ok(results);
    }

    [HttpGet("overdue")]
    public virtual async Task<ActionResult<IEnumerable<TOrderModel>>> GetOverdueOrders()
    {
        var results = await OrderManager.GetOverdueOrdersAsync();
        return Ok(results);
    }
}