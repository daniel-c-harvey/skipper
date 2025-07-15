using Models.Shared.Common;
using Models.Shared.Models;

namespace Web.Shared.Maintenance.Entities;

public interface IModelPageViewModel<TModel> 
    where TModel : class, IModel, new()
{
    PagedResult<TModel>? Page { get; }
    NetBlocks.Models.Result? ErrorResults { get; set; }
    string SearchTerm { get; }
    Task<(int, int)> SetPage(int pageNumber, int pageSize, string searchTerm, bool refresh = false);
    void ClearCache();
    Task UpdateItem(TModel model);
    Task DeleteItem(TModel model);
}