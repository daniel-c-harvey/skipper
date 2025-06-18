using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;


namespace SkipperWeb.ApiClients;

public abstract class EntityControllerClient<TEntity, TConfig> : ApiClient<TConfig> 
where TConfig : EntityControllerClientConfig
where TEntity : BaseEntity, new()
{
    protected EntityControllerClient(TConfig config) : base(config) { }

    public async Task<ApiResult<PagedResult<TEntity>>> GetByPage(PagedQuery query)
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
            
            var result = await http.GetFromJsonAsync<ApiResultDto<PagedResult<TEntity>>>(uri)
                   ?? throw new HttpRequestException("Failed to deserialize response");

            return result.From();
        }
        catch (Exception e)
        {
            return ApiResult<PagedResult<TEntity>>.CreateFailResult(e.Message);
        }
    }

    public async Task<ApiResult<int>> GetPageCount(PagedQuery query)
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

            var result = await http.GetFromJsonAsync<ApiResultDto<int>>(uri)
                   ?? throw new HttpRequestException("Failed to deserialize response");

            return result.From();
        }
            catch (Exception e)
            {
                return ApiResult<int>.CreateFailResult(e.Message);
            }
        }

    public async Task<ApiResult<TEntity>> Add(TEntity entity)
    {
        try
        {
            var response = await http.PostAsJsonAsync($"api/{config.ControllerName}", entity);
            if (response == null) throw new HttpRequestException(HttpRequestError.InvalidResponse);
            
            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<TEntity>>()
                ?? throw new HttpRequestException("Failed to deserialize response");

            return result.From();
        }
        catch (Exception e)
        {
            return ApiResult<TEntity>.CreateFailResult(e.Message);
        }
    }
}