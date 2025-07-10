using System.Net.Http.Json;
using System.Text.Json;
using AuthBlocksModels.ApiModels;
using AuthBlocksWeb.Services;
using NetBlocks.Models;

namespace AuthBlocksWeb.ApiClients;

public class RolesApiClient : IRolesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly JsonSerializerOptions _jsonOptions;

    public RolesApiClient(HttpClient httpClient, ITokenService tokenService)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
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

    public async Task<ApiResult<RoleInfo>> GetRoleAsync(long id)
    {
        try
        {
            // Add authorization header
            var token = await _tokenService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return ApiResult<RoleInfo>.CreateFailResult("No access token available");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/roles/{id}");
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<RoleInfo>>(_jsonOptions);

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (dtoResult == null) return ApiResult<RoleInfo>.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<RoleInfo>.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult<RoleInfo>> CreateRoleAsync(CreateRoleRequest request)
    {
        try
        {
            // Add authorization header
            var token = await _tokenService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return ApiResult<RoleInfo>.CreateFailResult("No access token available");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("api/roles", request);
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<RoleInfo>>(_jsonOptions);

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (dtoResult == null) return ApiResult<RoleInfo>.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<RoleInfo>.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult<RoleInfo>> UpdateRoleAsync(long id, UpdateRoleRequest request)
    {
        try
        {
            // Add authorization header
            var token = await _tokenService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return ApiResult<RoleInfo>.CreateFailResult("No access token available");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsJsonAsync($"api/roles/{id}", request);
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<RoleInfo>>(_jsonOptions);

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (dtoResult == null) return ApiResult<RoleInfo>.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<RoleInfo>.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult> DeleteRoleAsync(long id)
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

            var response = await _httpClient.DeleteAsync($"api/roles/{id}");
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