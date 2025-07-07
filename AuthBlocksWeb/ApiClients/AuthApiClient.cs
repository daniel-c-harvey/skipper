using System.Text;
using System.Text.Json;
using AuthBlocksWeb.Models.Api;
using AuthBlocksWeb.Services;

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

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/login", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(responseJson, _jsonOptions);
        
        if (result?.Success == true && result.Data != null)
        {
            // Store tokens when login is successful
            await _tokenService.SetTokensAsync(result.Data.AccessToken, result.Data.RefreshToken);
        }

        return result ?? new ApiResponse<AuthResponse> { Success = false, Message = "Failed to parse response" };
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/register", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(responseJson, _jsonOptions);
        
        if (result?.Success == true && result.Data != null)
        {
            // Store tokens when registration is successful
            await _tokenService.SetTokensAsync(result.Data.AccessToken, result.Data.RefreshToken);
        }

        return result ?? new ApiResponse<AuthResponse> { Success = false, Message = "Failed to parse response" };
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/refresh", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(responseJson, _jsonOptions);
        
        if (result?.Success == true && result.Data != null)
        {
            // Update stored tokens when refresh is successful
            await _tokenService.SetTokensAsync(result.Data.AccessToken, result.Data.RefreshToken);
        }

        return result ?? new ApiResponse<AuthResponse> { Success = false, Message = "Failed to parse response" };
    }

    public async Task<ApiResponse> LogoutAsync(RefreshTokenRequest request)
    {
        // Add authorization header for logout
        var token = await _tokenService.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/logout", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ApiResponse>(responseJson, _jsonOptions);
        
        if (result?.Success == true)
        {
            // Clear tokens when logout is successful
            await _tokenService.ClearTokensAsync();
        }

        // Clear authorization header
        _httpClient.DefaultRequestHeaders.Authorization = null;

        return result ?? new ApiResponse { Success = false, Message = "Failed to parse response" };
    }

    public async Task<ApiResponse<UserInfo>> GetCurrentUserAsync()
    {
        // Add authorization header
        var token = await _tokenService.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            return new ApiResponse<UserInfo> { Success = false, Message = "No access token available" };
        }

        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("api/auth/me");
        var responseJson = await response.Content.ReadAsStringAsync();

        // Clear authorization header
        _httpClient.DefaultRequestHeaders.Authorization = null;

        var result = JsonSerializer.Deserialize<ApiResponse<UserInfo>>(responseJson, _jsonOptions);
        return result ?? new ApiResponse<UserInfo> { Success = false, Message = "Failed to parse response" };
    }
} 