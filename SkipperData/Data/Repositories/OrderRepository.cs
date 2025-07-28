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
public class OrderRepository<TOrderEntity, TCustomer> : RepositoryBase<SkipperContext, TOrderEntity>, IOrderRepository<TOrderEntity, TCustomer>
    where TOrderEntity : OrderEntity<TCustomer>
    where TCustomer : CustomerEntity
{
    public OrderRepository(SkipperContext context, ILogger<OrderRepository<TOrderEntity, TCustomer>> logger) 
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
        
        var result = await Context.Set<TOrderEntity>().AddAsync(entity);
        await SaveChangesAsync();
        return result.Entity;
    }

    public virtual async Task UpdateAsync(TOrderEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<TOrderEntity>().Update(entity);
        await SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await SaveChangesAsync();
        }
    }

    public virtual async Task<bool> ExistsAsync(long id)
    {
        return await Context.Orders
            .OfType<TOrderEntity>()
            .AnyAsync(o => o.Id == id && !o.IsDeleted);
    }

    public virtual async Task<int> CountAsync()
    {
        return await Context.Orders
            .OfType<TOrderEntity>()
            .CountAsync(o => !o.IsDeleted);
    }

    public virtual async Task<int> CountAsync(Expression<Func<TOrderEntity, bool>> predicate)
    {
        return await Context.Orders
            .OfType<TOrderEntity>()
            .Where(o => !o.IsDeleted)
            .CountAsync(predicate);
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

    #region IOrderRepository<TOrderEntity, TCustomer> Type-Specific Methods (return TOrderEntity only)

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