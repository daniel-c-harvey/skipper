using System.Linq.Expressions;
using NetBlocks.Models;
using SkipperData.Data.Repositories;
using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public abstract class ManagerBase<TEntity, TDto> : IManager<TEntity, TDto>
where TEntity : class, IEntity<TEntity, TDto>
where TDto : class, IModel<TDto, TEntity>
{
    protected IRepository<TEntity, TDto> _repository;

    protected ManagerBase(IRepository<TEntity, TDto> repository)
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
}