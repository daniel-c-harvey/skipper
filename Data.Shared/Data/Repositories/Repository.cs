using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using Models.Shared.Entities;
using NetBlocks.Models;

namespace Data.Shared.Data.Repositories;

public class Repository<TContext, TEntity> : IRepository<TEntity>
where TContext : DbContext
where TEntity : class, IEntity
{
    
    private TContext _context;
    protected ILogger<Repository<TContext,TEntity>> Logger;
    protected DbSet<TEntity> Set { get; private init; }
    protected IQueryable<TEntity> Query { get; init; }

    public Repository(TContext context, 
                      ILogger<Repository<TContext, TEntity>> logger,
                      Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAdditions = null)
    {
        _context = context;
        Logger = logger;
        Set = context.Set<TEntity>();
        Query = queryAdditions != null 
            ? queryAdditions(Set.Where(e => !e.IsDeleted))
            : Set.Where(e => !e.IsDeleted);
    }

    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        return await Query.FirstOrDefaultAsync(e => e.Id == id );
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Query.ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Query.Where(predicate).ToListAsync();
    }

    public async Task<int> GetPageCountAsync(PagingParameters<TEntity> pagingParameters)
    {
        double rowCount = (double)(await Query.CountAsync()) / pagingParameters.PageSize;
        return (int)Math.Ceiling(rowCount);
    }
    
    public async Task<int> GetPageCountAsync(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters)
    {
        double rowCount = (double)(await Query.CountAsync(predicate)) / pagingParameters.PageSize;
        return (int)Math.Ceiling(rowCount);
    }
    
    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(PagingParameters<TEntity> pagingParameters)
    {
        var query = Query.Where(e => !e.IsDeleted);
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters)
    {
        var query = Set.Where(e => !e.IsDeleted).Where(predicate);
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        var addedEntity = await Set.AddAsync(entity);
        await SaveChangesAsync();
        
        // Fresh entity will not have navigation properties attached, load from DB
        return (await GetByIdAsync(addedEntity.Entity.Id))!;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        
        // This method handles both tracked and untracked entities
        var fullEntity = await GetByIdAsync(entity.Id);
        if (fullEntity != null)
        {
            UpdateEntity(fullEntity, entity);
        }
        await SaveChangesAsync();
    }

    protected virtual void UpdateEntity(TEntity target, TEntity source)
    {
        target.CreatedAt = source.CreatedAt;
        target.UpdatedAt = source.UpdatedAt;
        target.IsDeleted = source.IsDeleted;
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(entity);
            await SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await Query.AnyAsync(e => e.Id == id);
    }
    
    // Helpers
    protected async Task<PagedResult<TEntity>> ExecutePagedQueryAsync(IQueryable<TEntity> query, PagingParameters<TEntity> pagingParameters)
    {
        // Apply ordering
        if (pagingParameters.OrderBy != null)
        {
            query = pagingParameters.IsDescending 
                ? query.OrderByDescending(pagingParameters.OrderBy)
                : query.OrderBy(pagingParameters.OrderBy);
        }
        else
        {
            // Default ordering by Id if no ordering specified
            query = query.OrderBy(e => e.Id);
        }

        // Get total count before paging
        var totalCount = await query.CountAsync();

        // Apply paging
        var items = await query
            .Skip(pagingParameters.Skip)
            .Take(pagingParameters.PageSize)
            .ToListAsync();

        return new PagedResult<TEntity>(items, totalCount, pagingParameters.Page, pagingParameters.PageSize);
    }

    protected async Task<Result> SaveChangesAsync()
    {
        try
        {
            var x = await _context.SaveChangesAsync();
            return Result.CreatePassResult();
        }
        catch (Exception ex)
        {
            _context.ChangeTracker.Clear();
            LoggerExtensions.LogError(Logger, ex, ex.Message);
            return Result.CreateFailResult("A database error occured while saving changes.");
        }
    }
}