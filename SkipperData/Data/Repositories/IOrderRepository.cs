using System.Linq.Expressions;
using Data.Shared.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels;
using Models.Shared.Common;
using NetBlocks.Models;

namespace SkipperData.Data.Repositories;

// Generic interface for type-specific order operations (works with specific TOrderEntity only)
public interface IOrderRepository<TOrderEntity, TCustomerEntity> : IRepository<TOrderEntity>
    where TOrderEntity : OrderEntity
    where TCustomerEntity : CustomerEntity
{
    // Type-specific order methods (return TOrderEntity only)
    Task<IEnumerable<TOrderEntity>> GetOrdersByCustomerAsync(long customerId);
    Task<IEnumerable<TOrderEntity>> GetOrdersByStatusAsync(OrderStatus status);
    Task<IEnumerable<TOrderEntity>> GetActiveOrdersAsync();
    Task<IEnumerable<TOrderEntity>> GetOverdueOrdersAsync();
}