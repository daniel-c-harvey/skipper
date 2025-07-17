using Microsoft.AspNetCore.Components;
using Models.Shared.Converters;
using Models.Shared.Entities;
using Models.Shared.InputModels;
using Models.Shared.Models;
using MudBlazor;

namespace Web.Shared.Maintenance.Entities;

[CascadingTypeParameter(nameof(T))]
public partial class ModelView<T, TModel, TEditModal, TViewModel, TConverter>  : ComponentBase
    where T : class, IInputModel, new()
    where TModel : class, IModel, new()
    where TEditModal : IEditModal<T>
    where TViewModel : IModelPageViewModel<TModel>
    where TConverter : IModelToInputConverter<TModel, T>
{
    [SupplyParameterFromQuery]
    public int? Page { get; set; }

    [SupplyParameterFromQuery]
    public int? PageSize { get; set; }

    [SupplyParameterFromQuery]
    public string? SearchTerm { get; set; }
    
    [Inject]
    public required TViewModel ViewModel { get; set; }
    
    [Inject]
    public required NavigationManager Nav { get; set; }
    
    [Inject]
    public required IDialogService DialogService { get; set; }
    
    [Parameter]
    public required string PageRoute { get; set; }
    
    [Parameter]
    public required string Title { get; set; }
    
    [Parameter]
    public required string ModelDisplayName { get; set; }
    
    [Parameter]
    public required RenderFragment Columns { get; set; }
    
    [Parameter]
    public RenderFragment<T>? EditAction { get; set; }
    
    [Parameter]
    public RenderFragment<T>? DeleteAction { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = "Search...";

    private MudDataGrid<T>? Grid;
    private bool _updatingParameters = false;
    private bool _forceReload = false;

    protected override async Task OnParametersSetAsync()
    {
        // If parameters are missing, set defaults and update URL
        if (Page == null || PageSize == null || SearchTerm is null)
        {
            UpdatePageWithUrl(Page ?? 1, PageSize ?? 10, SearchTerm ?? string.Empty, replace: true);
            return; // URL change will trigger OnParametersSetAsync again with proper values
        }

        _updatingParameters = true;
        if (Grid != null) await Grid.ReloadServerData();
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
            Items = ViewModel.Page?.Items.Select(TConverter.Convert) ?? Enumerable.Empty<T>(),
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
        (page, pageSize) = await ViewModel.SetPage(page, pageSize, searchTerm, _forceReload);

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
    
    public async Task EditItem(T inputModel)
    {
        var parameters = new DialogParameters<TEditModal>
        {
            { x => x.Model, inputModel },
        };

        var dialog = await DialogService.ShowAsync<TEditModal>("Edit Item", parameters);
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: T model })
        {
            await ViewModel.UpdateItem(TConverter.Convert(model));
            await RefreshGridData();
        }
    }

    public async Task RefreshGridData()
    {
        // Refresh data
        _forceReload = true;
        await (Grid?.ReloadServerData() ?? Task.CompletedTask);
        _forceReload = false;
    }

    public async Task DeleteItem(T inputModel)
    {
        var dialog = await DialogService.ShowAsync<ConfirmDeleteModal>("Delete Item");
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: bool confirm })
        {
            await ViewModel.DeleteItem(TConverter.Convert(inputModel));
            await RefreshGridData();
        }
    }
}