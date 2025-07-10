using System.Net.Http.Json;
using System.Text.Json;
using AuthBlocksModels.ApiModels;
using AuthBlocksWeb.Services;
using NetBlocks.Models;

namespace AuthBlocksWeb.ApiClients;

public class UsersApiClient : IUsersApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly JsonSerializerOptions _jsonOptions;

    public UsersApiClient(HttpClient httpClient, ITokenService tokenService)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ApiResult<List<UserInfo>>> GetUsersAsync()
    {
        try
        {
            // Add authorization header
            var token = await _tokenService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return ApiResult<List<UserInfo>>.CreateFailResult("No access token available");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/users");
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<List<UserInfo>>>(_jsonOptions);

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (dtoResult == null) return ApiResult<List<UserInfo>>.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<List<UserInfo>>.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult<UserInfo>> GetUserAsync(long id)
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

            var response = await _httpClient.GetAsync($"api/users/{id}");
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

    public async Task<ApiResult<UserInfo>> UpdateUserAsync(long id, UpdateUserRequest request)
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

            var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", request);
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

    public async Task<ApiResult> DeleteUserAsync(long id)
    {
        try
        {
            // Add authorization header
            var token = await _tokenService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return ApiResult.CreateFailResult("No access token available");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/users/{id}");
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto>(_jsonOptions);

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (dtoResult == null) return ApiResult.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult> AddUserToRoleAsync(long id, UserRoleRequest request)
    {
        try
        {
            // Add authorization header
            var token = await _tokenService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return ApiResult.CreateFailResult("No access token available");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync($"api/users/{id}/roles", request);
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto>(_jsonOptions);

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (dtoResult == null) return ApiResult.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult> RemoveUserFromRoleAsync(long id, UserRoleRequest request)
    {
        try
        {
            // Add authorization header
            var token = await _tokenService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return ApiResult.CreateFailResult("No access token available");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/users/{id}/roles");
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto>(_jsonOptions);

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (dtoResult == null) return ApiResult.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult.CreateFailResult(ex.Message);
        }
    }
} 