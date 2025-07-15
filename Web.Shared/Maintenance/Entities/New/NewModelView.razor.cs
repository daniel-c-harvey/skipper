using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Models.Shared.Converters;
using Models.Shared.InputModels;
using Models.Shared.Models;
using MudBlazor;
using NetBlocks.Models;
using Web.Shared.ApiClients;

namespace Web.Shared.Maintenance.Entities.New;

public partial class NewModelView<TModel, TInputModel, TClient, TClientConfig, TConverter> : ComponentBase
where TModel : class, IModel, new()
where TInputModel : class, IInputModel, new()
where TClient : ModelClient<TModel, TClientConfig>
where TClientConfig : ModelClientConfig
where TConverter : IModelToInputConverter<TModel, TInputModel>
{
    [SupplyParameterFromForm]
    public TInputModel Input { get; set; } = new();
    
    [Inject]
    public required TClient Client { get; set; }
    
    [Inject]
    public required NavigationManager Navigation { get; set; }
    
    [Inject]
    public required IDialogService DialogService { get; set; }
    
    [Parameter]
    public required string ModelDisplayName { get; set; }
    
    [Parameter]
    public required string PageRoute { get; set; }
    
    [Parameter]
    public required RenderFragment<TInputModel> ChildContent { get; set; }
    
    public async Task Post(EditContext editContext)
    {
        Task<NetBlocks.Models.Result> resultTask = Task.Run(async () =>
        {
            TModel newVessel = TConverter.Convert(Input);

            ApiResult<TModel> addResult = await Client.Update(newVessel);
            return addResult.Success ? NetBlocks.Models.Result.CreatePassResult() : NetBlocks.Models.Result.From(addResult);
        });

        var parameters = new DialogParameters<ModelSubmittedModal>
        {
            { x => x.ResultTask, resultTask },
            { x => x.ModelName, ModelDisplayName}
        };
        var options = new DialogOptions { CloseButton = true, FullWidth = true };

        // Post and show results
        var dialog = await DialogService.ShowAsync<ModelSubmittedModal>($"Submit {ModelDisplayName} Result", parameters, options);
        var dialogResult = await dialog.Result;

        if(dialogResult != null && 
           !dialogResult.Canceled && 
           dialogResult.Data is NetBlocks.Models.Result result &&
           result.Success)
        {
            Navigation.NavigateTo($"/{PageRoute}", forceLoad: true);
        }
    }
}