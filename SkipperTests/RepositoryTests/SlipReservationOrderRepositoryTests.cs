using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public class SlipReservationOrderRepositoryTests : RepositoryTestBase<SlipReservationOrderEntity>
{
    private SlipReservationOrderRepository _repository;
    protected ILogger<SlipReservationOrderRepository> Logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        Logger = TestSetup.CreateLogger<SlipReservationOrderRepository>();
        _repository = new SlipReservationOrderRepository(Context, Logger);
        
        // Create required related entities for foreign key relationships
        SetupRelatedEntities();
    }
    
    private void SetupRelatedEntities()
    {
        // Create a test address for the contact
        var address = new AddressEntity
        {
            Address1 = "123 Main St",
            City = "Anytown",
            State = "CA",
            ZipCode = "12345",
            Country = "USA",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        Context.Addresses.Add(address);
        Context.SaveChanges();
        
        // Create a test contact for the vessel owner
        var contact = new ContactEntity
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "555-1234",
            AddressId = address.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        Context.Contacts.Add(contact);
        Context.SaveChanges();
        
        // Create a test customer
        var customer = new VesselOwnerCustomerEntity
        {
            AccountNumber = "CUST001",
            Name = "Test Vessel Owner",
            LicenseNumber = "LIC123456",
            LicenseExpiryDate = DateTime.UtcNow.AddYears(1),
            ContactId = contact.Id,
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        Context.Customers.Add(customer);
        
        // Create a test slip
        var slip = new SlipEntity
        {
            SlipNumber = "A-001",
            LocationCode = "MARINA-A",
            SlipClassificationId = 1,
            Status = SlipStatus.Available,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        Context.Slips.Add(slip);
        
        // Create a test vessel
        var vessel = new VesselEntity
        {
            Name = "Test Vessel",
            RegistrationNumber = "ABC123",
            VesselType = VesselType.Sailboat,
            Length = 30,
            Beam = 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        Context.Vessels.Add(vessel);
        
        Context.SaveChanges();
    }

    #region Helper Methods

    private SlipReservationOrderEntity CreateSlipReservationOrder(
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
        var actualOrderNumber = orderNumber ?? $"SR-{actualOrderDate:yyyyMM}-0001-0001";

        return new SlipReservationOrderEntity
        {
            // Common order properties
            OrderNumber = actualOrderNumber,
            CustomerId = customerId,
            OrderDate = actualOrderDate,
            OrderType = OrderType.SlipReservation,
            TotalAmount = totalAmount,
            Notes = notes,
            Status = orderStatus,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false,
            
            // Slip-specific properties
            SlipId = slipId,
            VesselId = vesselId,
            StartDate = actualStartDate,
            EndDate = actualEndDate,
            PriceRate = priceRate,
            PriceUnit = priceUnit,
            RentalStatus = rentalStatus
        };
    }

    private async Task SetupOrdersAsync(params SlipReservationOrderEntity[] orders)
    {
        foreach (var order in orders)
        {
            await _repository.AddAsync(order);
        }
    }

    #endregion

    #region GetByIdAsync Tests

    [Test]
    public async Task GetByIdAsync_ValidId_ReturnsOrder()
    {
        // Arrange
        var order = CreateSlipReservationOrder("SR-202401-0001-0001");
        await SetupOrdersAsync(order);

        // Act
        var result = await _repository.GetByIdAsync(order.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(order.Id));
        Assert.That(result.OrderNumber, Is.EqualTo("SR-202401-0001-0001"));
        Assert.That(result.OrderType, Is.EqualTo(OrderType.SlipReservation));
        
        var slipOrder = result as SlipReservationOrderEntity;
        Assert.That(slipOrder, Is.Not.Null);
        Assert.That(slipOrder.SlipId, Is.EqualTo(1));
        Assert.That(slipOrder.VesselId, Is.EqualTo(1));
    }

    [Test]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_DeletedOrder_ReturnsNull()
    {
        // Arrange
        var order = CreateSlipReservationOrder();
        order.IsDeleted = true;
        await SetupOrdersAsync(order);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetSlipReservationsAsync Tests

    [Test]
    public async Task GetSlipReservationsAsync_ReturnsAllSlipReservations()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder("SR-202401-0001-0001"),
            CreateSlipReservationOrder("SR-202401-0001-0002"),
            CreateSlipReservationOrder("SR-202401-0001-0003")
        };
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.GetSlipReservationsAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.All(o => o.OrderType == OrderType.SlipReservation), Is.True);
    }

    [Test]
    public async Task GetSlipReservationsAsync_ExcludesDeletedOrders()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder("SR-202401-0001-0001"),
            CreateSlipReservationOrder("SR-202401-0001-0002"),
            CreateSlipReservationOrder("SR-202401-0001-0003")
        };
        orders[1].IsDeleted = true;
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.GetSlipReservationsAsync();

        // Assert
        var reservations = result.ToList();
        Assert.That(reservations, Has.Count.EqualTo(2));
        Assert.That(reservations.All(r => !r.IsDeleted), Is.True);
    }

    #endregion

    #region GetReservationsBySlipAsync Tests

    [Test]
    public async Task GetReservationsBySlipAsync_ReturnsReservationsForSlip()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder("SR-202401-0001-0001", slipId: 1),
            CreateSlipReservationOrder("SR-202401-0001-0002", slipId: 1),
            CreateSlipReservationOrder("SR-202401-0001-0003", slipId: 2)
        };
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.GetReservationsBySlipAsync(1);

        // Assert
        var reservations = result.ToList();
        Assert.That(reservations, Has.Count.EqualTo(2));
        Assert.That(reservations.All(r => r.SlipId == 1), Is.True);
    }

    #endregion

    #region GetReservationsByVesselAsync Tests

    [Test]
    public async Task GetReservationsByVesselAsync_ReturnsReservationsForVessel()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder("SR-202401-0001-0001", vesselId: 1),
            CreateSlipReservationOrder("SR-202401-0001-0002", vesselId: 1),
            CreateSlipReservationOrder("SR-202401-0001-0003", vesselId: 2)
        };
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.GetReservationsByVesselAsync(1);

        // Assert
        var reservations = result.ToList();
        Assert.That(reservations, Has.Count.EqualTo(2));
        Assert.That(reservations.All(r => r.VesselId == 1), Is.True);
    }

    #endregion

    #region IsSlipAvailableAsync Tests

    [Test]
    public async Task IsSlipAvailableAsync_NoOverlappingReservations_ReturnsTrue()
    {
        // Arrange
        var existingOrder = CreateSlipReservationOrder("SR-202401-0001-0001", slipId: 1);
        existingOrder.StartDate = DateTime.UtcNow.AddDays(10);
        existingOrder.EndDate = DateTime.UtcNow.AddDays(15);
        await SetupOrdersAsync(existingOrder);

        // Act
        var result = await _repository.IsSlipAvailableAsync(1, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(5));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsSlipAvailableAsync_OverlappingReservations_ReturnsFalse()
    {
        // Arrange
        var existingOrder = CreateSlipReservationOrder("SR-202401-0001-0001", slipId: 1);
        existingOrder.StartDate = DateTime.UtcNow.AddDays(1);
        existingOrder.EndDate = DateTime.UtcNow.AddDays(10);
        await SetupOrdersAsync(existingOrder);

        // Act
        var result = await _repository.IsSlipAvailableAsync(1, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(15));

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsSlipAvailableAsync_ExcludesSpecifiedOrder_ReturnsTrue()
    {
        // Arrange
        var existingOrder = CreateSlipReservationOrder("SR-202401-0001-0001", slipId: 1);
        existingOrder.StartDate = DateTime.UtcNow.AddDays(1);
        existingOrder.EndDate = DateTime.UtcNow.AddDays(10);
        await SetupOrdersAsync(existingOrder);

        // Act
        var result = await _repository.IsSlipAvailableAsync(1, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(15), existingOrder.Id);

        // Assert
        Assert.That(result, Is.True);
    }

    #endregion

    #region AddAsync Tests

    [Test]
    public async Task AddAsync_ValidOrder_AddsSuccessfully()
    {
        // Arrange
        var order = CreateSlipReservationOrder("SR-202401-0001-0001");

        // Act
        var result = await _repository.AddAsync(order);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.OrderNumber, Is.EqualTo("SR-202401-0001-0001"));
        Assert.That(result.CreatedAt, Is.Not.EqualTo(DateTime.MinValue));
        Assert.That(result.UpdatedAt, Is.Not.EqualTo(DateTime.MinValue));
    }

    #endregion

    #region UpdateAsync Tests

    [Test]
    public async Task UpdateAsync_ValidOrder_UpdatesSuccessfully()
    {
        // Arrange
        var order = CreateSlipReservationOrder("SR-202401-0001-0001");
        await SetupOrdersAsync(order);

        // Modify the order
        order.Notes = "Updated notes";
        order.TotalAmount = 15000;

        // Act
        await _repository.UpdateAsync(order);

        // Assert
        var updatedOrder = await _repository.GetByIdAsync(order.Id);
        Assert.That(updatedOrder.Notes, Is.EqualTo("Updated notes"));
        Assert.That(updatedOrder.TotalAmount, Is.EqualTo(15000));
    }

    #endregion

    #region DeleteAsync Tests

    [Test]
    public async Task DeleteAsync_ValidId_SoftDeletesOrder()
    {
        // Arrange
        var order = CreateSlipReservationOrder("SR-202401-0001-0001");
        await SetupOrdersAsync(order);

        // Act
        await _repository.DeleteAsync(order.Id);

        // Assert
        var deletedOrder = await _repository.GetByIdAsync(order.Id);
        Assert.That(deletedOrder, Is.Null); // Should not be found due to soft delete
    }

    #endregion

    #region GetActiveReservationsAsync Tests

    [Test]
    public async Task GetActiveReservationsAsync_ReturnsCurrentlyActiveReservations()
    {
        // Arrange
        var currentDate = DateTime.UtcNow.Date;
        var orders = new[]
        {
            CreateSlipReservationOrder("SR-202401-0001-0001"),
            CreateSlipReservationOrder("SR-202401-0001-0002"),
            CreateSlipReservationOrder("SR-202401-0001-0003")
        };
        orders[0].StartDate = DateTime.UtcNow.AddDays(-1);
        orders[0].EndDate = DateTime.UtcNow.AddDays(1);
        orders[0].RentalStatus = RentalStatus.Active;
        orders[1].StartDate = DateTime.UtcNow.AddDays(1);
        orders[1].EndDate = DateTime.UtcNow.AddDays(10);
        orders[1].RentalStatus = RentalStatus.Active;
        orders[2].StartDate = DateTime.UtcNow.AddDays(-10);
        orders[2].EndDate = DateTime.UtcNow.AddDays(-1);
        orders[2].RentalStatus = RentalStatus.Active;
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.GetActiveReservationsAsync();

        // Assert
        var activeReservations = result.ToList();
        Assert.That(activeReservations, Has.Count.EqualTo(1));
        Assert.That(activeReservations[0].OrderNumber, Is.EqualTo("SR-202401-0001-0001"));
    }

    #endregion

    #region GetRevenueBySlipAsync Tests

    [Test]
    public async Task GetRevenueBySlipAsync_CompletedOrders_ReturnsCorrectRevenue()
    {
        // Arrange
        var orders = new[]
        {
            CreateSlipReservationOrder("SR-202401-0001-0001", slipId: 1, totalAmount: 10000),
            CreateSlipReservationOrder("SR-202401-0001-0002", slipId: 1, totalAmount: 15000),
            CreateSlipReservationOrder("SR-202401-0001-0003", slipId: 2, totalAmount: 20000)
        };
        orders[0].Status = OrderStatus.Completed;
        orders[1].Status = OrderStatus.Completed;
        orders[2].Status = OrderStatus.Completed;
        await SetupOrdersAsync(orders);

        // Act
        var result = await _repository.GetRevenueBySlipAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(250.00m)); // (10000 + 15000) / 100
    }

    #endregion
} 