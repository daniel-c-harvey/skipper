using System.Linq.Expressions;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public interface IManager<TEntity, TDto>
where TEntity : class, IEntity<TEntity, TDto>
where TDto : class, IModel<TDto, TEntity>
{
    Task<Result> Add(TEntity vessel);
    Task<ResultContainer<int>> GetPageCount(Expression<Func<TEntity, bool>> predicate,  PagingParameters<TEntity> pagingParameters);
    Task<ResultContainer<PagedResult<TEntity>>> GetPage(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters);
}