using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Shared;
using Models.Shared.Common;
using Models.Shared.Entities;
using NetBlocks.Models;

namespace Data.Shared.Data.Repositories;

public abstract class RepositoryBase<TContext, TEntity> where TContext : DbContext where TEntity : class, IKeyed
{
    private TContext _context;
    protected ILogger<RepositoryBase<TContext,TEntity>> Logger;

    protected RepositoryBase(TContext context, ILogger<RepositoryBase<TContext,TEntity>> logger)
    {
        _context = context;
        Logger = logger;
    }
    
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
            await _context.SaveChangesAsync();
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