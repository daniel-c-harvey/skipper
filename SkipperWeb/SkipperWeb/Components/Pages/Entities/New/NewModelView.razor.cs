using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using NetBlocks.Models;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;
using SkipperWeb.ApiClients;

namespace SkipperWeb.Components.Pages.Entities.New;

public partial class NewModelView<TModel, TEntity, TInputModel, TClient, TClientConfig> : ComponentBase
where TModel : class, IModel<TModel, TEntity>, new()
where TEntity : class, IEntity<TEntity, TModel>, new()
where TInputModel : class, IInputModel<TInputModel, TModel, TEntity>, new()
where TClient : ModelControllerClient<TModel, TEntity, TClientConfig>
where TClientConfig : ModelControllerClientConfig
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
        Task<Result> resultTask = Task.Run(async () =>
        {
            TModel newVessel = TInputModel.MakeModel(Input);

            ApiResult<TModel> addResult = await Client.Add(newVessel);
            return addResult.Success ? Result.CreatePassResult() : Result.From(addResult);
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
           dialogResult.Data is Result result &&
           result.Success)
        {
            Navigation.NavigateTo($"/{PageRoute}", forceLoad: true);
        }
    }
}