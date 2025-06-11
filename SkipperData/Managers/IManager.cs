using System.Linq.Expressions;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperData.Managers;

public interface IManager<TEntity>
where TEntity : BaseEntity
{
    Task<Result> Add(TEntity vessel);
    Task<ResultContainer<PagedResult<TEntity>>> GetPage(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters);
}