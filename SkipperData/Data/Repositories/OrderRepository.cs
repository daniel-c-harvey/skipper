using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Shared.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels;
using System.Linq.Expressions;

namespace SkipperData.Data.Repositories;

// Generic repository for type-specific order operations
public class OrderRepository<TOrderEntity, TCustomer> : Repository<SkipperContext, TOrderEntity>, IOrderRepository<TOrderEntity, TCustomer>
    where TOrderEntity : OrderEntity
    where TCustomer : CustomerEntity
{
    public OrderRepository(
        SkipperContext context, 
        ILogger<OrderRepository<TOrderEntity, TCustomer>> logger, 
        Func<DbSet<TOrderEntity>, Expression<Func<DbSet<TOrderEntity>, IQueryable>>?, IQueryable<TOrderEntity>>? baseQuery = null) 
    : base(context, logger, s => baseQuery is null 
        ? s.OfType<TOrderEntity>().Include(order => order.Customer)
        : baseQuery(s, s2 => s2.OfType<TOrderEntity>().Include(order => order.Customer)))
    {
    }
    
    protected override void UpdateEntity(TOrderEntity target, TOrderEntity source)
    {
        base.UpdateEntity(target, source);
        target.Status = source.Status;
        target.CustomerId = source.CustomerId;
        target.OrderDate = source.OrderDate;
        target.TotalAmount = source.TotalAmount;
    }
    
    public virtual async Task<IEnumerable<TOrderEntity>> GetOrdersByCustomerAsync(long customerId)
    {
        return await Query
            .Where(o => o.CustomerId == customerId && !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<TOrderEntity>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await Query
            .Where(o => o.Status == status)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<TOrderEntity>> GetActiveOrdersAsync()
    {
        var activeStatuses = new[] { OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.InProgress };
        
        return await Query
            .Where(o => activeStatuses.Contains(o.Status))
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<TOrderEntity>> GetOverdueOrdersAsync()
    {
        var currentDate = DateTime.UtcNow.Date;
        
        return await Query
            .Where(o => o.Status == OrderStatus.Pending && 
                       o.OrderDate.Date < currentDate.AddDays(-30) && 
                       !o.IsDeleted)
            .ToListAsync();
    }
}