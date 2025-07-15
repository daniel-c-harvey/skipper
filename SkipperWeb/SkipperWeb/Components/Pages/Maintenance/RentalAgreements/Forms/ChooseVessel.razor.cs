using Microsoft.AspNetCore.Components;
using Models.Shared.Common;
using MudBlazor;
using NetBlocks.Models;
using SkipperModels.Converters;
using SkipperModels.InputModels;
using SkipperModels.Models;
using SkipperWeb.ApiClients;

namespace SkipperWeb.Components.Pages.Maintenance.RentalAgreements.Forms;

public partial class ChooseVessel : ComponentBase
{
    [Inject]
    public required VesselClient VesselClient { get; set; }
    
    [Inject]
    public required IDialogService DialogService { get; set; }
    
    private IEnumerable<VesselInputModel> _vessels = [];
    private string _vesselSearch = string.Empty;
    private Result? _errorResult = null;

    // Pagination state for vessels
    private int _currentVesselPage = 1;
    private bool _hasMoreVessels = false;
    private bool _loadingMoreVessels = false;
    private bool _initialVesselsLoaded = false;

    protected override async Task OnInitializedAsync()
    {
        await GetVessels();
    }
    
    private async Task GetVessels(bool resetPagination = false)
    {
        // Reset pagination when search changes or explicitly requested
        if (!_initialVesselsLoaded || resetPagination)
        {
            _currentVesselPage = 1;
            _vessels = [];
            _initialVesselsLoaded = true;
        }

        var vesselResults = await VesselClient.GetByPage(new PagedQuery()
        {
            Page = _currentVesselPage,
            PageSize = 20, // Smaller page size for better UX
            Search = _vesselSearch
        });

        if (!vesselResults.Success || vesselResults.Value is null)
        {
            _errorResult = Result.From(vesselResults);
            return;
        }

        var newVessels = vesselResults.Value.Items.Select(VesselModelToInputConverter.Convert);
        
        // If it's the first page or a new search, replace the vessels
        if (_currentVesselPage == 1)
        {
            _vessels = newVessels;
        }
        else
        {
            // Append new vessels to existing ones
            _vessels = _vessels.Concat(newVessels);
        }
        
        _hasMoreVessels = vesselResults.Value.HasNextPage;
    }

    private async Task LoadMoreVessels()
    {
        if (_loadingMoreVessels || !_hasMoreVessels) return;

        _loadingMoreVessels = true;
        StateHasChanged();

        try
        {
            _currentVesselPage++;
            await GetVessels();
        }
        finally
        {
            _loadingMoreVessels = false;
            StateHasChanged();
        }
    }

    private async Task OnVesselSearchChanged()
    {
        // Reset pagination when search changes
        await GetVessels(resetPagination: true);
        StateHasChanged();
    }
    
    private async Task AddVessel()
    {
        var options = new DialogOptions { CloseButton = true, FullWidth = true};
        var dialog = await DialogService.ShowAsync<AddVesselModal>("Add Vessel", options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: VesselInputModel vessel })
        {
            var addResult = await VesselClient.Update(VesselModelToInputConverter.Convert(vessel));
            if (addResult is { Success: true, Value: VesselModel newVessel })
            {
                // Successfully added, now make this new vessel the selected value
                _vessels = _vessels.Append(vessel);
                Context.Vessel = vessel;
            }
        }
    }
}