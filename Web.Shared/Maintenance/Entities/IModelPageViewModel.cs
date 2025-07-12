using Models.Shared.Common;
using Models.Shared.Entities;
using Models.Shared.Models;

namespace Web.Shared.Maintenance.Entities;

public interface IModelPageViewModel<TModel, TEntity> 
    where TModel : class, IModel<TModel, TEntity>, new()
    where TEntity : class, IEntity<TEntity,TModel>, new()
{
    PagedResult<TModel>? Page { get; }
    NetBlocks.Models.Result? ErrorResults { get; set; }
    string SearchTerm { get; }
    Task<(int, int)> SetPage(int pageNumber, int pageSize, string searchTerm, bool refresh = false);
    void ClearCache();
    Task UpdateItem(TModel model);
    Task DeleteItem(TModel model);
}