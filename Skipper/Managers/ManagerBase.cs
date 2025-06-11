using System.Linq.Expressions;
using NetBlocks.Models;
using Skipper.Common;
using Skipper.Data.Repositories;
using Skipper.Domain.Entities;

namespace Skipper.Managers;

public abstract class ManagerBase<TEntity> : IManager<TEntity>
where TEntity : BaseEntity
{
    protected IRepository<TEntity> _repository;

    public ManagerBase(IRepository<TEntity> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task<Result> Add(TEntity vessel)
    {
        try
        {
            await _repository.AddAsync(vessel);
            await _repository.SaveChangesAsync();
            return Result.CreatePassResult();
        }
        catch (Exception ex)
        {
            return Result.CreateFailResult(ex.Message);
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
            Console.WriteLine(e);
            throw;
        }
    }
}