using System.Linq.Expressions;
using Data.Shared.Data.Repositories;
using Models.Shared.Common;
using Models.Shared.Entities;
using Models.Shared.Models;
using NetBlocks.Models;

namespace Data.Shared.Managers;

public abstract class ManagerBase<TEntity, TDto, TRepository> : IManager<TEntity, TDto>
where TEntity : class, IEntity<TEntity, TDto>
where TDto : class, IModel<TDto, TEntity>
where TRepository : IRepository<TEntity, TDto>
{
    protected TRepository _repository;

    protected ManagerBase(TRepository repository)
    {
        _repository = repository;
    }

    public virtual async Task<ResultContainer<bool>> Exists(TEntity entity)
    {
        try
        {
            var exists = await _repository.ExistsAsync(entity.Id);
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
            return ResultContainer<TEntity>.CreatePassResult(await _repository.GetByIdAsync(id));
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
                return ResultContainer<IEnumerable<TEntity>>.CreatePassResult(await _repository.GetAllAsync());
            }
            return ResultContainer<IEnumerable<TEntity>>.CreatePassResult(await _repository.FindAsync(predicate));
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
                await _repository.GetPageCountAsync(predicate, pagingParameters)
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
                await _repository.GetPagedAsync(predicate, pagingParameters)    
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
            await _repository.AddAsync(entity);
            return await _repository.SaveChangesAsync();
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
            await _repository.UpdateAsync(entity);
            return await _repository.SaveChangesAsync();
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
            await _repository.DeleteAsync(id);
            return await _repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result.CreateFailResult(ex.Message);
        }
    }
}