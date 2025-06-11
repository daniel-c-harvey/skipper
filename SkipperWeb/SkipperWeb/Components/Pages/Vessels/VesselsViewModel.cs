using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperWeb.Components.Pages.Vessels;

public class VesselsViewModel
{
    public int PageNumber { get; set; } = 1;
    public string SearchTerm { get; set; } =  string.Empty;
    public PagedResult<Vessel>? Page { get; set; }
}