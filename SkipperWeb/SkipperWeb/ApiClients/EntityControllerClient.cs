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
            
            var result = await http.GetFromJsonAsync<ApiResult<PagedResult<TEntity>>>(uri)
                   ?? throw new HttpRequestException("Failed to deserialize response");

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ApiResult<PagedResult<TEntity>>.CreateFailResult(e.Message);
        }
    }

    public async Task<ApiResult<TEntity>> Add(TEntity entity)
    {
        try
        {
            var response = await http.PostAsJsonAsync($"api/{config.ControllerName}", entity);
            if (response == null) throw new HttpRequestException(HttpRequestError.InvalidResponse);
            
            ApiResultDto<TEntity> newEntityResult = await response.Content.ReadFromJsonAsync<ApiResultDto<TEntity>>() //await JsonSerializer.DeserializeAsync<ApiResultDto<TEntity>>(newEntityStream)
                ?? throw new HttpRequestException("Failed to deserialize response");

            return newEntityResult.From();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ApiResult<TEntity>.CreateFailResult(e.Message);
        }
    }
}