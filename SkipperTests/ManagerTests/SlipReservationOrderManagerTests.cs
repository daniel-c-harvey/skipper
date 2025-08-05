using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperData.Data.Repositories;
using SkipperData.Managers;
using SkipperModels.Entities;
using SkipperModels.Models;
using SkipperModels;
using Models.Shared.Common;
using NetBlocks.Models;

namespace SkipperTests.ManagerTests;

[TestFixture]
public class SlipReservationOrderManagerTests
{
    private SkipperContext _context;
    private SlipReservationOrderManager _manager;
    private VesselOwnerCustomerManager _customerManager;

    [SetUp]
    public void SetUp()
    {
        _context = TestSetup.CreateContext();
        _customerManager = new VesselOwnerCustomerManager(
            new VesselOwnerCustomerRepository(
                _context, 
                TestSetup.CreateLogger<VesselOwnerCustomerRepository>()), 
                new ContactManager(
                    new ContactRepository(
                        _context,
                        TestSetup.CreateLogger<ContactRepository>()),
                    new AddressManager(
                        new AddressRepository(
                            _context,
                            TestSetup.CreateLogger<AddressRepository>()))));
        _manager = new SlipReservationOrderManager(
            new SlipReservationOrderRepository(
                _context, 
                TestSetup.CreateLogger<SlipReservationOrderRepository>()),
            _customerManager);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    #region Helper Methods

    private (SlipModel slip, VesselModel vessel, VesselOwnerCustomerModel customer, SlipReservationOrderModel order) CreateSlipReservationOrderModel(
        long customerId = 0,
        long slipId = 0,
        long vesselId = 0,
        DateTime? startDate = null,
        DateTime? endDate = null,
        OrderStatus status = OrderStatus.Pending,
        RentalStatus rentalStatus = RentalStatus.Active,
        int priceRate = 100,
        PriceUnit priceUnit = PriceUnit.PerDay)
    {
        var now = DateTime.UtcNow;
        var start = startDate ?? now;
        var end = endDate ?? start.AddDays(7);

        var customer = CreateVesselOwnerCustomerModel(now);

        var slip = new SlipModel
        {
            SlipNumber = "1234567890",
            SlipClassification = new SlipClassificationModel
            {
                Name = "Test Classification",
                BasePrice = 500,
                Description = "Slip Classification Description",
                MaxBeam = 1,
                MaxLength = 1,
                CreatedAt = now,
                UpdatedAt = now
            },
        };

        var vessel = new VesselModel
        {
            Beam = 7,
            Length = 23,
            Name = "Test Vessel",
            RegistrationNumber = "1234567890",
            VesselType = VesselType.Motorboat,
            CreatedAt = now,
            UpdatedAt = now
        };
        
        var order =  new SlipReservationOrderModel
        {
            Customer = customer,
            Slip = slip,
            Vessel = vessel,
            StartDate = start,
            EndDate = end,
            Status = status,
            RentalStatus = rentalStatus,
            PriceRate = priceRate,
            PriceUnit = priceUnit,
            OrderDate = now,
            CreatedAt = now,
            UpdatedAt = now
        };
        
        return (slip, vessel, customer, order);
    }

    private VesselOwnerCustomerModel CreateVesselOwnerCustomerModel(DateTime now)
    {
        var customer = new VesselOwnerCustomerModel
        {
            Name = "Test McTester",
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            Contact = new ContactModel
            {
                Address = new AddressModel
                {
                    Address1 = "1600 Pennsylvania Ave NW",
                    City = "Washington",
                    State = "DC",
                    ZipCode = "20500",
                    Country = "US",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                FirstName = "Test",
                LastName = "McTester",
                Email = "test@test.com",
                PhoneNumber = "1234567890",
                CreatedAt = now,
                UpdatedAt = now
            },
            CreatedAt = now,
            UpdatedAt = now,
        };
        return customer;
    }

    #endregion

    #region Manager Tests

    [Test]
    public async Task CompleteWorkflow_CreateUpdateDelete_WorksCorrectly()
    {
        // Arrange
        var modelPackage = CreateSlipReservationOrderModel();
        var slipModel = modelPackage.slip;
        var vesselModel = modelPackage.vessel;
        var customerModel = modelPackage.customer;
        var orderModel = modelPackage.order;

        // Act - Create
        var createdCustomer = await _customerManager.Add(customerModel);
        var createdOrder = await _manager.Add(orderModel);
        Assert.That(createdOrder.Success, Is.True);
        Assert.That(createdOrder.Value, Is.Not.Null);
        Assert.That(createdOrder.Value.Id, Is.GreaterThan(0));

        // Arrange - Update
        orderModel.Status = OrderStatus.Confirmed;
        
        // Act - Update
        var updateResult = await _manager.Update(orderModel);
        Assert.That(updateResult.Success, Is.True);
        
        var updated = await _manager.GetById(createdOrder.Value.Id);
        Assert.That(updated.Success, Is.True);
        Assert.That(updated.Value, Is.Not.Null);
        Assert.That(updated.Value.Status, Is.EqualTo(OrderStatus.Confirmed));

        // Act - Delete
        var deleteResult = await _manager.Delete(createdOrder.Value.Id);
        Assert.That(deleteResult.Success, Is.True);
        
        var deleted = await _manager.GetById(createdOrder.Value.Id);
        Assert.That(deleted.Success, Is.False);
    }

    // [Test]
    // public async Task ComplexQuery_GetActiveReservationsForCustomer_ReturnsCorrectResults()
    // {
    //     // Arrange - Create multiple models with different statuses
    //     var model1 = CreateSlipReservationOrderModel(customerId: 1, status: OrderStatus.Confirmed, rentalStatus: RentalStatus.Active);
    //     var model2 = CreateSlipReservationOrderModel(customerId: 1, status: OrderStatus.Confirmed, rentalStatus: RentalStatus.Active);
    //     var model3 = CreateSlipReservationOrderModel(customerId: 2, status: OrderStatus.Confirmed, rentalStatus: RentalStatus.Active);
    //     var model4 = CreateSlipReservationOrderModel(customerId: 1, status: OrderStatus.Confirmed, rentalStatus: RentalStatus.Expired);
    //
    //     // Act - Add all models
    //     var created1 = await _manager.Add(model1);
    //     var created2 = await _manager.Add(model2);
    //     var created3 = await _manager.Add(model3);
    //     var created4 = await _manager.Add(model4);
    //
    //     Assert.That(created1.Success, Is.True);
    //     Assert.That(created2.Success, Is.True);
    //     Assert.That(created3.Success, Is.True);
    //     Assert.That(created4.Success, Is.True);
    //
    //     // Act - Test manager methods
    //     var customerOrders = await _manager.GetOrdersByCustomerAsync(1);
    //     var activeReservations = await _manager.GetActiveReservationsAsync();
    //
    //     // Assert
    //     Assert.That(customerOrders, Is.Not.Null);
    //     Assert.That(customerOrders.Count(), Is.EqualTo(3)); // 3 orders for customer 1
    //     Assert.That(activeReservations, Is.Not.Null);
    //     Assert.That(activeReservations.Count(), Is.EqualTo(2)); // 2 active reservations (excluding expired)
    // }

    #endregion
} 