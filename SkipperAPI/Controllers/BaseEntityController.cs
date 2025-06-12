using Microsoft.AspNetCore.Mvc;
using SkipperData.Managers;
using System.Linq.Expressions;
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
        public virtual async Task<ActionResult<PagedResult<T>>> Get([FromQuery] PagedQuery query)
        {
            var paging = new PagingParameters<T>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                OrderBy = GetSortExpression(query.Sort),
                IsDescending = query.Desc
            };

            var predicate = BuildSearchPredicate(query.Search);
            var result = await Manager.GetPage(predicate, paging);
            
            return Ok(result.Value); // This will need to be result.Data or similar
        }

        /// <summary>
        /// Get entity by ID
        /// </summary>
        [HttpGet("{id:long}")]
        public virtual async Task<ActionResult<T>> Get(long id)
        {
            var result = await Manager.GetPage(e => e.Id == id, new PagingParameters<T> { PageSize = 1 });
            var entity = result?.Value?.Items?.FirstOrDefault();
            return entity == null ? NotFound() : Ok(entity);
        }

        /// <summary>
        /// Create entity - accepts domain entity directly
        /// </summary>
        [HttpPost]
        public virtual async Task<ActionResult<T>> Post([FromBody] T entity)
        {
            var result = await Manager.Add(entity);
            // TODO: Handle NetBlocks Result properly once we know the API
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
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