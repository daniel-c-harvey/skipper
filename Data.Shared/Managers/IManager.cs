using System.Linq.Expressions;
using Models.Shared.Common;
using Models.Shared.Entities;
using NetBlocks.Models;

namespace Data.Shared.Managers;

public interface IManager<TEntity>
where TEntity : class, IEntity
{
    Task<ResultContainer<bool>> Exists(TEntity entity);
    Task<ResultContainer<TEntity>> GetById(long id);
    Task<ResultContainer<IEnumerable<TEntity>>> Get(Expression<Func<TEntity, bool>>? predicate = null);
    Task<ResultContainer<int>> GetPageCount(Expression<Func<TEntity, bool>> predicate,  PagingParameters<TEntity> pagingParameters);
    Task<ResultContainer<PagedResult<TEntity>>> GetPage(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters);
    Task<Result> Add(TEntity entity);
    Task<Result> Update(TEntity entity);
    Task<Result> Delete(long id);
}