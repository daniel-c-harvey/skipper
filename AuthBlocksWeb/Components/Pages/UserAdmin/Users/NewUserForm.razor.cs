using AuthBlocksModels.ApiModels;
using AuthBlocksWeb.ApiClients;
using AuthBlocksWeb.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace AuthBlocksWeb.Components.Pages.UserAdmin.Users;

public partial class NewUserForm : ComponentBase
{
    [SupplyParameterFromForm]
    public PendingRegistrationInputModel Input { get; set; } = new();
    
    [Inject]
    public required PendingRegistrationClient Client { get; set; }
    
    [Inject]
    public required NavigationManager Navigation { get; set; }
    
    [Inject]
    public required IDialogService DialogService { get; set; }

    private async Task CreatePendingRegistration(EditContext arg)
    {
        Task<RegistrationCreatedResult> resultTask = Task.Run(async () => await Client.CreatePendingRegistration(Input.Email, Navigation.ToAbsoluteUri("account/register").AbsoluteUri));

        var parameters = new DialogParameters<RegistrationSubmittedModal>
        {
            { x => x.ResultTask, resultTask },
        };
        var options = new DialogOptions { CloseButton = true, FullWidth = true };

        // Post and show results
        var dialog = await DialogService.ShowAsync<RegistrationSubmittedModal>($"Submit Account Registration Result", parameters, options);
        var dialogResult = await dialog.Result;

        if(dialogResult != null && 
           !dialogResult.Canceled && 
           dialogResult.Data is NetBlocks.Models.Result result &&
           result.Success)
        {
            // Navigation.NavigateTo($"/useradmin/users", forceLoad: true);
        }
    }
}