using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AuthBlocksModels.ApiModels;
using AuthBlocksWeb.Services;
using MudBlazor;
using NetBlocks.Models;

namespace AuthBlocksWeb.ApiClients;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthApiClient(HttpClient httpClient, ITokenService tokenService)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ApiResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<AuthResponse>>(_jsonOptions);
            
            if (dtoResult == null) return ApiResult<AuthResponse>.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();
            
            if (result is {Success: true, Value: AuthResponse authReponse} )
            {
                // Store tokens when login is successful
                await _tokenService.SetTokensAsync(authReponse.AccessToken, authReponse.RefreshToken);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<AuthResponse>.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<AuthResponse>>(_jsonOptions);

            if (dtoResult == null) return ApiResult<AuthResponse>.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            if (result is {Success: true, Value: AuthResponse authResponse} )
            {
                // Store tokens when registration is successful
                await _tokenService.SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);
            }

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<AuthResponse>.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<AuthResponse>>(_jsonOptions);

            if (dtoResult == null) return ApiResult<AuthResponse>.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            if (result is {Success: true, Value: AuthResponse authResponse} )
            {
                // Update stored tokens when refresh is successful
                await _tokenService.SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);
            }

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<AuthResponse>.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult> LogoutAsync(RefreshTokenRequest request)
    {
        try
        {
            // Add authorization header for logout
            var token = await _tokenService.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsJsonAsync("api/auth/logout", request);
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto>(_jsonOptions);

            if (dtoResult == null) return ApiResult.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            if (result.Success)
            {
                // Clear tokens when logout is successful
                await _tokenService.ClearTokensAsync();
            }

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult<UserInfo>> GetCurrentUserAsync()
    {
        try
        {
            // Add authorization header
            var token = await _tokenService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return ApiResult<UserInfo>.CreateFailResult("No access token available");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/auth/me");
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<UserInfo>>(_jsonOptions);

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (dtoResult == null) return ApiResult<UserInfo>.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<UserInfo>.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult<List<RoleInfo>>> GetRolesAsync()
    {
        try
        {
            // Add authorization header
            var token = await _tokenService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return ApiResult<List<RoleInfo>>.CreateFailResult("No access token available");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/roles");
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<List<RoleInfo>>>(_jsonOptions);

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (dtoResult == null) return ApiResult<List<RoleInfo>>.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<List<RoleInfo>>.CreateFailResult(ex.Message);
        }
    }
} 