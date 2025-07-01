using Microsoft.AspNetCore.Mvc;
using SkipperData.Managers;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperAPI.Controllers
{
    [ApiController] 
    public abstract class BaseModelController<TEntity, TModel> : ControllerBase 
    where TEntity : class, IEntity<TEntity, TModel>, new()
    where TModel : class, IModel<TModel, TEntity>, new()
    {
        protected readonly IManager<TEntity, TModel> Manager;
        private readonly Dictionary<string, Expression<Func<TEntity, object>>> _sortExpressions;

        protected BaseModelController(IManager<TEntity, TModel> manager)
        {
            Manager = manager;
            _sortExpressions = new Dictionary<string, Expression<Func<TEntity, object>>>(StringComparer.OrdinalIgnoreCase);
            
            // Initialize base sort expressions using nameof()
            _sortExpressions[nameof(IEntity<TEntity, TModel>.Id)] = e => e.Id;
            _sortExpressions[nameof(IEntity<TEntity, TModel>.CreatedAt)] = e => e.CreatedAt;
            _sortExpressions[nameof(IEntity<TEntity, TModel>.UpdatedAt)] = e => e.UpdatedAt;
        }

        /// <summary>
        /// Get entities - returns existing PagedResult<T> directly
        /// </summary>
        [HttpGet]
        public virtual async Task<ActionResult<ApiResultDto<PagedResult<TModel>>>> Get([FromQuery] PagedQuery query)
        {
            var paging = new PagingParameters<TEntity>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                OrderBy = GetSortExpression(query.Sort),
                IsDescending = query.Desc
            }; 
            
            var predicate = BuildSearchPredicate(query.Search);
            var pageResult = await Manager.GetPage(predicate, paging);
            
            var result = ApiResult<PagedResult<TModel>>.From(pageResult);
            result.Value = new PagedResult<TModel>()
            {
                Items = pageResult?.Value?.Items.Select(TEntity.CreateModel) ?? [],
                Page = pageResult?.Value?.Page ?? 1,
                PageSize = pageResult?.Value?.PageSize ?? 0,
                TotalCount = pageResult?.Value?.TotalCount ?? 0
            };
            ApiResultDto<PagedResult<TModel>> dto = new(result);
            
            return result.Success ? Ok(dto) : StatusCode(500, dto);
        }

        /// <summary>
        /// Get entity by ID
        /// </summary>
        [HttpGet("{id:long}")]
        public virtual async Task<ActionResult<ApiResultDto<TModel>>> Get(long id)
        {
            var pageResult = await Manager.GetPage(e => e.Id == id, new PagingParameters<TEntity> { PageSize = 1 });
            
            ApiResult<TModel> result = ApiResult<TModel>.From(pageResult);
            var val = pageResult?.Value?.Items.FirstOrDefault();
            if (val != null) result.Value = TEntity.CreateModel(val);
            
            ApiResultDto<TModel> dto = new(result);
            
            if (!result.Success) { return StatusCode(500, dto); }
            
            var entity = result.Value;
            return entity == null ? NotFound(dto) : Ok(dto);
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
            var addResult = await Manager.Add(TModel.CreateEntity(model));
            ApiResult<TModel> result = ApiResult<TModel>.From(addResult);
            result.Value = model;
            
            ApiResultDto<TModel> dto = new(result);
            
            return result.Success ? 
                CreatedAtAction(nameof(Get), new { id = model.Id }, dto) :
                StatusCode(500, dto);
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