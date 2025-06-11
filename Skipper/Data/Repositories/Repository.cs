using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Skipper.Common;
using SkipperModels.Entities;

namespace Skipper.Data.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly SkipperContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(SkipperContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(long id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
    }

    public async Task<PagedResult<T>> GetPagedAsync(PagingParameters<T> pagingParameters)
    {
        var query = _dbSet.Where(e => !e.IsDeleted);
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    public async Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, PagingParameters<T> pagingParameters)
    {
        var query = _dbSet.Where(e => !e.IsDeleted).Where(predicate);
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    private async Task<PagedResult<T>> ExecutePagedQueryAsync(IQueryable<T> query, PagingParameters<T> pagingParameters)
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

        return new PagedResult<T>(items, totalCount, pagingParameters.Page, pagingParameters.PageSize);
    }

    public async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(entity);
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id && !e.IsDeleted);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
} 