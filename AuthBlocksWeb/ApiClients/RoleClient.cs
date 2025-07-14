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

public class RoleClient : ModelControllerClient<RoleModel, ApplicationRole, RolesClientConfig>, IRoleApiClient
{
    private readonly ITokenService _tokenService;

    public RoleClient(RolesClientConfig config, IOptions<JsonSerializerOptions> options, ITokenService tokenService) : base(config, options)
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
    public override async Task<ApiResult<PagedResult<RoleModel>>> GetByPage(PagedQuery query)
    {
        if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult<PagedResult<RoleModel>>.From(error);
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

    public override async Task<ApiResult<RoleModel>> Update(RoleModel model)
    {
        if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult<RoleModel>.From(error);
        var result = await base.Update(model);
        ClearAuthorizationHeader();
        return result;
    }

    public override async Task<ApiResult> Delete(RoleModel model)
    {
        if (await AddAuthorizationHeader() is {Success: false} error) return ApiResult.From(error);
        var result = await base.Delete(model);
        ClearAuthorizationHeader();
        return result;
    }
}