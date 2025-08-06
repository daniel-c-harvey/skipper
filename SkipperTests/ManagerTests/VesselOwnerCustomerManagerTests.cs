using Data.Shared.Managers;
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
    private VesselOwnerCustomerManager _manager;
    private ContactManager _contactManager;
    private VesselManager _vesselManager;
    private ILogger<VesselOwnerCustomerRepository> _logger;

    [SetUp]
    public void SetUp()
    {
        _context = TestSetup.CreateContext();
        _logger = TestSetup.CreateLogger<VesselOwnerCustomerRepository>();
        
        // Create managers with their dependencies
        _contactManager = new ContactManager(
            new ContactRepository(_context, TestSetup.CreateLogger<ContactRepository>()),
            new AddressManager(
                new AddressRepository(_context, TestSetup.CreateLogger<AddressRepository>())
            )
        );
        
        _vesselManager = new VesselManager(
            new VesselRepository(_context, TestSetup.CreateLogger<VesselRepository>())
        );
        
        _manager = new VesselOwnerCustomerManager(
            new VesselOwnerCustomerRepository(_context, _logger),
            _contactManager,
            _vesselManager
        );
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    #region Helper Methods

    private VesselOwnerCustomerModel CreateVesselOwnerCustomerModel(
        string accountNumber = "ACC-001",
        string name = "John Doe",
        string licenseNumber = "LIC-123456",
        DateTime? licenseExpiryDate = null)
    {
        var now = DateTime.UtcNow;
        return new VesselOwnerCustomerModel
        {
            // Id is NOT set - let the database auto-generate it
            AccountNumber = accountNumber,
            Name = name,
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            LicenseNumber = licenseNumber,
            LicenseExpiryDate = licenseExpiryDate ?? now.AddYears(1),
            Contact = CreateContactModel("John", "Doe"), // The manager will handle contact creation
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private ContactModel CreateContactModel(string firstName, string lastName)
    {
        var now = DateTime.UtcNow;
        return new ContactModel
        {
            // Id is NOT set - let the database auto-generate it
            FirstName = firstName,
            LastName = lastName,
            Email = firstName.ToLower() + "." + lastName.ToLower() + "@test.com",
            PhoneNumber = "41041041010",
            Address = CreateAddressModel(), // The contact manager will handle address creation
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private AddressModel CreateAddressModel(
        string street = "123 Test St",
        string city = "Test City",
        string state = "TS",
        string zipCode = "12345")
    {
        var now = DateTime.UtcNow;
        return new AddressModel
        {
            // Id is NOT set - let the database auto-generate it
            Address1 = street,
            City = city,
            State = state,
            ZipCode = zipCode,
            Country = "US",
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private VesselModel CreateVesselModel(
        string name = "Test Vessel",
        string registrationNumber = "REG-123456",
        VesselType vesselType = VesselType.Sailboat,
        decimal length = 30.0m,
        decimal beam = 10.0m)
    {
        var now = DateTime.UtcNow;
        return new VesselModel
        {
            // Id is NOT set - let the database auto-generate it
            Name = name,
            RegistrationNumber = registrationNumber,
            VesselType = vesselType,
            Length = length,
            Beam = beam,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    // Helper method to create and add vessel owner customers using the manager
    private async Task<List<VesselOwnerCustomerModel>> CreateVesselOwnerCustomersAsync(params VesselOwnerCustomerModel[] models)
    {
        var custs = new List<VesselOwnerCustomerModel>();
        
        foreach (var model in models)
        {
            var result = await _manager.Add(model);
            if (result.Success && result.Value != null)
            {
                custs.Add(result.Value); // Get the auto-generated ID
            }
        }
        
        return custs;
    }

    // Helper method to create and add vessels using the vessel manager
    private async Task<List<long>> CreateVesselsAsync(params VesselModel[] models)
    {
        var ids = new List<long>();
        
        foreach (var model in models)
        {
            var result = await _vesselManager.Add(model);
            if (result.Success && result.Value != null)
            {
                ids.Add(result.Value.Id); // Get the auto-generated ID
            }
        }
        
        return ids;
    }

    #endregion

    #region GetVesselOwnersByLicenseAsync Tests

    [Test]
    public async Task GetVesselOwnersByLicenseAsync_ValidLicense_ReturnsConvertedModels()
    {
        // Arrange
        var licenseNumber = "LIC-123456";
        
        var models = new[]
        {
            CreateVesselOwnerCustomerModel(licenseNumber: licenseNumber),
            CreateVesselOwnerCustomerModel(licenseNumber: licenseNumber)
        };

        // Use the manager to add entities (this will auto-create contacts and addresses)
        var customerIds = await CreateVesselOwnerCustomersAsync(models);

        // Act
        var result = await _manager.GetVesselOwnersByLicenseAsync(licenseNumber);

        // Assert
        var resultModels = result.ToList();
        Assert.That(resultModels, Has.Count.EqualTo(2));
        Assert.That(resultModels.All(m => m.LicenseNumber == licenseNumber), Is.True);
        Assert.That(resultModels.All(m => m.CustomerProfileType == CustomerProfileType.VesselOwnerProfile), Is.True);
        
        // Verify that the returned models have the auto-generated IDs
        Assert.That(resultModels.Select(m => m.Id), Is.EquivalentTo(customerIds));
    }

    [Test]
    public async Task GetVesselOwnersByLicenseAsync_EmptyResult_ReturnsEmptyCollection()
    {
        // Arrange
        var model = CreateVesselOwnerCustomerModel(licenseNumber: "LIC-123456");
        await _manager.Add(model);

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
        
        var models = new[]
        {
            CreateVesselOwnerCustomerModel(licenseExpiryDate: now.AddDays(-30)),
            CreateVesselOwnerCustomerModel(licenseExpiryDate: now.AddDays(-10))
        };

        var customerIds = await CreateVesselOwnerCustomersAsync(models);

        // Act
        var result = await _manager.GetVesselOwnersWithExpiredLicensesAsync();

        // Assert
        var resultModels = result.ToList();
        Assert.That(resultModels, Has.Count.EqualTo(2));
        Assert.That(resultModels.All(m => m.LicenseExpiryDate < now), Is.True);
        Assert.That(resultModels.All(m => m.CustomerProfileType == CustomerProfileType.VesselOwnerProfile), Is.True);
        Assert.That(resultModels.Select(m => m.Id), Is.EquivalentTo(customerIds));
    }

    [Test]
    public async Task GetVesselOwnersWithExpiredLicensesAsync_NoExpiredLicenses_ReturnsEmptyCollection()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var model = CreateVesselOwnerCustomerModel(licenseExpiryDate: now.AddDays(30));
        await _manager.Add(model);

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
        var vesselOwnerModels = new[]
        {
            CreateVesselOwnerCustomerModel(name: "Owner with Vessels 1"),
            CreateVesselOwnerCustomerModel(name: "Owner with Vessels 2")
        };

        // Create vessel owners first
        var customers = await CreateVesselOwnerCustomersAsync(vesselOwnerModels);

        // Create vessels using the vessel manager
        var vesselModels = new[]
        {
            CreateVesselModel(name: "Vessel 1", registrationNumber: "REG001"),
            CreateVesselModel(name: "Vessel 2", registrationNumber: "REG002")
        };

        await _manager.AddVesselToOwner(customers[0], vesselModels[0]);
        await _manager.AddVesselToOwner(customers[1], vesselModels[1]);

        // Act
        var result = await _manager.GetVesselOwnersWithVesselsAsync();

        // Assert
        var resultModels = result.ToList();
        Assert.That(resultModels, Has.Count.EqualTo(2));
        Assert.That(resultModels.All(m => m.CustomerProfileType == CustomerProfileType.VesselOwnerProfile), Is.True);
        Assert.That(resultModels.Select(m => m.Id), Is.EquivalentTo(customers.Select(c => c.Id)));
    }

    #endregion

    #region SearchCustomersAsync Tests (Inherited from base)

    [Test]
    public async Task SearchCustomersAsync_ValidSearchTerm_ReturnsConvertedModels()
    {
        // Arrange
        var models = new[]
        {
            CreateVesselOwnerCustomerModel(name: "John Doe"),
            CreateVesselOwnerCustomerModel(name: "John Smith"),
            CreateVesselOwnerCustomerModel(name: "Jane Doe")
        };

        var customerIds = await CreateVesselOwnerCustomersAsync(models);

        // Act
        var result = await _manager.SearchCustomersAsync("John");

        // Assert
        var resultModels = result.ToList();
        Assert.That(resultModels, Has.Count.EqualTo(2));
        Assert.That(resultModels.All(m => m.Name.Contains("John")), Is.True);
        Assert.That(resultModels.All(m => m.CustomerProfileType == CustomerProfileType.VesselOwnerProfile), Is.True);
        
        // Verify we get the correct IDs for customers with "John" in their name
        var expectedIds = customerIds.Where((id, index) => models[index].Name.Contains("John")).ToList();
        Assert.That(resultModels.Select(m => m.Id), Is.EquivalentTo(expectedIds));
    }

    [Test]
    public async Task SearchCustomersAsync_NoMatches_ReturnsEmptyCollection()
    {
        // Arrange
        var model = CreateVesselOwnerCustomerModel(name: "John Doe");
        await _manager.Add(model);

        // Act
        var result = await _manager.SearchCustomersAsync("NonExistent");

        // Assert
        Assert.That(result.ToList(), Is.Empty);
    }

    #endregion
} 