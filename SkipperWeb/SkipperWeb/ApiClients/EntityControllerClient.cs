using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;
using System.Text.Json;

namespace SkipperWeb.ApiClients;

public abstract class EntityControllerClient<TEntity, TConfig> : ApiClient<TConfig> 
where TConfig : EntityControllerClientConfig
where TEntity : BaseEntity, new()
{
    protected EntityControllerClient(TConfig config) : base(config) { }

    public async Task<PagedResult<TEntity>> GetByPage(PagedQuery query)
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
            
            var result = await http.GetFromJsonAsync<PagedResult<TEntity>>(uri)
                   ?? throw new HttpRequestException("Failed to deserialize response");

            return result;
        }
        catch (Exception e)
        {
            // TODO better error handling
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultContainer<TEntity>> Add(TEntity entity)
    {
        try
        {
            var response = await http.PostAsJsonAsync($"api/{config.ControllerName}", entity);
            if (response == null) throw new HttpRequestException(HttpRequestError.InvalidResponse);
            if (!response.IsSuccessStatusCode) throw new HttpRequestException(response.StatusCode.ToString());
            
            Stream newEntityStream = await response.Content.ReadAsStreamAsync();
            
            TEntity newEntity = await JsonSerializer.DeserializeAsync<TEntity>(newEntityStream) 
                ?? throw new HttpRequestException("Failed to deserialize response");

            return new ResultContainer<TEntity>(newEntity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultContainer<TEntity>.CreateFailResult(e.Message);
        }
    }
}