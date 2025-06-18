using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperWeb.ApiClients;

namespace SkipperWeb.Components.Pages.Vessels;

public class VesselsViewModel
{
    //private int _pageNumber;
    //private int _pageCount;
    //public int PageNumber => _pageNumber;
    //public int PageCount => _pageCount;
    
    public string SearchTerm { get; set; } =  string.Empty;
    public PagedResult<Vessel>? Page { get; set; }

    private VesselClient _client;

    public VesselsViewModel(VesselClient client)
    {
        _client = client;
    }

    public async Task SetPage(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > (Page?.TotalPages ?? 1)) { pageNumber = 1; }

        var result = await _client.GetByPage(new PagedQuery { Page = pageNumber });
        if (result.Success && result.Value != null)
        {
            Page = result.Value;
        } // TODO show the error
    }
}