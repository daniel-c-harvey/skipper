// using System.Linq.Expressions;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using SkipperModels.Entities;
// using SkipperModels;
// using Models.Shared.Common;
//
// namespace SkipperData.Data.Repositories;
//
// /// <summary>
// /// Repository for cross-type order operations that works with all order types
// /// without type invariance issues.
// /// </summary>
// public class CrossTypeOrderRepository
// {
//     private readonly SkipperContext _context;
//     private readonly ILogger<CrossTypeOrderRepository> _logger;
//
//     public CrossTypeOrderRepository(SkipperContext context, ILogger<CrossTypeOrderRepository> logger)
//     {
//         _context = context;
//         _logger = logger;
//     }
//
//     public async Task<OrderEntity<CustomerEntity>?> GetByOrderNumberAsync(string orderNumber)
//     {
//         return await _context.Orders
//             .Include(o => o.Customer)
//             .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && !o.IsDeleted);
//     }
//
//     public async Task<IEnumerable<OrderEntity<CustomerEntity>>> GetOrdersByCustomerAsync(long customerId)
//     {
//         return await _context.Orders
//             .Include(o => o.Customer)
//             .Where(o => o.CustomerId == customerId && !o.IsDeleted)
//             .OrderByDescending(o => o.OrderDate)
//             .ToListAsync();
//     }
//
//     public async Task<IEnumerable<OrderEntity<CustomerEntity>>> GetOrdersByStatusAsync(OrderStatus status)
//     {
//         return await _context.Orders
//             .Include(o => o.Customer)
//             .Where(o => o.Status == status && !o.IsDeleted)
//             .ToListAsync();
//     }
//
//     public async Task<IEnumerable<OrderEntity<CustomerEntity>>> GetOrdersByTypeAsync(OrderType orderType)
//     {
//         return await _context.Orders
//             .Include(o => o.Customer)
//             .Where(o => o.OrderType == orderType && !o.IsDeleted)
//             .OrderByDescending(o => o.OrderDate)
//             .ToListAsync();
//     }
//
//     public async Task<IEnumerable<OrderEntity<CustomerEntity>>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
//     {
//         return await _context.Orders
//             .Include(o => o.Customer)
//             .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && !o.IsDeleted)
//             .OrderByDescending(o => o.OrderDate)
//             .ToListAsync();
//     }
//
//     public async Task<IEnumerable<OrderEntity<CustomerEntity>>> GetOrdersByTypesAsync(params OrderType[] orderTypes)
//     {
//         return await _context.Orders
//             .Include(o => o.Customer)
//             .Where(o => orderTypes.Contains(o.OrderType) && !o.IsDeleted)
//             .OrderByDescending(o => o.OrderDate)
//             .ToListAsync();
//     }
//
//     public async Task<PagedResult<OrderEntity<CustomerEntity>>> GetOrdersPagedAsync(
//         Expression<Func<OrderEntity<CustomerEntity>, bool>>? predicate = null,
//         PagingParameters<OrderEntity<CustomerEntity>>? pagingParameters = null)
//     {
//         var query = _context.Orders
//             .Include(o => o.Customer)
//             .Where(o => !o.IsDeleted);
//
//         if (predicate != null)
//             query = query.Where(predicate);
//
//         var paging = pagingParameters ?? new PagingParameters<OrderEntity<CustomerEntity>>();
//         
//         var totalCount = await query.CountAsync();
//         
//         if (paging.OrderBy != null)
//         {
//             query = query.OrderBy(paging.OrderBy);
//         }
//         
//         var items = await query
//             .Skip((paging.Page - 1) * paging.PageSize)
//             .Take(paging.PageSize)
//             .ToListAsync();
//             
//         return new PagedResult<OrderEntity<CustomerEntity>>(items, totalCount, paging.Page, paging.PageSize);
//     }
//
//     public async Task<IEnumerable<OrderEntity<CustomerEntity>>> GetActiveOrdersAsync()
//     {
//         var activeStatuses = new[] { OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.InProgress };
//         
//         return await _context.Orders
//             .Include(o => o.Customer)
//             .Where(o => activeStatuses.Contains(o.Status) && !o.IsDeleted)
//             .OrderByDescending(o => o.OrderDate)
//             .ToListAsync();
//     }
//
//     public async Task<IEnumerable<OrderEntity<CustomerEntity>>> GetOverdueOrdersAsync()
//     {
//         var currentDate = DateTime.UtcNow.Date;
//         
//         return await _context.Orders
//             .Include(o => o.Customer)
//             .Where(o => o.Status == OrderStatus.Pending && 
//                        o.OrderDate.Date < currentDate.AddDays(-30) && 
//                        !o.IsDeleted)
//             .OrderByDescending(o => o.OrderDate)
//             .ToListAsync();
//     }
//
//     public async Task<decimal> GetTotalRevenueByTypeAsync(OrderType orderType, DateTime? startDate = null, DateTime? endDate = null)
//     {
//         var query = _context.Orders
//             .Where(o => o.OrderType == orderType && 
//                        o.Status == OrderStatus.Completed && 
//                        !o.IsDeleted);
//
//         if (startDate.HasValue)
//             query = query.Where(o => o.OrderDate >= startDate.Value);
//             
//         if (endDate.HasValue)
//             query = query.Where(o => o.OrderDate <= endDate.Value);
//
//         return await query.SumAsync(o => o.TotalAmount) / 100m; // Convert from cents
//     }
// } 