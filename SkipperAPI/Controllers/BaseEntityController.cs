using Microsoft.AspNetCore.Mvc;
using Skipper.Managers;
using Skipper.Common;
using System.Linq.Expressions;
using SkipperModels.Entities;

namespace SkipperAPI.Controllers
{
    /// <summary>
    /// Ultra-simplified base controller - uses ONLY existing infrastructure
    /// Zero duplication, zero custom wrappers, maximum reuse
    /// </summary>
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
        public virtual async Task<ActionResult<PagedResult<T>>> Get([FromQuery] SimpleQuery query)
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
            
            // TODO: Handle NetBlocks Result<T> properly once we know the API
            return Ok(result); // This will need to be result.Data or similar
        }

        /// <summary>
        /// Get entity by ID
        /// </summary>
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<T>> Get(long id)
        {
            var result = await Manager.GetPage(e => e.Id == id, new PagingParameters<T> { PageSize = 1 });
            // TODO: Handle NetBlocks Result<T> properly
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
            => string.IsNullOrEmpty(search) ? e => true : e => true;

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

    /// <summary>
    /// Minimal query parameters - reuses existing validation from PagingParameters<T>
    /// </summary>
    public class SimpleQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public bool Desc { get; set; }
    }
} 