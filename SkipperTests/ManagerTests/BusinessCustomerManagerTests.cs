using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperData.Data.Repositories;
using SkipperData.Managers;
using SkipperModels.Entities;
using SkipperModels.Models;
using SkipperModels;

namespace SkipperTests.ManagerTests;

[TestFixture]
public class BusinessCustomerManagerTests
{
    private SkipperContext _context;
    private BusinessCustomerRepository _repository;
    private BusinessCustomerManager _manager;
    private ILogger<BusinessCustomerRepository> _logger;

    [SetUp]
    public void SetUp()
    {
        _context = TestSetup.CreateContext();
        _logger = TestSetup.CreateLogger<BusinessCustomerRepository>();
        _repository = new BusinessCustomerRepository(_context, _logger);
        _manager = new BusinessCustomerManager(_repository);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    #region Helper Methods

    private BusinessCustomerEntity CreateBusinessCustomerEntity(
        long id,
        string accountNumber = "ACC-001",
        string name = "Acme Corp",
        string businessName = "Acme Corporation",
        string taxId = "TAX-123456",
        bool isDeleted = false)
    {
        var now = DateTime.UtcNow;
        return new BusinessCustomerEntity
        {
            Id = id,
            AccountNumber = accountNumber,
            Name = name,
            CustomerProfileType = CustomerProfileType.BusinessCustomerProfile,
            BusinessName = businessName,
            TaxId = taxId,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = isDeleted
        };
    }

    private BusinessCustomerModel CreateBusinessCustomerModel(
        long id,
        string accountNumber = "ACC-001",
        string name = "Acme Corp",
        string businessName = "Acme Corporation",
        string taxId = "TAX-123456")
    {
        var now = DateTime.UtcNow;
        return new BusinessCustomerModel
        {
            Id = id,
            AccountNumber = accountNumber,
            Name = name,
            CustomerProfileType = CustomerProfileType.BusinessCustomerProfile,
            BusinessName = businessName,
            TaxId = taxId,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    #endregion

    #region GetBusinessCustomersByNameAsync Tests

    [Test]
    public async Task GetBusinessCustomersByNameAsync_ValidName_ReturnsConvertedModels()
    {
        // Arrange
        var entities = new[]
        {
            CreateBusinessCustomerEntity(1, businessName: "Acme Corporation"),
            CreateBusinessCustomerEntity(2, businessName: "Acme Industries"),
            CreateBusinessCustomerEntity(3, businessName: "Beta Corp")
        };

        foreach (var entity in entities)
        {
            await _repository.AddAsync(entity);
        }

        // Act
        var result = await _manager.GetBusinessCustomersByNameAsync("Acme");

        // Assert
        var models = result.ToList();
        Assert.That(models, Has.Count.EqualTo(2));
        Assert.That(models.All(m => m.BusinessName.Contains("Acme")), Is.True);
        Assert.That(models.All(m => m.CustomerProfileType == CustomerProfileType.BusinessCustomerProfile), Is.True);
    }

    [Test]
    public async Task GetBusinessCustomersByNameAsync_EmptyResult_ReturnsEmptyCollection()
    {
        // Arrange
        var entity = CreateBusinessCustomerEntity(1, businessName: "Acme Corporation");
        await _repository.AddAsync(entity);

        // Act
        var result = await _manager.GetBusinessCustomersByNameAsync("NonExistent");

        // Assert
        Assert.That(result.ToList(), Is.Empty);
    }

    #endregion

    #region GetByTaxIdAsync Tests

    [Test]
    public async Task GetByTaxIdAsync_ValidTaxId_ReturnsConvertedModel()
    {
        // Arrange
        var taxId = "TAX-123456";
        var entity = CreateBusinessCustomerEntity(1, taxId: taxId);
        await _repository.AddAsync(entity);

        // Act
        var result = await _manager.GetByTaxIdAsync(taxId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TaxId, Is.EqualTo(taxId));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.BusinessCustomerProfile));
    }

    [Test]
    public async Task GetByTaxIdAsync_NonExistentTaxId_ReturnsNull()
    {
        // Arrange
        var entity = CreateBusinessCustomerEntity(1, taxId: "TAX-123456");
        await _repository.AddAsync(entity);

        // Act
        var result = await _manager.GetByTaxIdAsync("TAX-999999");

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetBusinessCustomersWithContactsAsync Tests

    [Test]
    public async Task GetBusinessCustomersWithContactsAsync_WithContacts_ReturnsConvertedModels()
    {
        // Arrange
        var entities = new[]
        {
            CreateBusinessCustomerEntity(1, name: "Business with Contacts 1"),
            CreateBusinessCustomerEntity(2, name: "Business with Contacts 2")
        };

        foreach (var entity in entities)
        {
            // Create a contact for this business customer
            var contact = new ContactEntity { Id = entity.Id, Email = $"business{entity.Id}@test.com", FirstName = $"Contact{entity.Id}", LastName = "Person" };
            entity.BusinessCustomerContacts.Add(new BusinessCustomerContactsEntity 
            { 
                BusinessCustomerId = entity.Id, 
                ContactId = contact.Id, 
                IsPrimary = true, 
                Contact = contact 
            });
            
            await _repository.AddAsync(entity);
        }

        // Act
        var result = await _manager.GetBusinessCustomersWithContactsAsync();

        // Assert
        var models = result.ToList();
        Assert.That(models, Has.Count.EqualTo(2));
        Assert.That(models.All(m => m.CustomerProfileType == CustomerProfileType.BusinessCustomerProfile), Is.True);
    }

    #endregion

    #region GetActiveCustomersAsync Tests (Inherited from base)

    [Test]
    public async Task GetActiveCustomersAsync_ActiveCustomers_ReturnsConvertedModels()
    {
        // Arrange
        var entities = new[]
        {
            CreateBusinessCustomerEntity(1, name: "Active Business 1"),
            CreateBusinessCustomerEntity(2, name: "Active Business 2"),
            CreateBusinessCustomerEntity(3, name: "Deleted Business", isDeleted: true)
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
        Assert.That(models.All(m => m.CustomerProfileType == CustomerProfileType.BusinessCustomerProfile), Is.True);
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
            CreateBusinessCustomerEntity(1, name: "Tech Solutions Inc"),
            CreateBusinessCustomerEntity(2, name: "Global Tech Corp"),
            CreateBusinessCustomerEntity(3, name: "Other Company")
        };

        foreach (var entity in entities)
        {
            await _repository.AddAsync(entity);
        }

        // Act
        var result = await _manager.SearchCustomersAsync("Tech");

        // Assert
        var models = result.ToList();
        Assert.That(models, Has.Count.EqualTo(2));
        Assert.That(models.All(m => m.Name.Contains("Tech")), Is.True);
        Assert.That(models.All(m => m.CustomerProfileType == CustomerProfileType.BusinessCustomerProfile), Is.True);
    }

    [Test]
    public async Task SearchCustomersAsync_NoMatches_ReturnsEmptyCollection()
    {
        // Arrange
        var entity = CreateBusinessCustomerEntity(1, name: "Acme Corporation");
        await _repository.AddAsync(entity);

        // Act
        var result = await _manager.SearchCustomersAsync("NonExistent");

        // Assert
        Assert.That(result.ToList(), Is.Empty);
    }

    #endregion
} 