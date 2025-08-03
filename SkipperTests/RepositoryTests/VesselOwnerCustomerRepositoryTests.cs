using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public class VesselOwnerCustomerRepositoryTests : RepositoryTestBase<VesselOwnerCustomerEntity>
{
    private VesselOwnerCustomerRepository _repository;
    protected ILogger<VesselOwnerCustomerRepository> Logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        Logger = TestSetup.CreateLogger<VesselOwnerCustomerRepository>();
        _repository = new VesselOwnerCustomerRepository(Context, Logger);
    }

    #region Helper Methods

    private VesselOwnerCustomerEntity CreateVesselOwnerCustomer(
        long id,
        string accountNumber = null,
        string name = null,
        string licenseNumber = null,
        DateTime? licenseExpiryDate = null,
        long contactId = 1,
        bool isDeleted = false)
    {
        var now = DateTime.UtcNow;
        var actualAccountNumber = accountNumber ?? $"ACC-{id:D6}";
        var actualName = name ?? $"Vessel Owner {id}";
        var actualLicenseNumber = licenseNumber ?? $"LIC-{id:D6}";
        var actualExpiryDate = licenseExpiryDate ?? now.AddYears(1);

        return new VesselOwnerCustomerEntity
        {
            Id = id,
            AccountNumber = actualAccountNumber,
            Name = actualName,
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            LicenseNumber = actualLicenseNumber,
            LicenseExpiryDate = actualExpiryDate,
            ContactId = contactId,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = isDeleted
        };
    }

    private async Task SetupCustomersAsync(params VesselOwnerCustomerEntity[] customers)
    {
        foreach (var customer in customers)
        {
            await _repository.AddAsync(customer);
        }
    }

    #endregion

    #region GetByIdAsync Tests

    [Test]
    public async Task GetByIdAsync_ValidId_ReturnsCustomer()
    {
        // Arrange
        var customer = CreateVesselOwnerCustomer(1, "ACC-001", "John Doe");
        await SetupCustomersAsync(customer);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-001"));
        Assert.That(result.Name, Is.EqualTo("John Doe"));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.VesselOwnerProfile));
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
    public async Task GetByIdAsync_DeletedCustomer_ReturnsNull()
    {
        // Arrange
        var customer = CreateVesselOwnerCustomer(1, isDeleted: true);
        await SetupCustomersAsync(customer);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetVesselOwnersByLicenseAsync Tests

    [Test]
    public async Task GetVesselOwnersByLicenseAsync_ValidLicense_ReturnsMatchingCustomers()
    {
        // Arrange
        var customer1 = CreateVesselOwnerCustomer(1, licenseNumber: "LIC-123456");
        var customer2 = CreateVesselOwnerCustomer(2, licenseNumber: "LIC-123456");
        var customer3 = CreateVesselOwnerCustomer(3, licenseNumber: "LIC-789012");
        await SetupCustomersAsync(customer1, customer2, customer3);

        // Act
        var result = await _repository.GetVesselOwnersByLicenseAsync("LIC-123456");

        // Assert
        var customers = result.ToList();
        Assert.That(customers, Has.Count.EqualTo(2));
        Assert.That(customers.All(c => c.LicenseNumber == "LIC-123456"), Is.True);
    }

    [Test]
    public async Task GetVesselOwnersByLicenseAsync_NonExistentLicense_ReturnsEmpty()
    {
        // Arrange
        var customer = CreateVesselOwnerCustomer(1, licenseNumber: "LIC-123456");
        await SetupCustomersAsync(customer);

        // Act
        var result = await _repository.GetVesselOwnersByLicenseAsync("LIC-999999");

        // Assert
        Assert.That(result.ToList(), Is.Empty);
    }

    #endregion

    #region GetVesselOwnersWithExpiredLicensesAsync Tests

    [Test]
    public async Task GetVesselOwnersWithExpiredLicensesAsync_ExpiredLicenses_ReturnsMatchingCustomers()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var customer1 = CreateVesselOwnerCustomer(1, licenseExpiryDate: now.AddDays(-30)); // Expired
        var customer2 = CreateVesselOwnerCustomer(2, licenseExpiryDate: now.AddDays(-1));  // Expired
        var customer3 = CreateVesselOwnerCustomer(3, licenseExpiryDate: now.AddDays(30));  // Not expired
        await SetupCustomersAsync(customer1, customer2, customer3);

        // Act
        var result = await _repository.GetVesselOwnersWithExpiredLicensesAsync();

        // Assert
        var customers = result.ToList();
        Assert.That(customers, Has.Count.EqualTo(2));
        Assert.That(customers.All(c => c.LicenseExpiryDate < now), Is.True);
    }

    [Test]
    public async Task GetVesselOwnersWithExpiredLicensesAsync_NoExpiredLicenses_ReturnsEmpty()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var customer1 = CreateVesselOwnerCustomer(1, licenseExpiryDate: now.AddDays(30));
        var customer2 = CreateVesselOwnerCustomer(2, licenseExpiryDate: now.AddDays(60));
        await SetupCustomersAsync(customer1, customer2);

        // Act
        var result = await _repository.GetVesselOwnersWithExpiredLicensesAsync();

        // Assert
        Assert.That(result.ToList(), Is.Empty);
    }

    #endregion

    #region SearchCustomersAsync Tests

    [Test]
    public async Task SearchCustomersAsync_NameSearch_ReturnsMatchingCustomers()
    {
        // Arrange
        var customer1 = CreateVesselOwnerCustomer(1, name: "John Smith");
        var customer2 = CreateVesselOwnerCustomer(2, name: "Jane Smith");
        var customer3 = CreateVesselOwnerCustomer(3, name: "Bob Johnson");
        await SetupCustomersAsync(customer1, customer2, customer3);

        // Act
        var result = await _repository.SearchCustomersAsync("Smith");

        // Assert
        var customers = result.ToList();
        Assert.That(customers, Has.Count.EqualTo(2));
        Assert.That(customers.All(c => c.Name.Contains("Smith")), Is.True);
    }

    [Test]
    public async Task SearchCustomersAsync_AccountNumberSearch_ReturnsMatchingCustomers()
    {
        // Arrange
        var customer1 = CreateVesselOwnerCustomer(1, accountNumber: "ACC-123456");
        var customer2 = CreateVesselOwnerCustomer(2, accountNumber: "ACC-789012");
        await SetupCustomersAsync(customer1, customer2);

        // Act
        var result = await _repository.SearchCustomersAsync("123456");

        // Assert
        var customers = result.ToList();
        Assert.That(customers, Has.Count.EqualTo(1));
        Assert.That(customers.First().AccountNumber, Is.EqualTo("ACC-123456"));
    }

    #endregion

    #region CRUD Operations Tests

    [Test]
    public async Task AddAsync_ValidCustomer_AddsSuccessfully()
    {
        // Arrange
        var customer = CreateVesselOwnerCustomer(1, "ACC-001", "John Doe");

        // Act
        await _repository.AddAsync(customer);
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-001"));
        Assert.That(result.Name, Is.EqualTo("John Doe"));
    }

    [Test]
    public async Task UpdateAsync_ValidCustomer_UpdatesSuccessfully()
    {
        // Arrange
        var customer = CreateVesselOwnerCustomer(1, "ACC-001", "John Doe");
        await SetupCustomersAsync(customer);

        // Act
        customer.Name = "John Updated";
        await _repository.UpdateAsync(customer);
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("John Updated"));
    }

    [Test]
    public async Task DeleteAsync_ValidCustomer_MarksAsDeleted()
    {
        // Arrange
        var customer = CreateVesselOwnerCustomer(1, "ACC-001", "John Doe");
        await SetupCustomersAsync(customer);

        // Act
        await _repository.DeleteAsync(customer.Id);
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Null); // Should return null for deleted customers
    }

    #endregion

    #region Paging Tests

    [Test]
    public async Task GetPagedAsync_WithPaging_ReturnsCorrectPage()
    {
        // Arrange
        var customers = new[]
        {
            CreateVesselOwnerCustomer(1, "ACC-001", "Customer 1"),
            CreateVesselOwnerCustomer(2, "ACC-002", "Customer 2"),
            CreateVesselOwnerCustomer(3, "ACC-003", "Customer 3"),
            CreateVesselOwnerCustomer(4, "ACC-004", "Customer 4"),
            CreateVesselOwnerCustomer(5, "ACC-005", "Customer 5")
        };
        await SetupCustomersAsync(customers);

        var pagingParameters = new PagingParameters<VesselOwnerCustomerEntity>
        {
            Page = 2,
            PageSize = 2
        };

        // Act
        var result = await _repository.GetPagedAsync(null, pagingParameters);

        // Assert
        Assert.That(result.Items.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetPageCountAsync_WithCustomers_ReturnsCorrectCount()
    {
        // Arrange
        var customers = new[]
        {
            CreateVesselOwnerCustomer(1),
            CreateVesselOwnerCustomer(2),
            CreateVesselOwnerCustomer(3)
        };
        await SetupCustomersAsync(customers);

        var pagingParameters = new PagingParameters<VesselOwnerCustomerEntity>
        {
            PageSize = 2
        };

        // Act
        var result = await _repository.GetPageCountAsync(null, pagingParameters);

        // Assert
        Assert.That(result, Is.EqualTo(2)); // 3 items with page size 2 = 2 pages
    }

    #endregion
} 