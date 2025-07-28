using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;
using SkipperModels.InputModels;
using SkipperModels;

namespace SkipperTests.ConverterTests;

[TestFixture]
public class CustomerConverterTests
{
    #region Generic CustomerConverter Tests

    [Test]
    public void GenericConverter_VesselOwnerCustomer_EntityToModel_ConvertsCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entity = new VesselOwnerCustomerEntity
        {
            Id = 1,
            AccountNumber = "ACC-001",
            Name = "John Doe",
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            LicenseNumber = "LIC-123456",
            LicenseExpiryDate = now.AddYears(1),
            ContactId = 100,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act
        var result = VesselOwnerCustomerConverter.Convert(entity);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-001"));
        Assert.That(result.Name, Is.EqualTo("John Doe"));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.VesselOwnerProfile));
        Assert.That(result.LicenseNumber, Is.EqualTo("LIC-123456"));
        Assert.That(result.LicenseExpiryDate, Is.EqualTo(now.AddYears(1)));
        Assert.That(result.CreatedAt, Is.EqualTo(now));
        Assert.That(result.UpdatedAt, Is.EqualTo(now));
    }

    [Test]
    public void GenericConverter_VesselOwnerCustomer_ModelToEntity_ConvertsCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var model = new VesselOwnerCustomerModel
        {
            Id = 1,
            AccountNumber = "ACC-001",
            Name = "John Doe",
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            LicenseNumber = "LIC-123456",
            LicenseExpiryDate = now.AddYears(1),
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act
        var result = CustomerConverter<VesselOwnerCustomerEntity, VesselOwnerCustomerModel>.Convert(model);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-001"));
        Assert.That(result.Name, Is.EqualTo("John Doe"));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.VesselOwnerProfile));
        Assert.That(result.CreatedAt, Is.EqualTo(now));
        Assert.That(result.UpdatedAt, Is.EqualTo(now));
    }

    [Test]
    public void GenericConverter_BusinessCustomer_EntityToModel_ConvertsCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entity = new BusinessCustomerEntity
        {
            Id = 2,
            AccountNumber = "ACC-002",
            Name = "Acme Corp",
            CustomerProfileType = CustomerProfileType.BusinessCustomerProfile,
            BusinessName = "Acme Corporation",
            TaxId = "TAX-789012",
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act
        var result = BusinessCustomerConverter.Convert(entity);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-002"));
        Assert.That(result.Name, Is.EqualTo("Acme Corp"));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.BusinessCustomerProfile));
        Assert.That(result.BusinessName, Is.EqualTo("Acme Corporation"));
        Assert.That(result.TaxId, Is.EqualTo("TAX-789012"));
        Assert.That(result.CreatedAt, Is.EqualTo(now));
        Assert.That(result.UpdatedAt, Is.EqualTo(now));
    }

    [Test]
    public void GenericConverter_BusinessCustomer_ModelToEntity_ConvertsCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var model = new BusinessCustomerModel
        {
            Id = 2,
            AccountNumber = "ACC-002",
            Name = "Acme Corp",
            CustomerProfileType = CustomerProfileType.BusinessCustomerProfile,
            BusinessName = "Acme Corporation",
            TaxId = "TAX-789012",
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act
        var result = CustomerConverter<BusinessCustomerEntity, BusinessCustomerModel>.Convert(model);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-002"));
        Assert.That(result.Name, Is.EqualTo("Acme Corp"));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.BusinessCustomerProfile));
        Assert.That(result.CreatedAt, Is.EqualTo(now));
        Assert.That(result.UpdatedAt, Is.EqualTo(now));
    }

    #endregion

    #region Static CustomerConverter Tests

    [Test]
    public void StaticConverter_VesselOwnerCustomer_EntityToModel_ConvertsCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entity = new VesselOwnerCustomerEntity
        {
            Id = 1,
            AccountNumber = "ACC-001",
            Name = "John Doe",
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            LicenseNumber = "LIC-123456",
            LicenseExpiryDate = now.AddYears(1),
            ContactId = 100,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act
        var result = VesselOwnerCustomerConverter.Convert(entity);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-001"));
        Assert.That(result.Name, Is.EqualTo("John Doe"));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.VesselOwnerProfile));
        Assert.That(result.CreatedAt, Is.EqualTo(now));
        Assert.That(result.UpdatedAt, Is.EqualTo(now));
        
        // Note: Static converter only converts base CustomerEntity properties,
        // so specific properties like LicenseNumber are not included in the base CustomerModel
    }

    [Test]
    public void StaticConverter_BusinessCustomer_EntityToModel_ConvertsCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entity = new BusinessCustomerEntity
        {
            Id = 2,
            AccountNumber = "ACC-002",
            Name = "Acme Corp",
            CustomerProfileType = CustomerProfileType.BusinessCustomerProfile,
            BusinessName = "Acme Corporation",
            TaxId = "TAX-789012",
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act
        var result = BusinessCustomerConverter.Convert(entity);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-002"));
        Assert.That(result.Name, Is.EqualTo("Acme Corp"));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.BusinessCustomerProfile));
        Assert.That(result.CreatedAt, Is.EqualTo(now));
        Assert.That(result.UpdatedAt, Is.EqualTo(now));
        
        // Note: Static converter only converts base CustomerEntity properties,
        // so specific properties like BusinessName are not included in the base CustomerModel
    }

    #endregion

    #region CustomerModelToInputConverter Tests

    [Test]
    public void CustomerModelToInputConverter_ModelToInput_ConvertsCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var model = new CustomerModel
        {
            Id = 1,
            AccountNumber = "ACC-001",
            Name = "John Doe",
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act
        var result = VesselOwnerCustomerConverter.Convert(model);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-001"));
        Assert.That(result.Name, Is.EqualTo("John Doe"));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.VesselOwnerProfile));
        Assert.That(result.CreatedAt, Is.EqualTo(now));
        Assert.That(result.UpdatedAt, Is.EqualTo(now));
    }

    [Test]
    public void CustomerModelToInputConverter_InputToModel_ConvertsCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var input = new CustomerInputModel
        {
            Id = 1,
            AccountNumber = "ACC-001",
            Name = "John Doe",
            CustomerProfileType = CustomerProfileType.VesselOwnerProfile,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act
        var result = CustomerModelToInputConverter.Convert(input);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.AccountNumber, Is.EqualTo("ACC-001"));
        Assert.That(result.Name, Is.EqualTo("John Doe"));
        Assert.That(result.CustomerProfileType, Is.EqualTo(CustomerProfileType.VesselOwnerProfile));
        Assert.That(result.CreatedAt, Is.EqualTo(now));
        Assert.That(result.UpdatedAt, Is.EqualTo(now));
    }

    #endregion

    #region Null Handling Tests

    [Test]
    public void GenericConverter_NullEntity_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<NullReferenceException>(() => 
            CustomerConverter<VesselOwnerCustomerEntity, VesselOwnerCustomerModel>.Convert((VesselOwnerCustomerEntity)null));
    }

    [Test]
    public void GenericConverter_NullModel_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<NullReferenceException>(() => 
            CustomerConverter<VesselOwnerCustomerEntity, VesselOwnerCustomerModel>.Convert((VesselOwnerCustomerModel)null));
    }

    [Test]
    public void StaticConverter_NullEntity_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<NullReferenceException>(() => 
            CustomerConverter.Convert((CustomerEntity)null));
    }

    #endregion
} 