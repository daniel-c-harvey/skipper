using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperWeb.ApiClients;
using System.Diagnostics;

namespace SkipperWeb.Components.Pages.Vessels;

public class VesselsViewModel
{
    public string SearchTerm { get; set; } = string.Empty;
    public PagedResult<Vessel>? Page { get; private set; }
    
    private readonly VesselClient _client;
    private int _currentPage = 0;
    private int _currentPageSize = 0;
    private int? _cachedPageCount;

    public VesselsViewModel(VesselClient client)
    {
        _client = client;
    }

    public async Task<(int, int)> SetPage(int pageNumber, int pageSize)
    {        
        // Avoid unnecessary API calls if we're requesting the same page
        if (_currentPage == pageNumber && _currentPageSize == pageSize && Page != null)
        {
            return (pageNumber, pageSize);
        }

        int pageCount = 1;
        if (Page is null || _cachedPageCount is null)
        {
            var countResult = await _client.GetPageCount(new PagedQuery { PageSize = pageSize });
            if (!countResult.Success) { throw new Exception("TODO present error"); }
            pageCount = countResult.Value;
            _cachedPageCount = pageCount;
        }
        else
        {
            pageCount = _cachedPageCount.Value;
        }

        if (pageNumber < 1 || pageNumber > pageCount)
        {
            pageNumber = 1;
        }

        var result = await _client.GetByPage(new PagedQuery { Page = pageNumber, PageSize = pageSize });
        if (result.Success && result.Value != null)
        {
            Page = result.Value;
            _currentPage = pageNumber;
            _currentPageSize = pageSize;
        }
        else
        {
            // TODO show the error
            Debugger.Break();
        }

        return (pageNumber, pageSize);
    }

    public void ClearCache()
    {
        _cachedPageCount = null;
        Page = null;
        _currentPage = 0;
        _currentPageSize = 0;
    }
}