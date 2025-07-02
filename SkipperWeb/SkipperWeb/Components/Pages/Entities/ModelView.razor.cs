using Microsoft.AspNetCore.Components;
using MudBlazor;
using SkipperModels.Entities;
using SkipperModels.Models;
using SkipperWeb.ApiClients;

namespace SkipperWeb.Components.Pages.Entities;

[CascadingTypeParameter(nameof(T))]
public partial class ModelView<T, TEntity, TClient, TClientConfig>  : ComponentBase
    where T : class, IModel<T, TEntity>, new()
    where TEntity : class, IEntity<TEntity, T>, new()
    where TClient : ModelControllerClient<T, TEntity, TClientConfig>
    where TClientConfig : ModelControllerClientConfig
{
    [SupplyParameterFromQuery]
    public int? Page { get; set; }

    [SupplyParameterFromQuery]
    public int? PageSize { get; set; }

    [SupplyParameterFromQuery]
    public string? SearchTerm { get; set; }
    
    [Inject]
    public required ModelPageViewModel<T, TEntity, TClient, TClientConfig> ViewModel { get; set; }
    
    [Inject]
    public required NavigationManager Nav { get; set; }
    
    [Parameter]
    public required string PageRoute { get; set; }
    
    [Parameter]
    public required string Title { get; set; }
    
    [Parameter]
    public required string ModelDisplayName { get; set; }
    
    [Parameter]
    public required RenderFragment Columns { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = "Search...";

    private MudDataGrid<T>? _grid;
    private bool _updatingParameters = false;
    
    
    protected override async Task OnParametersSetAsync()
    {
        // If parameters are missing, set defaults and update URL
        if (Page == null || PageSize == null || SearchTerm is null)
        {
            UpdatePageWithUrl(Page ?? 1, PageSize ?? 10, SearchTerm ?? string.Empty, replace: true);
            return; // URL change will trigger OnParametersSetAsync again with proper values
        }

        _updatingParameters = true;
        if (_grid != null) await _grid.ReloadServerData();
        _updatingParameters = false;
    }

    private void OnSearchChanged(string searchTerm)
    {
        UpdatePageWithUrl(1, PageSize ?? 10, searchTerm);
    }

    private async Task<GridData<T>> LoadGridServerData(GridState<T> state)
    {
        if (!_updatingParameters) UpdatePageWithUrl(state.Page + 1, state.PageSize, ViewModel.SearchTerm);
        await LoadServerData(Page ?? 1, PageSize ?? 10 , SearchTerm ?? string.Empty);

        return new GridData<T>()
        {
            Items = ViewModel.Page?.Items ?? Enumerable.Empty<T>(),
            TotalItems = ViewModel.Page?.TotalCount ?? 0
        };
    }
    
    private void UpdatePageWithUrl(int page, int pageSize, string searchTerm, bool replace = false)
    {
        var currentPage = Page;
        var currentPageSize = PageSize;
        var currentSearchTerm = ViewModel.SearchTerm;

        if (currentPage == page && currentPageSize == pageSize && currentSearchTerm == searchTerm) 
            return;
        
        var queryParams = new Dictionary<string, object?>
        {
            ["Page"] = page,
            ["PageSize"] = pageSize,
            ["SearchTerm"] = searchTerm
        };

        var queryString = Nav.GetUriWithQueryParameters(queryParams);
        Nav.NavigateTo(queryString, forceLoad: false, replace: replace);
    }
    
    private async Task LoadServerData(int page, int pageSize, string searchTerm)
    {
        // Ensure ViewModel has the latest data
        (page, pageSize) = await ViewModel.SetPage(page, pageSize, searchTerm);

        // if page was coerced, update again
        if (page != Page || pageSize != PageSize)
        {
            UpdatePageWithUrl(page, pageSize, searchTerm);
        }
    }

    private void NewEntity()
    {
        Nav.NavigateTo($"{PageRoute}/new");
    }
}