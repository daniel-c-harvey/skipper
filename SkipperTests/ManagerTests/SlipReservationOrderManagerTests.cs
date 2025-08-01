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
    private SlipReservationOrderManager _manager;
    private ILogger<SlipReservationOrderRepository> _logger;

    [SetUp]
    public void SetUp()
    {
        _logger = TestSetup.CreateLogger<SlipReservationOrderRepository>();
        _manager = new SlipReservationOrderManager(new SlipReservationOrderRepository(TestSetup.CreateContext(), _logger));
    }

    #region Helper Methods

    private SlipReservationOrderModel CreateSlipReservationOrderModel(
        long customerId = 1,
        long slipId = 1,
        long vesselId = 1,
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
        
        return new SlipReservationOrderModel
        {
            Customer = new VesselOwnerCustomerModel { Id = customerId },
            Slip = new SlipModel { Id = slipId },
            Vessel = new VesselModel { Id = vesselId },
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
    }

    #endregion

    #region Manager Tests

    [Test]
    public async Task CompleteWorkflow_CreateUpdateDelete_WorksCorrectly()
    {
        // Arrange
        var model = CreateSlipReservationOrderModel();

        // Act - Create
        var created = await _manager.Add(model);
        Assert.That(created.Success, Is.True);
        Assert.That(created.Value, Is.Not.Null);
        Assert.That(created.Value.Id, Is.GreaterThan(0));

        // Act - Update
        var updateModel = CreateSlipReservationOrderModel();
        updateModel.Id = created.Value.Id;
        updateModel.Status = OrderStatus.Confirmed;
        
        var updateResult = await _manager.Update(updateModel);
        Assert.That(updateResult.Success, Is.True);
        
        var updated = await _manager.GetById(created.Value.Id);
        Assert.That(updated.Success, Is.True);
        Assert.That(updated.Value, Is.Not.Null);
        Assert.That(updated.Value.Status, Is.EqualTo(OrderStatus.Confirmed));

        // Act - Delete
        var deleteResult = await _manager.Delete(created.Value.Id);
        Assert.That(deleteResult.Success, Is.True);
        
        var deleted = await _manager.GetById(created.Value.Id);
        Assert.That(deleted.Success, Is.False);
    }

    [Test]
    public async Task ComplexQuery_GetActiveReservationsForCustomer_ReturnsCorrectResults()
    {
        // Arrange - Create multiple models with different statuses
        var model1 = CreateSlipReservationOrderModel(customerId: 1, status: OrderStatus.Confirmed, rentalStatus: RentalStatus.Active);
        var model2 = CreateSlipReservationOrderModel(customerId: 1, status: OrderStatus.Confirmed, rentalStatus: RentalStatus.Active);
        var model3 = CreateSlipReservationOrderModel(customerId: 2, status: OrderStatus.Confirmed, rentalStatus: RentalStatus.Active);
        var model4 = CreateSlipReservationOrderModel(customerId: 1, status: OrderStatus.Confirmed, rentalStatus: RentalStatus.Expired);

        // Act - Add all models
        var created1 = await _manager.Add(model1);
        var created2 = await _manager.Add(model2);
        var created3 = await _manager.Add(model3);
        var created4 = await _manager.Add(model4);

        Assert.That(created1.Success, Is.True);
        Assert.That(created2.Success, Is.True);
        Assert.That(created3.Success, Is.True);
        Assert.That(created4.Success, Is.True);

        // Act - Test manager methods
        var customerOrders = await _manager.GetOrdersByCustomerAsync(1);
        var activeReservations = await _manager.GetActiveReservationsAsync();

        // Assert
        Assert.That(customerOrders, Is.Not.Null);
        Assert.That(customerOrders.Count(), Is.EqualTo(3)); // 3 orders for customer 1
        Assert.That(activeReservations, Is.Not.Null);
        Assert.That(activeReservations.Count(), Is.EqualTo(2)); // 2 active reservations (excluding expired)
    }

    #endregion
} 