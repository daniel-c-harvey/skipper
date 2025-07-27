using System.Linq.Expressions;
using Data.Shared.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels;
using Models.Shared.Common;
using NetBlocks.Models;

namespace SkipperData.Data.Repositories;

// Base interface for cross-type order operations (works with mixed OrderEntity types)
public interface IOrderRepository : IRepository<OrderEntity>
{
    // Cross-type order methods that return OrderEntity (base type)
    Task<OrderEntity?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<OrderEntity>> GetOrdersByCustomerAsync(long customerId);
    Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync(OrderStatus status);
    Task<IEnumerable<OrderEntity>> GetOrdersByTypeAsync(OrderType orderType);
    Task<IEnumerable<OrderEntity>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    // Multi-type queries
    Task<IEnumerable<OrderEntity>> GetOrdersByTypesAsync(params OrderType[] orderTypes);
    Task<PagedResult<OrderEntity>> GetOrdersPagedAsync(
        Expression<Func<OrderEntity, bool>>? predicate = null,
        PagingParameters<OrderEntity>? pagingParameters = null);
        
    // Business logic methods
    Task<IEnumerable<OrderEntity>> GetActiveOrdersAsync();
    Task<IEnumerable<OrderEntity>> GetOverdueOrdersAsync();
    Task<decimal> GetTotalRevenueByTypeAsync(OrderType orderType, DateTime? startDate = null, DateTime? endDate = null);
}

// Generic interface for type-specific order operations (works with specific TOrderEntity only)
public interface IOrderRepository<TOrderEntity> : IRepository<TOrderEntity>
    where TOrderEntity : OrderEntity
{
    // Type-specific business methods that return TOrderEntity only
    // NO cross-type methods here - those belong in IOrderRepository
    Task<IEnumerable<TOrderEntity>> GetOrdersByCustomerAsync(long customerId);
    Task<IEnumerable<TOrderEntity>> GetOrdersByStatusAsync(OrderStatus status);
    Task<IEnumerable<TOrderEntity>> GetActiveOrdersAsync();
    Task<IEnumerable<TOrderEntity>> GetOverdueOrdersAsync();
}