using AuthBlocksModels.ApiModels;
using NetBlocks.Models;

namespace AuthBlocksWeb.ApiClients;

public interface IUsersApiClient
{
    Task<ApiResult<List<UserInfo>>> GetUsersAsync();
    Task<ApiResult<UserInfo>> GetUserAsync(long id);
    Task<ApiResult<UserInfo>> UpdateUserAsync(long id, UpdateUserRequest request);
    Task<ApiResult> DeleteUserAsync(long id);
    Task<ApiResult> AddUserToRoleAsync(long id, UserRoleRequest request);
    Task<ApiResult> RemoveUserFromRoleAsync(long id, UserRoleRequest request);
} 