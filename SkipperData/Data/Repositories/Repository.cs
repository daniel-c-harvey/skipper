using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class Repository<TEntity, TDto> : IRepository<TEntity, TDto>
where TEntity : class, IEntity<TEntity, TDto>
where TDto : class, IModel<TDto, TEntity>
{
    protected readonly SkipperContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly ILogger<Repository<TEntity, TDto>> _logger;

    public Repository(SkipperContext context, ILogger<Repository<TEntity, TDto>> logger)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _logger = logger;
    }

    public async Task<TEntity?> GetByIdAsync(long id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
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
    
    public async Task<PagedResult<TEntity>> GetPagedAsync(PagingParameters<TEntity> pagingParameters)
    {
        var query = _dbSet.Where(e => !e.IsDeleted);
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    public async Task<PagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters)
    {
        var query = _dbSet.Where(e => !e.IsDeleted).Where(predicate);
        return await ExecutePagedQueryAsync(query, pagingParameters);
    }

    private async Task<PagedResult<TEntity>> ExecutePagedQueryAsync(IQueryable<TEntity> query, PagingParameters<TEntity> pagingParameters)
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

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(TEntity entity)
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

    public async Task<Result> SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            return Result.CreatePassResult();
        }
        catch (Exception ex)
        {
            
            _context.ChangeTracker.Clear();
            _logger.LogError(ex, ex.Message);
            return Result.CreateFailResult("A database error occured while saving changes.");
        }
    }
} 