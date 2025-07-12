using System.Net.Http.Json;
using System.Text.Json;
using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using AuthBlocksWeb.Services;
using Microsoft.Extensions.Options;
using Models.Shared.Common;
using Models.Shared.Models;
using NetBlocks.Models;
using Web.Shared.ApiClients;

namespace AuthBlocksWeb.ApiClients;

public class UsersClient : ModelControllerClient<UserModel, ApplicationUser, UserClientConfig>, IUsersApiClient
{
    private readonly ITokenService _tokenService;

    public UsersClient(UserClientConfig config, IOptions<JsonSerializerOptions> options, ITokenService tokenService) : base(config, options)
    {
        _tokenService = tokenService;
    }
    
    private async Task<Result> AddAuthorizationHeader()
    {
        // Add authorization header
        var token = await _tokenService.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            return Result.CreateFailResult("No access token available");
        }

        http.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        return Result.CreatePassResult();
    }
    
    private void ClearAuthorizationHeader()
    {
        http.DefaultRequestHeaders.Authorization = null;
    }

    /* Model Client Overrides */
    public override async Task<ApiResult<PagedResult<UserModel>>> GetByPage(PagedQuery query)
    {
        if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult<PagedResult<UserModel>>.From(error);
        var result = await base.GetByPage(query);
        ClearAuthorizationHeader();
        return result;
    }

    public override async Task<ApiResult<ItemCount>> GetPageCount(PagedQuery query)
    {
        if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult<ItemCount>.From(error);
        var result = await base.GetPageCount(query);
        ClearAuthorizationHeader();
        return result;
    }

    public override async Task<ApiResult<UserModel>> Update(UserModel model)
    {
        if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult<UserModel>.From(error);
        var result = await base.Update(model);
        ClearAuthorizationHeader();
        return result;
    }

    public override async Task<ApiResult> Delete(UserModel model)
    {
        if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult.From(error);
        var result = await base.Delete(model);
        ClearAuthorizationHeader();
        return result;
    }

    /* IUserApiClient additions */
    public async Task<ApiResult<List<UserInfo>>> GetUserInfoAsync()
    {
        try
        {
            if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult<List<UserInfo>>.From(error);

            var response = await http.GetAsync("api/users/list");
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<List<UserInfo>>>(Options);

            ClearAuthorizationHeader();

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
            if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult<UserInfo>.From(error);

            var response = await http.GetAsync($"api/users/info/{id}");
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto<UserInfo>>(Options);

            ClearAuthorizationHeader();

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
            if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult.From(error);

            var response = await http.DeleteAsync($"api/users/{id}");
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto>(Options);

            ClearAuthorizationHeader();

            if (dtoResult == null) return ApiResult.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult> AddUserToRoleAsync(long userId, string role)
    {
        try
        {
            if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult.From(error);

            var response = await http.PostAsJsonAsync($"api/users/{userId}/roles", new UserRoleRequest { RoleName = role });
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto>(Options);

            ClearAuthorizationHeader();

            if (dtoResult == null) return ApiResult.CreateFailResult("Failed to parse response");
            var result = dtoResult.From();

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApiResult> RemoveUserFromRoleAsync(long userId, string role)
    {
        try
        {
            if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult.From(error);

            var response = await http.DeleteAsync($"api/users/{userId}/roles");
            var dtoResult = await response.Content.ReadFromJsonAsync<ApiResultDto>(Options);

            ClearAuthorizationHeader();

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