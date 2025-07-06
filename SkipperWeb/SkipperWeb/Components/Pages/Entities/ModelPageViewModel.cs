using System.Diagnostics;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperModels.Models;
using SkipperWeb.ApiClients;

namespace SkipperWeb.Components.Pages.Entities;

public class ModelPageViewModel<TModel, TEntity, TClient, TClientConfig> 
    where TModel : class, IModel<TModel, TEntity>, new()
    where TEntity : class, IEntity<TEntity, TModel>, new()
    where TClient : ModelControllerClient<TModel, TEntity, TClientConfig>
    where TClientConfig : ModelControllerClientConfig
{
    public PagedResult<TModel>? Page { get; private set; }
    public Result? ErrorResults { get; set; }

    public string SearchTerm => _currentSearchTerm;
    
    private readonly TClient _client;
    private int _currentPage = 0;
    private int _currentPageSize = 0;
    private string _currentSearchTerm = string.Empty;
    private int? _cachedPageCount;

    public ModelPageViewModel(TClient client)
    {
        _client = client;
    }

    public async Task<(int, int)> SetPage(int pageNumber, int pageSize, string searchTerm, bool refresh = false)
    {        
        // Avoid unnecessary API calls if we're requesting the same page
        if (!refresh && _currentPage == pageNumber && _currentPageSize == pageSize && 
            searchTerm == _currentSearchTerm && Page != null)
        {
            return (pageNumber, pageSize);
        }

        int pageCount = 1;
        if (Page is null || _cachedPageCount is null)
        {
            var countResult = await _client.GetPageCount(
                new PagedQuery 
                { 
                    PageSize = pageSize, 
                    Search = searchTerm 
                });
            if (!countResult.Success || countResult.Value is null) 
            {
                ErrorResults = Result.From(countResult);
            }
            else
            {
                pageCount = countResult.Value.Count;
                _cachedPageCount = pageCount;
            }
        }
        else
        {
            pageCount = _cachedPageCount.Value;
        }

        if (pageNumber < 1 || pageNumber > pageCount)
        {
            pageNumber = 1;
        }

        var result = await _client.GetByPage(
            new PagedQuery 
            { 
                Page = pageNumber, 
                PageSize = pageSize, 
                Search = searchTerm 
            });
        if (result.Success && result.Value != null)
        {
            Page = result.Value;
            _currentSearchTerm = searchTerm;
            _currentPage = pageNumber;
            _currentPageSize = pageSize;
            ErrorResults = null;
        }
        else
        {
            ErrorResults = Result.From(result);
        }

        return (pageNumber, pageSize);
    }

    public void ClearCache()
    {
        _cachedPageCount = null;
        Page = null;
        _currentSearchTerm = string.Empty;
        _currentPage = 0;
        _currentPageSize = 0;
    }

    public async Task UpdateItem(TModel model)
    {
        var result = await _client.Post(model);

        ErrorResults = result.Success ? null : Result.From(result);
    }
    public async Task DeleteItem(TModel model)
    {
        var result = await _client.Delete(model);
        
        ErrorResults = result.Success ? null : Result.From(result);
    }
    
}