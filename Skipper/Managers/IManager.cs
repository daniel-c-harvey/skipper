using System.Linq.Expressions;
using NetBlocks.Models;
using Skipper.Common;
using Skipper.Data.Repositories;
using Skipper.Domain.Entities;

namespace Skipper.Managers;

public interface IManager<TEntity>
where TEntity : BaseEntity
{
    Task<Result> Add(TEntity vessel);
    Task<ResultContainer<PagedResult<TEntity>>> GetPage(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters);
}