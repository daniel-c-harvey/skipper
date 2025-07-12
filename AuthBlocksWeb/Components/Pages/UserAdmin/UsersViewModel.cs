using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using AuthBlocksWeb.ApiClients;
using Web.Shared.Maintenance.Entities;

namespace AuthBlocksWeb.Components.Pages.UserAdmin;

public class UsersViewModel : ModelPageViewModel<UserModel, ApplicationUser, UsersClient, UserClientConfig>
{
    public UsersViewModel(UsersClient client) : base(client)
    {
    }

    public override async Task DeleteItem(UserModel model)
    {
        await base.DeleteItem(model);
    }
}