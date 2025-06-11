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
            
            var uri = QueryHelpers.AddQueryString($"/api/{config.ControllerName}", queryMap);
            
            return await http.GetFromJsonAsync<PagedResult<TEntity>>(uri) 
                   ?? throw new HttpRequestException(HttpRequestError.InvalidResponse);
        }
        catch (Exception e)
        {
            // TODO better error handling
            Console.WriteLine(e);
            throw;
        }
    }
}