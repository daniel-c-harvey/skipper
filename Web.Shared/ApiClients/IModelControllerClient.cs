using Models.Shared.Common;
using Models.Shared.Entities;
using Models.Shared.Models;
using NetBlocks.Models;

namespace Web.Shared.ApiClients;

public interface IModelControllerClient<TModel, TEntity>
    where TModel : class, IModel<TModel, TEntity>, new()
    where TEntity : class, IEntity<TEntity, TModel>, new()
{
    Task<ApiResult<PagedResult<TModel>>> GetById(long id);
    Task<ApiResult<IEnumerable<TModel>>> GetAll();
    Task<ApiResult<PagedResult<TModel>>> GetByPage(PagedQuery query);
    Task<ApiResult<ItemCount>> GetPageCount(PagedQuery query);
    Task<ApiResult<TModel>> Update(TModel model);
    Task<ApiResult> Delete(TModel model);
}