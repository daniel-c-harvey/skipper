using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using Models.Shared.Entities;
using NetBlocks.Models;

namespace Data.Shared.Data.Repositories;

public class Repository<TContext, TEntity> : RepositoryBase<TContext, TEntity>, IRepository<TEntity>
where TContext : DbContext
where TEntity : class, IEntity
{
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(TContext context, ILogger<Repository<TContext, TEntity>> logger) : base(context, logger)
    {
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
    }

    public async Task<int> GetPageCountAsync(PagingParameters<TEntity> pagingParameters)
    {
        return await _dbSet.CountAsync(e => !e.IsDeleted);
    }
    
    public async Task<int> GetPageCountAsync(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters)
    {
        double rowCount = (double)(await _dbSet.Where(e => !e.IsDeleted).CountAsync(predicate)) / pagingParameters.PageSize;
        return (int)Math.Ceiling(rowCount);
    }
    
    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(PagingParameters<TEntity> pagingParameters)
    {
        var query = _dbSet.Where(e => !e.IsDeleted);
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters)
    {
        var query = _dbSet.Where(e => !e.IsDeleted).Where(predicate);
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        
        // This method handles both tracked and untracked entities
        var dbentity = await _dbSet.Where(e => e.Id == entity.Id).FirstOrDefaultAsync();
        if (dbentity != null)
        {
            UpdateModel(dbentity, entity);
        }
        await SaveChangesAsync();
    }

    protected virtual void UpdateModel(TEntity target, TEntity source)
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
        return await _dbSet.AnyAsync(e => e.Id == id && !e.IsDeleted);
    }
}