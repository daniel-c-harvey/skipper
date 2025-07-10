using AuthBlocksModels.ApiModels;
using NetBlocks.Models;

namespace AuthBlocksWeb.ApiClients;

public interface IRolesApiClient
{
    Task<ApiResult<List<RoleInfo>>> GetRolesAsync();
    Task<ApiResult<RoleInfo>> GetRoleAsync(long id);
    Task<ApiResult<RoleInfo>> CreateRoleAsync(CreateRoleRequest request);
    Task<ApiResult<RoleInfo>> UpdateRoleAsync(long id, UpdateRoleRequest request);
    Task<ApiResult> DeleteRoleAsync(long id);
} 