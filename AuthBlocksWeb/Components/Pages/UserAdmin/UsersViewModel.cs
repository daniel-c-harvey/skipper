using AuthBlocksModels.Models;
using AuthBlocksWeb.ApiClients;
using Web.Shared.Maintenance.Entities;

namespace AuthBlocksWeb.Components.Pages.UserAdmin;

public class UsersViewModel : ModelPageViewModel<UserModel, UsersClient, UsersClientConfig>
{
    protected RoleClient RoleClient;
    
    public UsersViewModel(UsersClient client, RoleClient roleClient) : base(client)
    {
        this.RoleClient = roleClient;
    }


    public override async Task DeleteItem(UserModel model)
    {
        await base.DeleteItem(model);
    }
}