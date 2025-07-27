using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Shared.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels;
using System.Linq.Expressions;
using Models.Shared.Common;
using NetBlocks.Models;

namespace SkipperData.Data.Repositories;

// Generic repository for type-specific order operations
public class OrderRepository<TOrderEntity> : RepositoryBase<SkipperContext, TOrderEntity>, IOrderRepository<TOrderEntity>
    where TOrderEntity : OrderEntity
{
    public OrderRepository(SkipperContext context, ILogger<OrderRepository<TOrderEntity>> logger) 
        : base(context, logger)
    {
    }

    #region IRepository<TOrderEntity> Implementation (Type-Specific CRUD)

    public virtual async Task<TOrderEntity?> GetByIdAsync(long id)
    {
        return await Context.Orders
            .OfType<TOrderEntity>()
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }

    public virtual async Task<IEnumerable<TOrderEntity>> GetAllAsync()
    {
        return await Context.Orders
            .OfType<TOrderEntity>()
            .Include(o => o.Customer)
            .Where(o => !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<TOrderEntity>> FindAsync(Expression<Func<TOrderEntity, bool>> predicate)
    {
        return await Context.Orders
            .OfType<TOrderEntity>()
            .Include(o => o.Customer)
            .Where(o => !o.IsDeleted)
            .Where(predicate)
            .ToListAsync();
    }

    public virtual async Task<TOrderEntity> AddAsync(TOrderEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        var result = await Context.Orders.AddAsync(entity);
        await SaveChangesAsync();
        return (TOrderEntity)result.Entity;
    }

    public virtual async Task UpdateAsync(TOrderEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Orders.Update(entity);
        await SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            await UpdateAsync(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(long id)
    {
        return await Context.Orders
            .OfType<TOrderEntity>()
            .AnyAsync(o => o.Id == id && !o.IsDeleted);
    }

    public virtual async Task<int> GetPageCountAsync(Expression<Func<TOrderEntity, bool>> predicate, PagingParameters<TOrderEntity> pagingParameters)
    {
        var count = await Context.Orders
            .OfType<TOrderEntity>()
            .Where(o => !o.IsDeleted)
            .Where(predicate)
            .CountAsync();
            
        return (int)Math.Ceiling((double)count / pagingParameters.PageSize);
    }

    public virtual async Task<PagedResult<TOrderEntity>> GetPagedAsync(PagingParameters<TOrderEntity> pagingParameters)
    {
        var query = Context.Orders
            .OfType<TOrderEntity>()
            .Include(o => o.Customer)
            .Where(o => !o.IsDeleted);
            
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    public virtual async Task<PagedResult<TOrderEntity>> GetPagedAsync(Expression<Func<TOrderEntity, bool>> predicate, PagingParameters<TOrderEntity> pagingParameters)
    {
        var query = Context.Orders
            .OfType<TOrderEntity>()
            .Include(o => o.Customer)
            .Where(o => !o.IsDeleted)
            .Where(predicate);
            
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    #endregion

    #region IOrderRepository<TOrderEntity> Type-Specific Methods (return TOrderEntity only)

    public virtual async Task<IEnumerable<TOrderEntity>> GetOrdersByCustomerAsync(long customerId)
    {
        return await Context.Orders
            .OfType<TOrderEntity>()
            .Include(o => o.Customer)
            .Where(o => o.CustomerId == customerId && !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<TOrderEntity>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await Context.Orders
            .OfType<TOrderEntity>()
            .Include(o => o.Customer)
            .Where(o => o.Status == status && !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<TOrderEntity>> GetActiveOrdersAsync()
    {
        var activeStatuses = new[] { OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.InProgress };
        
        return await Context.Orders
            .OfType<TOrderEntity>()
            .Include(o => o.Customer)
            .Where(o => activeStatuses.Contains(o.Status) && !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<TOrderEntity>> GetOverdueOrdersAsync()
    {
        var currentDate = DateTime.UtcNow.Date;
        
        return await Context.Orders
            .OfType<TOrderEntity>()
            .Include(o => o.Customer)
            .Where(o => o.Status == OrderStatus.Pending && 
                       o.OrderDate.Date < currentDate.AddDays(-30) && 
                       !o.IsDeleted)
            .ToListAsync();
    }

    #endregion
}

// Base non-generic repository for cross-type operations only
public class OrderRepository : RepositoryBase<SkipperContext, OrderEntity>, IOrderRepository
{
    public OrderRepository(SkipperContext context, ILogger<OrderRepository> logger) 
        : base(context, logger)
    {
    }

    #region IRepository<OrderEntity> Implementation

    public virtual async Task<OrderEntity?> GetByIdAsync(long id)
    {
        return await Context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }

    public virtual async Task<IEnumerable<OrderEntity>> GetAllAsync()
    {
        return await Context.Orders
            .Include(o => o.Customer)
            .Where(o => !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<OrderEntity>> FindAsync(Expression<Func<OrderEntity, bool>> predicate)
    {
        return await Context.Orders
            .Include(o => o.Customer)
            .Where(o => !o.IsDeleted)
            .Where(predicate)
            .ToListAsync();
    }

    public virtual async Task<OrderEntity> AddAsync(OrderEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        var result = await Context.Orders.AddAsync(entity);
        await SaveChangesAsync();
        return result.Entity;
    }

    public virtual async Task UpdateAsync(OrderEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Orders.Update(entity);
        await SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            await UpdateAsync(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(long id)
    {
        return await Context.Orders.AnyAsync(o => o.Id == id && !o.IsDeleted);
    }

    public virtual async Task<int> GetPageCountAsync(Expression<Func<OrderEntity, bool>> predicate, PagingParameters<OrderEntity> pagingParameters)
    {
        var count = await Context.Orders
            .Where(o => !o.IsDeleted)
            .Where(predicate)
            .CountAsync();
            
        return (int)Math.Ceiling((double)count / pagingParameters.PageSize);
    }

    public virtual async Task<PagedResult<OrderEntity>> GetPagedAsync(PagingParameters<OrderEntity> pagingParameters)
    {
        var query = Context.Orders
            .Include(o => o.Customer)
            .Where(o => !o.IsDeleted);
            
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    public virtual async Task<PagedResult<OrderEntity>> GetPagedAsync(Expression<Func<OrderEntity, bool>> predicate, PagingParameters<OrderEntity> pagingParameters)
    {
        var query = Context.Orders
            .Include(o => o.Customer)
            .Where(o => !o.IsDeleted)
            .Where(predicate);
            
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    #endregion

    #region IOrderRepository Cross-Type Methods (work with mixed OrderEntity types)

    public virtual async Task<OrderEntity?> GetByOrderNumberAsync(string orderNumber)
    {
        return await Context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && !o.IsDeleted);
    }

    public virtual async Task<IEnumerable<OrderEntity>> GetOrdersByCustomerAsync(long customerId)
    {
        return await Context.Orders
            .Include(o => o.Customer)
            .Where(o => o.CustomerId == customerId && !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await Context.Orders
            .Include(o => o.Customer)
            .Where(o => o.Status == status && !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<OrderEntity>> GetOrdersByTypeAsync(OrderType orderType)
    {
        return await Context.Orders
            .Include(o => o.Customer)
            .Where(o => o.OrderType == orderType && !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<OrderEntity>> GetOrdersByTypesAsync(params OrderType[] orderTypes)
    {
        return await Context.Orders
            .Include(o => o.Customer)
            .Where(o => orderTypes.Contains(o.OrderType) && !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<OrderEntity>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Context.Orders
            .Include(o => o.Customer)
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && !o.IsDeleted)
            .OrderBy(o => o.OrderDate)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<OrderEntity>> GetActiveOrdersAsync()
    {
        var activeStatuses = new[] { OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.InProgress };
        
        return await Context.Orders
            .Include(o => o.Customer)
            .Where(o => activeStatuses.Contains(o.Status) && !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<OrderEntity>> GetOverdueOrdersAsync()
    {
        var currentDate = DateTime.UtcNow.Date;
        
        return await Context.Orders
            .Include(o => o.Customer)
            .Where(o => o.Status == OrderStatus.Pending && 
                       o.OrderDate.Date < currentDate.AddDays(-30) && 
                       !o.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<decimal> GetTotalRevenueByTypeAsync(OrderType orderType, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = Context.Orders
            .Where(o => o.OrderType == orderType && 
                       o.Status == OrderStatus.Completed && 
                       !o.IsDeleted);

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);
            
        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        return await query.SumAsync(o => o.TotalAmount) / 100m; // Convert from cents
    }

    public virtual async Task<PagedResult<OrderEntity>> GetOrdersPagedAsync(
        Expression<Func<OrderEntity, bool>>? predicate = null,
        PagingParameters<OrderEntity>? pagingParameters = null)
    {
        var query = Context.Orders
            .Include(o => o.Customer)
            .Where(o => !o.IsDeleted);

        if (predicate != null)
            query = query.Where(predicate);

        var paging = pagingParameters ?? new PagingParameters<OrderEntity>();
        
        // Manual paging implementation
        var totalCount = await query.CountAsync();
        
        if (paging.OrderBy != null)
        {
            query = query.OrderBy(paging.OrderBy);
        }
        
        var items = await query
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();
            
        return new PagedResult<OrderEntity>(items, totalCount, paging.Page, paging.PageSize);
    }

    #endregion
}