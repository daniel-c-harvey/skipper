using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Models.Shared.Common;
using Models.Shared.Entities;
using Models.Shared.Models;
using NetBlocks.Models;

namespace Web.Shared.ApiClients;

public abstract class ModelControllerClient<TModel, TEntity, TConfig> : ApiClient<TConfig>, IModelControllerClient<TModel, TEntity> 
    where TConfig : ModelControllerClientConfig
where TModel : class, IModel<TModel, TEntity>, new()
where TEntity : class, IEntity<TEntity, TModel>, new()
{
    protected readonly JsonSerializerOptions Options;
    protected ModelControllerClient(TConfig config, IOptions<JsonSerializerOptions> options) : base(config) 
    {
        Options = options.Value;
    }

    public virtual async Task<ApiResult<PagedResult<TModel>>> GetByPage(PagedQuery query)
    {
        try
        {
            var queryMap = new Dictionary<string, string?>
            {
                { nameof(query.Page).ToLower(), query.Page.ToString() },
                { nameof(query.PageSize).ToLower(), query.PageSize.ToString() },
                { nameof(query.Search).ToLower(), query.Search },
                { nameof(query.Sort).ToLower(), query.Sort },
                { nameof(query.Desc).ToLower(), query.Desc.ToString() }
            };
            
            var uri = QueryHelpers.AddQueryString($"api/{config.ControllerName}", queryMap);
            
            var result = await http.GetFromJsonAsync<ApiResultDto<PagedResult<TModel>>>(uri, Options)
                   ?? throw new HttpRequestException("Failed to deserialize response");

            return result.From();
        }
        catch (Exception e)
        {
            return ApiResult<PagedResult<TModel>>.CreateFailResult(e.Message);
        }
    }

    public virtual async Task<ApiResult<ItemCount>> GetPageCount(PagedQuery query)
    {
        try
        { 
            var queryMap = new Dictionary<string, string?>
                {
                    { nameof(query.Page).ToLower(), query.Page.ToString() },
                    { nameof(query.PageSize).ToLower(), query.PageSize.ToString() },
                    { nameof(query.Search).ToLower(), query.Search },
                    { nameof(query.Sort).ToLower(), query.Sort },
                    { nameof(query.Desc).ToLower(), query.Desc.ToString() }
                };

            var uri = QueryHelpers.AddQueryString($"api/{config.ControllerName}/count", queryMap);

            var result = await http.GetFromJsonAsync<ApiResultDto<ItemCount>>(uri, Options)
                   ?? throw new HttpRequestException("Failed to deserialize response");

            return result.From();
        }
        catch (Exception e)
        {
            return ApiResult<ItemCount>.CreateFailResult(e.Message);
        }
    }

    public virtual async Task<ApiResult<TModel>> Update(TModel model)
    {
        try
        {
            var response = await http.PostAsJsonAsync($"api/{config.ControllerName}", model, Options);
            if (response == null) throw new HttpRequestException(HttpRequestError.InvalidResponse);
            
            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<TModel>>(Options)
                ?? throw new HttpRequestException("Failed to deserialize response");

            return result.From();
        }
        catch (Exception e)
        {
            return ApiResult<TModel>.CreateFailResult(e.Message);
        }
    }
    
    public virtual async Task<ApiResult> Delete(TModel model)
    {
        try
        {
            var result = await http.DeleteFromJsonAsync<ApiResultDto>($"api/{config.ControllerName}/{model.Id}");
            if (result == null) throw new HttpRequestException(HttpRequestError.InvalidResponse);

            return result.From();
        }
        catch (Exception e)
        {
            return ApiResult.CreateFailResult(e.Message);
        }
    }
}