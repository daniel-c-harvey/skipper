using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using Models.Shared.Entities;

namespace Data.Shared.Data.Repositories;

public class Repository<TContext, TEntity> : RepositoryBase<TContext, TEntity>, IRepository<TEntity>
where TContext : DbContext
where TEntity : class, IEntity
{
    protected DbSet<TEntity> Set { get; private init; }
    protected IQueryable<TEntity> Query { get; init; }

    public Repository(TContext context, 
                      ILogger<Repository<TContext, TEntity>> logger,
                      Func<DbSet<TEntity>, IQueryable<TEntity>>? baseQuery = null) : base(context, logger)
    {
        Set = context.Set<TEntity>();
        Query = baseQuery != null 
            ? baseQuery(Set).Where(e => !e.IsDeleted)
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
        return await Query.CountAsync();
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
}