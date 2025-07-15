using System.Linq.Expressions;
using Data.Shared.Data.Repositories;
using Models.Shared.Common;
using Models.Shared.Entities;
using NetBlocks.Models;

namespace Data.Shared.Managers;

public abstract class ManagerBase<TEntity, TRepository> : IManager<TEntity>
where TEntity : class, IEntity
where TRepository : IRepository<TEntity>
{
    protected TRepository Repository;

    protected ManagerBase(TRepository repository)
    {
        Repository = repository;
    }

    public virtual async Task<ResultContainer<bool>> Exists(TEntity entity)
    {
        try
        {
            var exists = await Repository.ExistsAsync(entity.Id);
            return ResultContainer<bool>.CreatePassResult(exists);
        }
        catch (Exception ex)
        {
            return ResultContainer<bool>.CreateFailResult(ex.Message);
        }
    }

    public async Task<ResultContainer<TEntity>> GetById(long id)
    {
        try
        {
            return ResultContainer<TEntity>.CreatePassResult(await Repository.GetByIdAsync(id));
        }
        catch (Exception e)
        {
            return ResultContainer<TEntity>.CreateFailResult(e.Message);
        }
    }

    public async Task<ResultContainer<IEnumerable<TEntity>>> Get(Expression<Func<TEntity, bool>>? predicate = null)
    {
        try
        {
            if (predicate is null)
            {
                return ResultContainer<IEnumerable<TEntity>>.CreatePassResult(await Repository.GetAllAsync());
            }
            return ResultContainer<IEnumerable<TEntity>>.CreatePassResult(await Repository.FindAsync(predicate));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<ResultContainer<int>> GetPageCount(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters)
    {
        try
        {
            return ResultContainer<int>.CreatePassResult
            (
                await Repository.GetPageCountAsync(predicate, pagingParameters)
            );
        }
        catch (Exception e)
        {
            return ResultContainer<int>.CreateFailResult(e.Message);
        }
    }

    public virtual async Task<ResultContainer<PagedResult<TEntity>>> GetPage(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters)
    {
        try
        {
            return ResultContainer<PagedResult<TEntity>>.CreatePassResult
            (
                await Repository.GetPagedAsync(predicate, pagingParameters)    
            );
        }
        catch (Exception e)
        {
            return  ResultContainer<PagedResult<TEntity>>.CreateFailResult(e.Message);
        }
    }

    public virtual async Task<Result> Add(TEntity entity)
    {
        try
        {
            await Repository.AddAsync(entity);
            return await Repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result.CreateFailResult(ex.Message);
        }
    }

    public virtual async Task<Result> Update(TEntity entity)
    {
        try
        {
            await Repository.UpdateAsync(entity);
            return await Repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result.CreateFailResult(ex.Message);
        }
    }

    public async Task<Result> Delete(long id)
    {
        try
        {
            await Repository.DeleteAsync(id);
            return await Repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result.CreateFailResult(ex.Message);
        }
    }
}