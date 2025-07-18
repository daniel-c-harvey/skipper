using AuthBlocksModels.ApiModels;
using NetBlocks.Models;

namespace AuthBlocksWeb.ApiClients;

public interface IAuthApiClient
{
    Task<LoginResult<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResult<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResult<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResult> LogoutAsync(RefreshTokenRequest request);
    Task<ApiResult<UserInfo>> GetCurrentUserAsync();
    Task<ApiResult<List<RoleInfo>>> GetRolesAsync();
} 