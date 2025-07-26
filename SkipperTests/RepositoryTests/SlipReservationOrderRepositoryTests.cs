using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public class SlipReservationOrderRepositoryTests : RepositoryTestBase<SlipReservationOrderCompositeEntity>
{
    private SlipReservationOrderRepository _repository;
    protected ILogger<SlipReservationOrderRepository> Logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        Logger = TestSetup.CreateLogger<SlipReservationOrderRepository>();
        _repository = new SlipReservationOrderRepository(Context, Logger);
    }

    #region Helper Methods

    private SlipReservationOrderCompositeEntity CreateSlipReservationOrder(
        long id,
        string orderNumber = null,
        long customerId = 1,
        DateTime? orderDate = null,
        long slipId = 1,
        long vesselId = 1,
        DateTime? startDate = null,
        DateTime? endDate = null,
        OrderStatus orderStatus = OrderStatus.Confirmed,
        RentalStatus rentalStatus = RentalStatus.Active,
        int totalAmount = 10000,
        int priceRate = 100,
        PriceUnit priceUnit = PriceUnit.PerDay,
        string notes = null)
    {
        var now = DateTime.UtcNow;
        var actualOrderDate = orderDate ?? now.AddDays(-1);
        var actualStartDate = startDate ?? now;
        var actualEndDate = endDate ?? now.AddDays(7);
        var actualOrderNumber = orderNumber ?? $"SR-{actualOrderDate:yyyyMM}-0001-{id:D4}";

        return new SlipReservationOrderCompositeEntity()
        {
            Order = new VesselOwnerOrderEntity
            {
                Id = id,
                OrderNumber = actualOrderNumber,
                CustomerId = customerId,
                OrderDate = actualOrderDate,
                OrderType = OrderType.SlipReservation,
                OrderTypeId = id,
                TotalAmount = totalAmount,
                Notes = notes,
                Status = orderStatus,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            },
            OrderInfo = new SlipReservationEntity
            {
                Id = id,
                SlipId = slipId,
                VesselId = vesselId,
                StartDate = actualStartDate,
                EndDate = actualEndDate,
                PriceRate = priceRate,
                PriceUnit = priceUnit,
                Status = rentalStatus,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            }
        };
    }

    private async Task SetupOrdersAsync(params SlipReservationOrderCompositeEntity[] orders)
    {
        foreach (var order in orders)
        {
            await _repository.AddAsync(order);
        }
    }

    #endregion

    #region GetByIdAsync Tests

    [Test]
    public async Task GetByIdAsync_ValidId_ReturnsComposite()
    {
        // Arrange
        var order = CreateSlipReservationOrder(1, "SR-202401-0001-0001");
        await SetupOrdersAsync(order);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Order.OrderNumber, Is.EqualTo("SR-202401-0001-0001"));
        Assert.That(result.OrderInfo.SlipId, Is.EqualTo(1));
        Assert.That(result.OrderInfo.VesselId, Is.EqualTo(1));
    }

    [Test]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        // Arrange
        var order = CreateSlipReservationOrder(1);
        await SetupOrdersAsync(order);

        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_DeletedOrder_ReturnsNull()
    {
        // Arrange
        var order = CreateSlipReservationOrder(1);
        await SetupOrdersAsync(order);
        await _repository.DeleteAsync(1);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetAllAsync Tests

    [Test]
    public async Task GetAllAsync_MultipleOrders_ReturnsAllActive()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder(1, "SR-202401-0001-0001"),
            CreateSlipReservationOrder(2, "SR-202401-0001-0002"),
            CreateSlipReservationOrder(3, "SR-202401-0001-0003")
        };
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 2L, 3L }));
    }

    [Test]
    public async Task GetAllAsync_NoOrders_ReturnsEmpty()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetAllAsync_WithDeletedOrders_ReturnsOnlyActive()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder(1),
            CreateSlipReservationOrder(2),
            CreateSlipReservationOrder(3)
        };
        await SetupOrdersAsync(orders);
        await _repository.DeleteAsync(2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 3L }));
    }

    #endregion

    #region AddAsync Tests

    [Test]
    public async Task AddAsync_ValidComposite_AddsSuccessfully()
    {
        // Arrange
        var order = CreateSlipReservationOrder(0, "SR-202401-0001-0001"); // ID 0 for new entity

        // Act
        var result = await _repository.AddAsync(order);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Order.OrderNumber, Is.EqualTo("SR-202401-0001-0001"));
        Assert.That(result.Order.CreatedAt, Is.Not.EqualTo(DateTime.MinValue));
        Assert.That(result.OrderInfo.CreatedAt, Is.Not.EqualTo(DateTime.MinValue));

        // Verify it was actually saved
        var retrieved = await _repository.GetByIdAsync(result.Id);
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved.Order.OrderNumber, Is.EqualTo("SR-202401-0001-0001"));
    }

    [Test]
    public async Task AddAsync_SetsTimestamps()
    {
        // Arrange
        var beforeAdd = DateTime.UtcNow;
        var order = CreateSlipReservationOrder(0);

        // Act
        var result = await _repository.AddAsync(order);
        var afterAdd = DateTime.UtcNow;

        // Assert
        Assert.That(result.Order.CreatedAt, Is.GreaterThanOrEqualTo(beforeAdd));
        Assert.That(result.Order.CreatedAt, Is.LessThanOrEqualTo(afterAdd));
        Assert.That(result.OrderInfo.CreatedAt, Is.GreaterThanOrEqualTo(beforeAdd));
        Assert.That(result.OrderInfo.CreatedAt, Is.LessThanOrEqualTo(afterAdd));
        Assert.That(result.Order.UpdatedAt, Is.EqualTo(result.Order.CreatedAt));
        Assert.That(result.OrderInfo.UpdatedAt, Is.EqualTo(result.OrderInfo.CreatedAt));
    }

    #endregion

    #region UpdateAsync Tests

    [Test]
    public async Task UpdateAsync_ValidComposite_UpdatesSuccessfully()
    {
        // Arrange
        var order = CreateSlipReservationOrder(1, totalAmount: 10000);
        await SetupOrdersAsync(order);

        var updatedOrder = await _repository.GetByIdAsync(1);
        updatedOrder.Order.TotalAmount = 15000;
        updatedOrder.Order.Notes = "Updated notes";
        updatedOrder.OrderInfo.PriceRate = 150;

        // Act
        await _repository.UpdateAsync(updatedOrder);

        // Assert
        var result = await _repository.GetByIdAsync(1);
        Assert.That(result.Order.TotalAmount, Is.EqualTo(15000));
        Assert.That(result.Order.Notes, Is.EqualTo("Updated notes"));
        Assert.That(result.OrderInfo.PriceRate, Is.EqualTo(150));
        Assert.That(result.Order.UpdatedAt, Is.GreaterThan(result.Order.CreatedAt));
        Assert.That(result.OrderInfo.UpdatedAt, Is.GreaterThan(result.OrderInfo.CreatedAt));
    }

    #endregion

    #region DeleteAsync Tests

    [Test]
    public async Task DeleteAsync_ValidId_SoftDeletesComposite()
    {
        // Arrange
        var order = CreateSlipReservationOrder(1);
        await SetupOrdersAsync(order);

        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var result = await _repository.GetByIdAsync(1);
        Assert.That(result, Is.Null);

        var allOrders = await _repository.GetAllAsync();
        Assert.That(allOrders.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task DeleteAsync_NonExistentId_DoesNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrowAsync(() => _repository.DeleteAsync(999));
    }

    #endregion

    #region ExistsAsync Tests

    [Test]
    public async Task ExistsAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var order = CreateSlipReservationOrder(1);
        await SetupOrdersAsync(order);

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsAsync_NonExistentId_ReturnsFalse()
    {
        // Arrange
        var order = CreateSlipReservationOrder(1);
        await SetupOrdersAsync(order);

        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ExistsAsync_DeletedOrder_ReturnsFalse()
    {
        // Arrange
        var order = CreateSlipReservationOrder(1);
        await SetupOrdersAsync(order);
        await _repository.DeleteAsync(1);

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region FindAsync Tests

    [Test]
    public async Task FindAsync_ByOrderStatus_ReturnsMatchingOrders()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder(1, orderStatus: OrderStatus.Confirmed),
            CreateSlipReservationOrder(2, orderStatus: OrderStatus.Confirmed),
            CreateSlipReservationOrder(3, orderStatus: OrderStatus.Pending),
            CreateSlipReservationOrder(4, orderStatus: OrderStatus.Cancelled)
        };
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.FindAsync(o => o.Order.Status == OrderStatus.Confirmed);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(r => r.Order.Status == OrderStatus.Confirmed), Is.True);
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 2L }));
    }

    [Test]
    public async Task FindAsync_ByRentalStatus_ReturnsMatchingOrders()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder(1, rentalStatus: RentalStatus.Active),
            CreateSlipReservationOrder(2, rentalStatus: RentalStatus.Active),
            CreateSlipReservationOrder(3, rentalStatus: RentalStatus.Pending),
            CreateSlipReservationOrder(4, rentalStatus: RentalStatus.Expired)
        };
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.FindAsync(o => o.OrderInfo.Status == RentalStatus.Active);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(r => r.OrderInfo.Status == RentalStatus.Active), Is.True);
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 2L }));
    }

    [Test]
    public async Task FindAsync_BySlipId_ReturnsMatchingOrders()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder(1, slipId: 1),
            CreateSlipReservationOrder(2, slipId: 1),
            CreateSlipReservationOrder(3, slipId: 2),
            CreateSlipReservationOrder(4, slipId: 3)
        };
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.FindAsync(o => o.OrderInfo.SlipId == 1);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(r => r.OrderInfo.SlipId == 1), Is.True);
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 2L }));
    }

    [Test]
    public async Task FindAsync_ByDateRange_ReturnsMatchingOrders()
    {
        // Arrange
        var baseDate = DateTime.Today;
        var orders = new[]
        {
            CreateSlipReservationOrder(1, startDate: baseDate, endDate: baseDate.AddDays(7)),
            CreateSlipReservationOrder(2, startDate: baseDate.AddDays(5), endDate: baseDate.AddDays(12)),
            CreateSlipReservationOrder(3, startDate: baseDate.AddDays(20), endDate: baseDate.AddDays(27))
        };
        await SetupOrdersAsync(orders);

        // Act - Find orders that start within first two weeks
        var result = await _repository.FindAsync(o => o.OrderInfo.StartDate <= baseDate.AddDays(14));

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 2L }));
    }

    [Test]
    public async Task FindAsync_NoMatches_ReturnsEmpty()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder(1, orderStatus: OrderStatus.Confirmed),
            CreateSlipReservationOrder(2, orderStatus: OrderStatus.Pending)
        };
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.FindAsync(o => o.Order.Status == OrderStatus.Cancelled);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    #endregion

    #region Paging Tests

    [Test]
    public async Task GetPagedAsync_FirstPage_ReturnsCorrectResults()
    {
        // Arrange
        var orders = Enumerable.Range(1, 10)
            .Select(i => CreateSlipReservationOrder(i, $"SR-202401-0001-{i:D4}"))
            .ToArray();
        await SetupOrdersAsync(orders);

        var pagingParams = new PagingParameters<SlipReservationOrderCompositeEntity>
        {
            PageSize = 3,
            Page = 1,
            OrderBy = o => o.Order.OrderNumber
        };

        // Act
        var result = await _repository.GetPagedAsync(pagingParams);

        // Assert
        Assert.That(result.Items.Count(), Is.EqualTo(3));
        Assert.That(result.TotalCount, Is.EqualTo(10));
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(3));
        Assert.That(result.Items.Select(i => i.Id), Is.EquivalentTo(new[] { 1L, 2L, 3L }));
    }

    [Test]
    public async Task GetPagedAsync_WithPredicate_ReturnsFilteredResults()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder(1, orderStatus: OrderStatus.Confirmed),
            CreateSlipReservationOrder(2, orderStatus: OrderStatus.Confirmed),
            CreateSlipReservationOrder(3, orderStatus: OrderStatus.Pending),
            CreateSlipReservationOrder(4, orderStatus: OrderStatus.Confirmed),
            CreateSlipReservationOrder(5, orderStatus: OrderStatus.Cancelled)
        };
        await SetupOrdersAsync(orders);

        var pagingParams = new PagingParameters<SlipReservationOrderCompositeEntity>
        {
            PageSize = 2,
            Page = 1,
            OrderBy = o => o.Id
        };

        // Act
        var result = await _repository.GetPagedAsync(
            o => o.Order.Status == OrderStatus.Confirmed, 
            pagingParams);

        // Assert
        Assert.That(result.Items.Count(), Is.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(3)); // Only confirmed orders
        Assert.That(result.Items.All(i => i.Order.Status == OrderStatus.Confirmed), Is.True);
    }

    [Test]
    public async Task GetPageCountAsync_CalculatesCorrectPageCount()
    {
        // Arrange
        var orders = Enumerable.Range(1, 7)
            .Select(i => CreateSlipReservationOrder(i))
            .ToArray();
        await SetupOrdersAsync(orders);

        var pagingParams = new PagingParameters<SlipReservationOrderCompositeEntity>
        {
            PageSize = 3
        };

        // Act
        var result = await _repository.GetPageCountAsync(o => true, pagingParams);

        // Assert
        Assert.That(result, Is.EqualTo(3)); // 7 items, 3 per page = 3 pages
    }

    [Test]
    public async Task GetPageCountAsync_WithPredicate_CalculatesCorrectPageCount()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder(1, orderStatus: OrderStatus.Confirmed),
            CreateSlipReservationOrder(2, orderStatus: OrderStatus.Confirmed),
            CreateSlipReservationOrder(3, orderStatus: OrderStatus.Pending),
            CreateSlipReservationOrder(4, orderStatus: OrderStatus.Confirmed),
            CreateSlipReservationOrder(5, orderStatus: OrderStatus.Cancelled)
        };
        await SetupOrdersAsync(orders);

        var pagingParams = new PagingParameters<SlipReservationOrderCompositeEntity>
        {
            PageSize = 2
        };

        // Act
        var result = await _repository.GetPageCountAsync(
            o => o.Order.Status == OrderStatus.Confirmed, 
            pagingParams);

        // Assert
        Assert.That(result, Is.EqualTo(2)); // 3 confirmed orders, 2 per page = 2 pages
    }

    #endregion
} 