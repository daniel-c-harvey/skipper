using AuthBlocksWeb.Models.Api;

namespace AuthBlocksWeb.ApiClients;

public interface IAuthApiClient
{
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResponse> LogoutAsync(RefreshTokenRequest request);
    Task<ApiResponse<UserInfo>> GetCurrentUserAsync();
} 