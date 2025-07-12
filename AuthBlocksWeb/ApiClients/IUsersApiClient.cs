using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using NetBlocks.Models;
using Web.Shared.ApiClients;

namespace AuthBlocksWeb.ApiClients;

public interface IUsersApiClient : IModelControllerClient<UserModel, ApplicationUser>
{
    Task<ApiResult<List<UserInfo>>> GetUserInfoAsync();
    Task<ApiResult<UserInfo>> GetUserAsync(long id);
    Task<ApiResult> AddUserToRoleAsync(long userId, string role);
    Task<ApiResult> RemoveUserFromRoleAsync(long userId, string role);
} 