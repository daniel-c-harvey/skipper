using Microsoft.AspNetCore.Components;
using Models.Shared.Common;
using MudBlazor;
using NetBlocks.Models;
using SkipperModels.Converters;
using SkipperModels.InputModels;
using SkipperModels.Models;
using SkipperWeb.ApiClients;

namespace SkipperWeb.Components.Pages.Maintenance.SlipReservations.Forms;

public partial class ChooseSlip : ComponentBase
{
    [Inject]
    public required SlipClient SlipClient { get; set; }

    [Inject]
    public required IDialogService DialogService { get; set; }

    private IEnumerable<SlipInputModel> _slips = [];    
    private string _slipSearch = string.Empty;
    private Result? _errorResult = null;
    
    // Pagination state for vessels
    private int _currentSlipsPage = 1;
    private bool _hasMoreSlips = false;
    private bool _loadingMoreSlips = false;
    private bool _initialSlipsLoaded = false;

    protected override async Task OnInitializedAsync()
    {
        await GetSlips();
    }
    
    private async Task GetSlips(bool resetPagination = false)
    {
        // Reset pagination when search changes or explicitly requested
        if (!_initialSlipsLoaded || resetPagination)
        {
            _currentSlipsPage = 1;
            _slips = [];
            _initialSlipsLoaded = true;
        }

        var slipResults = await SlipClient.GetByPage(new PagedQuery()
        {
            Page = _currentSlipsPage,
            PageSize = 20, // Smaller page size for better UX
            Search = _slipSearch
        });

        if (!slipResults.Success || slipResults.Value is null)
        {
            _errorResult = Result.From(slipResults);
            return;
        }

        var newSlips = slipResults.Value.Items.Select(SlipModelToInputConverter.Convert);
        
        // If it's the first page or a new search, replace the vessels
        if (_currentSlipsPage == 1)
        {
            _slips = newSlips;
        }
        else
        {
            // Append new vessels to existing ones
            _slips = _slips.Concat(newSlips);
        }
        
        _hasMoreSlips = slipResults.Value.HasNextPage;
    }

    private async Task LoadMoreSlips()
    {
        if (_loadingMoreSlips || !_hasMoreSlips) return;

        _loadingMoreSlips = true;
        StateHasChanged();

        try
        {
            _currentSlipsPage++;
            await GetSlips();
        }
        finally
        {
            _loadingMoreSlips = false;
            StateHasChanged();
        }
    }

    private async Task OnSlipsSearchChanged()
    {
        // Reset pagination when search changes
        await GetSlips(resetPagination: true);
        StateHasChanged();
    }
    
    private async Task AddSlip()
    {
        var options = new DialogOptions { CloseButton = true, FullWidth = true };
        var dialog = await DialogService.ShowAsync<AddSlipModal>("Add Slip", options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SlipInputModel slip })
        {
            var addResult = await SlipClient.Update(SlipModelToInputConverter.Convert(slip));
            if (addResult is { Success: true, Value: SlipModel newVessel })
            {
                // Successfully added, now make this new vessel the selected value
                _slips= _slips.Append(slip);
                Context.Slip = slip;
            }
        }
    }    
} 