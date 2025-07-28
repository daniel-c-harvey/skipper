using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public class BusinessCustomerRepositoryTests : RepositoryTestBase<BusinessCustomerEntity>
{
    private BusinessCustomerRepository _repository;
    protected ILogger<BusinessCustomerRepository> Logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        Logger = TestSetup.CreateLogger<BusinessCustomerRepository>();
        _repository = new BusinessCustomerRepository(Context, Logger);
    }

    #region Helper Methods

    private BusinessCustomerEntity CreateBusinessCustomer(
        long id,
        string accountNumber = null,
        string name = null,
        string businessName = null,
        string taxId = null,
        bool isDeleted = false)
    {
        var now = DateTime.UtcNow;
        var actualAccountNumber = accountNumber ?? $"ACC-{id:D6}";
        var actualName = name ?? $"Business Customer {id}";
        var actualBusinessName = businessName ?? $"Business {id} Corp";
        var actualTaxId = taxId ?? $"TAX-{id:D6}";

        return new BusinessCustomerEntity
        {
            Id = id,
            AccountNumber = actualAccountNumber,
            Name = actualName,
            CustomerProfileType = CustomerProfileType.BusinessCustomerProfile,
            BusinessName = actualBusinessName,
            TaxId = actualTaxId,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = isDeleted
        };
    }

    private async Task SetupCustomersAsync(params BusinessCustomerEntity[] customers)
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
        var customer = CreateBusinessCustomer(1, "ACC-001", "Acme Corp");
        await SetupCustomersAsync(customer);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-001"));
        Assert.That(result.Name, Is.EqualTo("Acme Corp"));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.BusinessCustomerProfile));
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
        var customer = CreateBusinessCustomer(1, isDeleted: true);
        await SetupCustomersAsync(customer);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetBusinessCustomersByNameAsync Tests

    [Test]
    public async Task GetBusinessCustomersByNameAsync_ValidName_ReturnsMatchingCustomers()
    {
        // Arrange
        var customer1 = CreateBusinessCustomer(1, businessName: "Acme Corporation");
        var customer2 = CreateBusinessCustomer(2, businessName: "Acme Industries");
        var customer3 = CreateBusinessCustomer(3, businessName: "Beta Corp");
        await SetupCustomersAsync(customer1, customer2, customer3);

        // Act
        var result = await _repository.GetBusinessCustomersByNameAsync("Acme");

        // Assert
        var customers = result.ToList();
        Assert.That(customers, Has.Count.EqualTo(2));
        Assert.That(customers.All(c => c.BusinessName.Contains("Acme")), Is.True);
    }

    [Test]
    public async Task GetBusinessCustomersByNameAsync_NonExistentName_ReturnsEmpty()
    {
        // Arrange
        var customer = CreateBusinessCustomer(1, businessName: "Acme Corporation");
        await SetupCustomersAsync(customer);

        // Act
        var result = await _repository.GetBusinessCustomersByNameAsync("XYZ");

        // Assert
        Assert.That(result.ToList(), Is.Empty);
    }

    #endregion

    #region GetByTaxIdAsync Tests

    [Test]
    public async Task GetByTaxIdAsync_ValidTaxId_ReturnsCustomer()
    {
        // Arrange
        var customer = CreateBusinessCustomer(1, taxId: "TAX-123456");
        await SetupCustomersAsync(customer);

        // Act
        var result = await _repository.GetByTaxIdAsync("TAX-123456");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TaxId, Is.EqualTo("TAX-123456"));
    }

    [Test]
    public async Task GetByTaxIdAsync_NonExistentTaxId_ReturnsNull()
    {
        // Arrange
        var customer = CreateBusinessCustomer(1, taxId: "TAX-123456");
        await SetupCustomersAsync(customer);

        // Act
        var result = await _repository.GetByTaxIdAsync("TAX-999999");

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetActiveCustomersAsync Tests

    [Test]
    public async Task GetActiveCustomersAsync_ActiveCustomers_ReturnsNonDeletedCustomers()
    {
        // Arrange
        var customer1 = CreateBusinessCustomer(1, isDeleted: false);
        var customer2 = CreateBusinessCustomer(2, isDeleted: false);
        var customer3 = CreateBusinessCustomer(3, isDeleted: true);
        await SetupCustomersAsync(customer1, customer2, customer3);

        // Act
        var result = await _repository.GetActiveCustomersAsync();

        // Assert
        var customers = result.ToList();
        Assert.That(customers, Has.Count.EqualTo(2));
        Assert.That(customers.All(c => !c.IsDeleted), Is.True);
    }

    #endregion

    #region SearchCustomersAsync Tests

    [Test]
    public async Task SearchCustomersAsync_NameSearch_ReturnsMatchingCustomers()
    {
        // Arrange
        var customer1 = CreateBusinessCustomer(1, name: "Acme Industries");
        var customer2 = CreateBusinessCustomer(2, name: "Acme Corporation");
        var customer3 = CreateBusinessCustomer(3, name: "Beta Corp");
        await SetupCustomersAsync(customer1, customer2, customer3);

        // Act
        var result = await _repository.SearchCustomersAsync("Acme");

        // Assert
        var customers = result.ToList();
        Assert.That(customers, Has.Count.EqualTo(2));
        Assert.That(customers.All(c => c.Name.Contains("Acme")), Is.True);
    }

    [Test]
    public async Task SearchCustomersAsync_BusinessNameSearch_ReturnsMatchingCustomers()
    {
        // Arrange
        var customer1 = CreateBusinessCustomer(1, businessName: "Global Tech Solutions");
        var customer2 = CreateBusinessCustomer(2, businessName: "Local Services Inc");
        await SetupCustomersAsync(customer1, customer2);

        // Act
        var result = await _repository.SearchCustomersAsync("Global");

        // Assert
        var customers = result.ToList();
        Assert.That(customers, Has.Count.EqualTo(1));
        Assert.That(customers.First().BusinessName, Is.EqualTo("Global Tech Solutions"));
    }

    #endregion

    #region CRUD Operations Tests

    [Test]
    public async Task AddAsync_ValidCustomer_AddsSuccessfully()
    {
        // Arrange
        var customer = CreateBusinessCustomer(1, "ACC-001", "Acme Corp");

        // Act
        await _repository.AddAsync(customer);
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-001"));
        Assert.That(result.Name, Is.EqualTo("Acme Corp"));
    }

    [Test]
    public async Task UpdateAsync_ValidCustomer_UpdatesSuccessfully()
    {
        // Arrange
        var customer = CreateBusinessCustomer(1, "ACC-001", "Acme Corp");
        await SetupCustomersAsync(customer);

        // Act
        customer.Name = "Acme Updated";
        await _repository.UpdateAsync(customer);
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Acme Updated"));
    }

    [Test]
    public async Task DeleteAsync_ValidCustomer_MarksAsDeleted()
    {
        // Arrange
        var customer = CreateBusinessCustomer(1, "ACC-001", "Acme Corp");
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
            CreateBusinessCustomer(1, "ACC-001", "Customer 1"),
            CreateBusinessCustomer(2, "ACC-002", "Customer 2"),
            CreateBusinessCustomer(3, "ACC-003", "Customer 3"),
            CreateBusinessCustomer(4, "ACC-004", "Customer 4"),
            CreateBusinessCustomer(5, "ACC-005", "Customer 5")
        };
        await SetupCustomersAsync(customers);

        var pagingParameters = new PagingParameters<BusinessCustomerEntity>
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
            CreateBusinessCustomer(1),
            CreateBusinessCustomer(2),
            CreateBusinessCustomer(3)
        };
        await SetupCustomersAsync(customers);

        var pagingParameters = new PagingParameters<BusinessCustomerEntity>
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