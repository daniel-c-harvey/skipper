using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperData.Data.Repositories;
using SkipperData.Managers;
using SkipperModels.Entities;
using SkipperModels.Models;
using SkipperModels;

namespace SkipperTests.ManagerTests;

[TestFixture]
public class VesselOwnerCustomerManagerTests
{
    private SkipperContext _context;
    private VesselOwnerCustomerRepository _repository;
    private VesselOwnerCustomerManager _manager;
    private ILogger<VesselOwnerCustomerRepository> _logger;

    [SetUp]
    public void SetUp()
    {
        _context = TestSetup.CreateContext();
        _logger = TestSetup.CreateLogger<VesselOwnerCustomerRepository>();
        _repository = new VesselOwnerCustomerRepository(_context, _logger);
        _manager = new VesselOwnerCustomerManager(_repository);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    #region Helper Methods

    private VesselOwnerCustomerEntity CreateVesselOwnerCustomerEntity(
        long id,
        string accountNumber = "ACC-001",
        string name = "John Doe",
        string licenseNumber = "LIC-123456",
        DateTime? licenseExpiryDate = null,
        bool isDeleted = false)
    {
        var now = DateTime.UtcNow;
        return new VesselOwnerCustomerEntity
        {
            Id = id,
            AccountNumber = accountNumber,
            Name = name,
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            LicenseNumber = licenseNumber,
            LicenseExpiryDate = licenseExpiryDate ?? now.AddYears(1),
            ContactId = 1,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = isDeleted
        };
    }

    private VesselOwnerCustomerModel CreateVesselOwnerCustomerModel(
        long id,
        string accountNumber = "ACC-001",
        string name = "John Doe",
        string licenseNumber = "LIC-123456",
        DateTime? licenseExpiryDate = null)
    {
        var now = DateTime.UtcNow;
        return new VesselOwnerCustomerModel
        {
            Id = id,
            AccountNumber = accountNumber,
            Name = name,
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            LicenseNumber = licenseNumber,
            LicenseExpiryDate = licenseExpiryDate ?? now.AddYears(1),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    #endregion

    #region GetVesselOwnersByLicenseAsync Tests

    [Test]
    public async Task GetVesselOwnersByLicenseAsync_ValidLicense_ReturnsConvertedModels()
    {
        // Arrange
        var licenseNumber = "LIC-123456";
        var entities = new[]
        {
            CreateVesselOwnerCustomerEntity(1, licenseNumber: licenseNumber),
            CreateVesselOwnerCustomerEntity(2, licenseNumber: licenseNumber)
        };

        foreach (var entity in entities)
        {
            await _repository.AddAsync(entity);
        }

        // Act
        var result = await _manager.GetVesselOwnersByLicenseAsync(licenseNumber);

        // Assert
        var models = result.ToList();
        Assert.That(models, Has.Count.EqualTo(2));
        Assert.That(models.All(m => m.LicenseNumber == licenseNumber), Is.True);
        Assert.That(models.All(m => m.CustomerProfileType == CustomerProfileType.VesselOwnerProfile), Is.True);
    }

    [Test]
    public async Task GetVesselOwnersByLicenseAsync_EmptyResult_ReturnsEmptyCollection()
    {
        // Arrange
        var entity = CreateVesselOwnerCustomerEntity(1, licenseNumber: "LIC-123456");
        await _repository.AddAsync(entity);

        // Act
        var result = await _manager.GetVesselOwnersByLicenseAsync("LIC-999999");

        // Assert
        Assert.That(result.ToList(), Is.Empty);
    }

    #endregion

    #region GetVesselOwnersWithExpiredLicensesAsync Tests

    [Test]
    public async Task GetVesselOwnersWithExpiredLicensesAsync_ExpiredLicenses_ReturnsConvertedModels()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entities = new[]
        {
            CreateVesselOwnerCustomerEntity(1, licenseExpiryDate: now.AddDays(-30)),
            CreateVesselOwnerCustomerEntity(2, licenseExpiryDate: now.AddDays(-10))
        };

        foreach (var entity in entities)
        {
            await _repository.AddAsync(entity);
        }

        // Act
        var result = await _manager.GetVesselOwnersWithExpiredLicensesAsync();

        // Assert
        var models = result.ToList();
        Assert.That(models, Has.Count.EqualTo(2));
        Assert.That(models.All(m => m.LicenseExpiryDate < now), Is.True);
        Assert.That(models.All(m => m.CustomerProfileType == CustomerProfileType.VesselOwnerProfile), Is.True);
    }

    [Test]
    public async Task GetVesselOwnersWithExpiredLicensesAsync_NoExpiredLicenses_ReturnsEmptyCollection()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entity = CreateVesselOwnerCustomerEntity(1, licenseExpiryDate: now.AddDays(30));
        await _repository.AddAsync(entity);

        // Act
        var result = await _manager.GetVesselOwnersWithExpiredLicensesAsync();

        // Assert
        Assert.That(result.ToList(), Is.Empty);
    }

    #endregion

    #region GetVesselOwnersWithVesselsAsync Tests

    [Test]
    public async Task GetVesselOwnersWithVesselsAsync_WithVessels_ReturnsConvertedModels()
    {
        // Arrange
        var entities = new[]
        {
            CreateVesselOwnerCustomerEntity(1, name: "Owner with Vessels 1"),
            CreateVesselOwnerCustomerEntity(2, name: "Owner with Vessels 2")
        };

        foreach (var entity in entities)
        {
            // Create a vessel for this vessel owner
            var vessel = new VesselEntity { Id = entity.Id, Name = $"Vessel{entity.Id}", RegistrationNumber = $"REG{entity.Id}", IsDeleted = false };
            entity.VesselOwnerVessels.Add(new VesselOwnerVesselEntity 
            { 
                VesselOwnerCustomerId = entity.Id, 
                VesselId = vessel.Id, 
                Vessel = vessel 
            });
            
            await _repository.AddAsync(entity);
        }

        // Act
        var result = await _manager.GetVesselOwnersWithVesselsAsync();

        // Assert
        var models = result.ToList();
        Assert.That(models, Has.Count.EqualTo(2));
        Assert.That(models.All(m => m.CustomerProfileType == CustomerProfileType.VesselOwnerProfile), Is.True);
    }

    #endregion

    #region GetActiveCustomersAsync Tests (Inherited from base)

    [Test]
    public async Task GetActiveCustomersAsync_ActiveCustomers_ReturnsConvertedModels()
    {
        // Arrange
        var entities = new[]
        {
            CreateVesselOwnerCustomerEntity(1, name: "Active Customer 1"),
            CreateVesselOwnerCustomerEntity(2, name: "Active Customer 2"),
            CreateVesselOwnerCustomerEntity(3, name: "Deleted Customer", isDeleted: true)
        };

        foreach (var entity in entities)
        {
            await _repository.AddAsync(entity);
        }

        // Act
        var result = await _manager.GetActiveCustomersAsync();

        // Assert
        var models = result.ToList();
        Assert.That(models, Has.Count.EqualTo(2));
        Assert.That(models.All(m => m.CustomerProfileType == CustomerProfileType.VesselOwnerProfile), Is.True);
        Assert.That(models.All(m => m.Name.Contains("Active")), Is.True);
    }

    #endregion

    #region SearchCustomersAsync Tests (Inherited from base)

    [Test]
    public async Task SearchCustomersAsync_ValidSearchTerm_ReturnsConvertedModels()
    {
        // Arrange
        var entities = new[]
        {
            CreateVesselOwnerCustomerEntity(1, name: "John Doe"),
            CreateVesselOwnerCustomerEntity(2, name: "John Smith"),
            CreateVesselOwnerCustomerEntity(3, name: "Jane Doe")
        };

        foreach (var entity in entities)
        {
            await _repository.AddAsync(entity);
        }

        // Act
        var result = await _manager.SearchCustomersAsync("John");

        // Assert
        var models = result.ToList();
        Assert.That(models, Has.Count.EqualTo(2));
        Assert.That(models.All(m => m.Name.Contains("John")), Is.True);
        Assert.That(models.All(m => m.CustomerProfileType == CustomerProfileType.VesselOwnerProfile), Is.True);
    }

    [Test]
    public async Task SearchCustomersAsync_NoMatches_ReturnsEmptyCollection()
    {
        // Arrange
        var entity = CreateVesselOwnerCustomerEntity(1, name: "John Doe");
        await _repository.AddAsync(entity);

        // Act
        var result = await _manager.SearchCustomersAsync("NonExistent");

        // Assert
        Assert.That(result.ToList(), Is.Empty);
    }

    #endregion
} 