using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using Models.Shared.Entities;
using NetBlocks.Models;

namespace Data.Shared.Data.Repositories;

public class CompositeRepository<TContext, TComposite, TRoot, TDiscriminator, TInfo> 
: RepositoryBase<TContext, TComposite>, 
  ICompositeRepository<TComposite, TRoot, TDiscriminator, TInfo>
where TContext : DbContext
where TComposite : class, ICompositeEntity<TRoot, TDiscriminator, TInfo>, new()
where TRoot : class, ICompositeEntityRoot<TDiscriminator>
where TInfo : class, IEntity
where TDiscriminator : Enum
{
    private readonly IQueryable<TComposite> _composite;

    public CompositeRepository(TContext context, TDiscriminator discriminator, ILogger<CompositeRepository<TContext, TComposite, TRoot, TDiscriminator, TInfo>> logger) : base(context, logger)
    {
        _composite = Context.Set<TRoot>()
            .Where(r => r.Discriminator.Equals(discriminator) && !r.IsDeleted)
            .Join(Context.Set<TInfo>(), r => r.Id, i => i.Id,
                (r, i) => new TComposite { Root = r, Info = i });
    }
    
    public virtual async Task<TComposite?> GetByIdAsync(long id)
    {
        return await _composite.Where(c => c.Root.Id == id).FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<TComposite>> GetAllAsync()
    {
        return await _composite.ToListAsync();
    }

    public virtual async Task<IEnumerable<TComposite>> FindAsync(Expression<Func<TComposite, bool>> predicate)
    {
        return await _composite.Where(predicate).ToListAsync();
    }

    public virtual async Task<TComposite> AddAsync(TComposite entity)
    {
        entity.Root.CreatedAt = DateTime.UtcNow;
        entity.Root.UpdatedAt = DateTime.UtcNow;
        entity.Info.CreatedAt = DateTime.UtcNow;
        entity.Info.UpdatedAt = DateTime.UtcNow;
        
        // Update info section first to get the ID
        var newInfoEntry = await Context.Set<TInfo>().AddAsync(entity.Info);
        await SaveChangesAsync();
        
        if (!newInfoEntry.IsKeySet) throw new Exception("Failed to get ID for new info entry.");
        
        // Replace the composite parts with the new entity and ID
        entity.Root.Id = newInfoEntry.Entity.Id;
        entity.Info = newInfoEntry.Entity;

        await Context.Set<TRoot>().AddAsync(entity.Root);
        await SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(TComposite entity)
    {
        entity.Root.UpdatedAt = DateTime.UtcNow;
        entity.Info.UpdatedAt = DateTime.UtcNow;
        
        var dbInfo = await Context.Set<TInfo>().Where(r => r.Id == entity.Info.Id).FirstAsync();
        UpdateInfo(dbInfo, entity.Info);
        Context.Set<TRoot>().Update(entity.Root);
        
        var dbRoot = await Context.Set<TRoot>().Where(r => r.Id == entity.Root.Id).FirstAsync();
        UpdateRoot(dbRoot, entity.Root);
        Context.Set<TInfo>().Update(entity.Info);
        
        await SaveChangesAsync();
    }

    protected virtual void UpdateInfo(TInfo target, TInfo source)
    {
        target.CreatedAt = source.CreatedAt;
        target.UpdatedAt = source.UpdatedAt;
        target.IsDeleted = source.IsDeleted;
    }

    protected virtual void UpdateRoot(TRoot target, TRoot source)
    {
        target.Id = source.Id;
        target.Discriminator = source.Discriminator;
        target.CreatedAt = source.CreatedAt;
        target.UpdatedAt = source.UpdatedAt;
        target.IsDeleted = source.IsDeleted;
    }

    public virtual async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.Root.IsDeleted = true;
            await UpdateAsync(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(long id)
    {
        return await _composite.AnyAsync(c => c.Root.Id == id);
    }

    public virtual async Task<int> GetPageCountAsync(Expression<Func<TComposite, bool>> predicate, PagingParameters<TComposite> pagingParameters)
    {
        double rowCount = (double)(await _composite.CountAsync(predicate)) / pagingParameters.PageSize;
        return (int)Math.Ceiling(rowCount);
    }

    public virtual async Task<PagedResult<TComposite>> GetPagedAsync(PagingParameters<TComposite> pagingParameters)
    {
        return await ExecutePagedQueryAsync(_composite, pagingParameters);
    }

    public virtual async Task<PagedResult<TComposite>> GetPagedAsync(Expression<Func<TComposite, bool>> predicate, PagingParameters<TComposite> pagingParameters)
    {
        var query = _composite.Where(predicate);
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }
}