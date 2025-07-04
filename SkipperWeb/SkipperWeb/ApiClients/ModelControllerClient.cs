using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperModels.Models;
using System.Text.Json;


namespace SkipperWeb.ApiClients;

public abstract class ModelControllerClient<TModel, TEntity, TConfig> : ApiClient<TConfig>
where TConfig : ModelControllerClientConfig
where TModel : class, IModel<TModel, TEntity>, new()
where TEntity : class, IEntity<TEntity, TModel>, new()
{
    private readonly JsonSerializerOptions _options;
    protected ModelControllerClient(TConfig config, IOptions<JsonSerializerOptions> options) : base(config) 
    {
        _options = options.Value;
    }

    public async Task<ApiResult<PagedResult<TModel>>> GetByPage(PagedQuery query)
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
            
            var result = await http.GetFromJsonAsync<ApiResultDto<PagedResult<TModel>>>(uri, _options)
                   ?? throw new HttpRequestException("Failed to deserialize response");

            return result.From();
        }
        catch (Exception e)
        {
            return ApiResult<PagedResult<TModel>>.CreateFailResult(e.Message);
        }
    }

    public async Task<ApiResult<ItemCount>> GetPageCount(PagedQuery query)
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

            var result = await http.GetFromJsonAsync<ApiResultDto<ItemCount>>(uri, _options)
                   ?? throw new HttpRequestException("Failed to deserialize response");

            return result.From();
        }
        catch (Exception e)
        {
            return ApiResult<ItemCount>.CreateFailResult(e.Message);
        }
    }

    public async Task<ApiResult<TModel>> Add(TModel entity)
    {
        try
        {
            var response = await http.PostAsJsonAsync($"api/{config.ControllerName}/new", entity, _options);
            if (response == null) throw new HttpRequestException(HttpRequestError.InvalidResponse);
            
            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<TModel>>(_options)
                ?? throw new HttpRequestException("Failed to deserialize response");

            return result.From();
        }
        catch (Exception e)
        {
            return ApiResult<TModel>.CreateFailResult(e.Message);
        }
    }

    public async Task<ApiResult<TModel>> Update(TModel model)
    {
        try
        {
            var response = await http.PostAsJsonAsync($"api/{config.ControllerName}", model, _options);
            if (response == null) throw new HttpRequestException(HttpRequestError.InvalidResponse);
            
            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<TModel>>(_options)
                         ?? throw new HttpRequestException("Failed to deserialize response");

            return result.From();
        }
        catch (Exception e)
        {
            return ApiResult<TModel>.CreateFailResult(e.Message);
        }
    }
}