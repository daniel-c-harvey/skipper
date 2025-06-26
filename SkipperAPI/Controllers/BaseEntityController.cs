using Microsoft.AspNetCore.Mvc;
using SkipperData.Managers;
using System.Linq.Expressions;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperAPI.Controllers
{
    [ApiController] 
    public abstract class BaseEntityController<T> : ControllerBase where T : BaseEntity, new()
    {
        protected readonly IManager<T> Manager;
        private readonly Dictionary<string, Expression<Func<T, object>>> _sortExpressions;

        protected BaseEntityController(IManager<T> manager)
        {
            Manager = manager;
            _sortExpressions = new Dictionary<string, Expression<Func<T, object>>>(StringComparer.OrdinalIgnoreCase);
            
            // Initialize base sort expressions using nameof()
            _sortExpressions[nameof(BaseEntity.Id)] = e => e.Id;
            _sortExpressions[nameof(BaseEntity.CreatedAt)] = e => e.CreatedAt;
            _sortExpressions[nameof(BaseEntity.UpdatedAt)] = e => e.UpdatedAt;
        }

        /// <summary>
        /// Get entities - returns existing PagedResult<T> directly
        /// </summary>
        [HttpGet]
        public virtual async Task<ActionResult<ApiResultDto<PagedResult<T>>>> Get([FromQuery] PagedQuery query)
        {
            var paging = new PagingParameters<T>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                OrderBy = GetSortExpression(query.Sort),
                IsDescending = query.Desc
            };
            
            var predicate = BuildSearchPredicate(query.Search);
            var pageResult = await Manager.GetPage(predicate, paging);
            
            var result = ApiResult<PagedResult<T>>.From(pageResult);
            result.Value = pageResult.Value;
            ApiResultDto<PagedResult<T>> dto = new(result);
            
            return result.Success ? Ok(dto) : StatusCode(500, dto);
        }

        /// <summary>
        /// Get entity by ID
        /// </summary>
        [HttpGet("{id:long}")]
        public virtual async Task<ActionResult<ApiResultDto<T>>> Get(long id)
        {
            var pageResult = await Manager.GetPage(e => e.Id == id, new PagingParameters<T> { PageSize = 1 });
            
            ApiResult<T> result = ApiResult<T>.From(pageResult); 
            result.Value = pageResult?.Value?.Items.FirstOrDefault();
            
            ApiResultDto<T> dto = new(result);
            
            if (!result.Success) { return StatusCode(500, dto); }
            
            var entity = result.Value;
            return entity == null ? NotFound(dto) : Ok(dto);
        }

        [HttpGet("count")]
        public virtual async Task<ActionResult<ApiResultDto<int>>> GetCount([FromQuery] PagedQuery query)
        {
            var paging = new PagingParameters<T>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                OrderBy = GetSortExpression(query.Sort),
                IsDescending = query.Desc
            };
            var predicate = BuildSearchPredicate(query.Search);
            var countResult = await Manager.GetPageCount(predicate, paging);
            ApiResult<int> result = ApiResult<int>.From(countResult);
            result.Value = countResult.Value;
            
            ApiResultDto<int> dto = new(result);

            return result.Success ? 
                Ok(dto) : 
                StatusCode(500, dto);
        }

        /// <summary>
        /// Create entity - accepts domain entity directly
        /// </summary>
        [HttpPost]
        public virtual async Task<ActionResult<ApiResultDto<T>>> Post([FromBody] T entity)
        {
            var addResult = await Manager.Add(entity);
            ApiResult<T> result = ApiResult<T>.From(addResult);
            result.Value = entity;
            
            ApiResultDto<T> dto = new(result);

            await Task.Delay(1000);            
            
            return result.Success ? 
                CreatedAtAction(nameof(Get), new { id = entity.Id }, dto) :
                StatusCode(500, dto);
        }

        /// <summary>
        /// Override in derived classes for entity-specific search
        /// </summary>
        protected virtual Expression<Func<T, bool>> BuildSearchPredicate(string? search) 
            => e => true;

        /// <summary>
        /// Add a sort expression using nameof() for type safety
        /// </summary>
        protected void AddSortExpression(string propertyName, Expression<Func<T, object>> expression)
        {
            _sortExpressions[propertyName] = expression;
        }

        /// <summary>
        /// Get sort expression by property name
        /// </summary>
        protected virtual Expression<Func<T, object>>? GetSortExpression(string? sort) 
        {
            if (string.IsNullOrEmpty(sort))
                return null;
                
            return _sortExpressions.TryGetValue(sort, out var expression) ? expression : null;
        }
    }
} 