using System.Linq.Expressions;
using Data.Shared.Managers;
using Microsoft.AspNetCore.Mvc;
using Models.Shared.Common;
using Models.Shared.Converters;
using Models.Shared.Entities;
using Models.Shared.Models;
using NetBlocks.Models;

namespace API.Shared.Controllers
{
    [ApiController] 
    public abstract class BaseModelController<TEntity, TModel, TManager> : ControllerBase, IBaseModelController<TEntity, TModel> 
    where TEntity : class, IEntity, new()
    where TModel : class, IModel, new()
    where TManager : IManager<TEntity, TModel>
    {
        protected readonly TManager Manager;
        private readonly Dictionary<string, Expression<Func<TEntity, object>>> _sortExpressions;

        protected BaseModelController(TManager manager)
        {
            Manager = manager;
            _sortExpressions = new Dictionary<string, Expression<Func<TEntity, object>>>(StringComparer.OrdinalIgnoreCase);
            
            _sortExpressions[nameof(IEntity.Id)] = e => e.Id;
            _sortExpressions[nameof(IEntity.CreatedAt)] = e => e.CreatedAt;
            _sortExpressions[nameof(IEntity.UpdatedAt)] = e => e.UpdatedAt;
        }
        
        [HttpGet("all")]
        public async Task<ActionResult<ApiResultDto<IEnumerable<TModel>>>> GetAll()
        {
            var queryResult = await Manager.Get();
            
            var result = ApiResult<IEnumerable<TModel>>.From(queryResult);
            var dto = new ApiResultDto<IEnumerable<TModel>>(result);
            
            return !result.Success ? StatusCode(500, dto) : Ok(dto);
        }
        
        [HttpGet]
        public virtual async Task<ActionResult<ApiResultDto<PagedResult<TModel>>>> Get([FromQuery] PagedQuery query)
        {
            return await Get(query, Manager.GetPage);
        }

        protected async Task<ActionResult<ApiResultDto<PagedResult<TModel>>>> Get(PagedQuery query, Func<Expression<Func<TEntity,bool>>, 
                                                                                                            PagingParameters<TEntity>, 
                                                                                                            Task<ResultContainer<PagedResult<TModel>>>> getPageFunc)
        {
            var paging = new PagingParameters<TEntity>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                OrderBy = GetSortExpression(query.Sort),
                IsDescending = query.Desc
            }; 
            
            var predicate = BuildSearchPredicate(query.Search);
            var pageResult = await getPageFunc(predicate, paging);
            
            var result = ApiResult<PagedResult<TModel>>.From(pageResult);
            ApiResultDto<PagedResult<TModel>> dto = new(result);
            
            return result.Success ? Ok(dto) : StatusCode(500, dto);
        }

        /// <summary>
        /// Get entity by ID
        /// </summary>
        [HttpGet("{id:long}")]
        public virtual async Task<ActionResult<ApiResultDto<TModel>>> Get(long id)
        {
            var getResult = await Manager.GetById(id);
            
            ApiResult<TModel> result = ApiResult<TModel>.From(getResult);
            ApiResultDto<TModel> dto = new(result);
            
            if (!result.Success) { return StatusCode(500, dto); }
            
            return result.Value == null ? NotFound(dto) : Ok(dto);
        }

        [HttpGet("count")]
        public virtual async Task<ActionResult<ApiResultDto<ItemCount>>> GetCount([FromQuery] PagedQuery query)
        {
            var paging = new PagingParameters<TEntity>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                OrderBy = GetSortExpression(query.Sort),
                IsDescending = query.Desc
            };
            var predicate = BuildSearchPredicate(query.Search);
            var countResult = await Manager.GetPageCount(predicate, paging);
            ApiResult<ItemCount> result = ApiResult<ItemCount>.From(countResult);
            result.Value = new ItemCount() { Count = countResult.Value };
            
            ApiResultDto<ItemCount> dto = new(result);

            return result.Success ? 
                Ok(dto) : 
                StatusCode(500, dto);
        }
        
        /// <summary>
        /// Create entity - accepts domain entity directly
        /// </summary>
        [HttpPost]
        public virtual async Task<ActionResult<ApiResultDto<TModel>>> Post([FromBody] TModel model)
        {
            var existsResult = await Manager.Exists(model); 
            if (existsResult is not { Success: true }) { return StatusCode(500, ApiResult<TModel>.From(existsResult)); }

            // Add or update based on entity existence
            if (existsResult.Value)
            {
                // Entity exists, update
                var updateResult = await Manager.Update(model);
                ApiResult<TModel> result = ApiResult<TModel>.From(updateResult);
                result.Value = model;
                
                ApiResultDto<TModel> dto = new(result);
                
                return result.Success ? 
                    AcceptedAtAction(nameof(Post), new { id = model.Id }, dto) :
                    StatusCode(500, dto);
            }
            else
            {
                // model does NOT exist, insert
                var addResult = await Manager.Add(model);
                ApiResult<TModel> result = ApiResult<TModel>.From(addResult);
                result.Value = model;
                
                ApiResultDto<TModel> dto = new(result);
                
                return result.Success ? 
                    CreatedAtAction(nameof(Post), new { id = model.Id }, dto) :
                    StatusCode(500, dto);
            }
        }

        [HttpDelete("{id:long}")]
        public virtual async Task<ActionResult<ApiResultDto>> Delete(long id)
        {
            var result = await Manager.Delete(id);
            var dto = new ApiResultDto(ApiResult.From(result));
            
            return result.Success ?
                Ok(dto)
                : StatusCode(500, dto);
        }

        /// <summary>
        /// Override in derived classes for entity-specific search
        /// </summary>
        protected virtual Expression<Func<TEntity, bool>> BuildSearchPredicate(string? search) 
            => e => true;

        /// <summary>
        /// Add a sort expression using nameof() for type safety
        /// </summary>
        protected void AddSortExpression(string propertyName, Expression<Func<TEntity, object>> expression)
        {
            _sortExpressions[propertyName] = expression;
        }

        /// <summary>
        /// Get sort expression by property name
        /// </summary>
        protected virtual Expression<Func<TEntity, object>>? GetSortExpression(string? sort) 
        {
            if (string.IsNullOrEmpty(sort))
                return null;
                
            return _sortExpressions.TryGetValue(sort, out var expression) ? expression : null;
        }
    }
} 