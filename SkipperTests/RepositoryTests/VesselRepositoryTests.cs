using Microsoft.Extensions.Logging;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public class VesselRepositoryTests : RepositoryTestBase<VesselEntity, VesselModel>
{
    private VesselRepository _repository;
    protected ILogger<VesselRepository> Logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        Logger = TestSetup.CreateLogger<VesselRepository>();
        _repository = new VesselRepository(Context, Logger);
    }

    #region Helper Methods

    private VesselEntity CreateVessel(long id, string name, string registrationNumber, VesselType type = VesselType.Sailboat, decimal length = 25.0m, decimal beam = 8.0m)
    {
        return new VesselEntity
        {
            Id = id,
            Name = name,
            RegistrationNumber = registrationNumber,
            VesselType = type,
            Length = length,
            Beam = beam,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    private async Task SetupVesselsAsync(params VesselEntity[] vessels)
    {
        foreach (var vessel in vessels)
        {
            await _repository.AddAsync(vessel);
        }
        await _repository.SaveChangesAsync();
    }

    #endregion

    #region GetByRegistrationNumberAsync Tests

    [Test]
    public async Task GetByRegistrationNumberAsync_ValidRegistration_ReturnsVessel()
    {
        // Arrange
        var vessels = new[]
        {
            CreateVessel(1, "Sea Breeze", "REG001", VesselType.Sailboat),
            CreateVessel(2, "Ocean Explorer", "REG002", VesselType.Motorboat),
            CreateVessel(3, "Wave Runner", "REG003", VesselType.Other)
        };

        await SetupVesselsAsync(vessels);

        // Act
        var result = await _repository.GetByRegistrationNumberAsync("REG002");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.RegistrationNumber, Is.EqualTo("REG002"));
        Assert.That(result.Name, Is.EqualTo("Ocean Explorer"));
        Assert.That(result.VesselType, Is.EqualTo(VesselType.Motorboat));
    }

    [Test]
    public async Task GetByRegistrationNumberAsync_NonExistentRegistration_ReturnsNull()
    {
        // Arrange
        var vessels = new[]
        {
            CreateVessel(1, "Sea Breeze", "REG001"),
            CreateVessel(2, "Ocean Explorer", "REG002")
        };

        await SetupVesselsAsync(vessels);

        // Act
        var result = await _repository.GetByRegistrationNumberAsync("REG999");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByRegistrationNumberAsync_EmptyRegistration_ReturnsNull()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        await SetupVesselsAsync(vessel);

        // Act
        var result = await _repository.GetByRegistrationNumberAsync("");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByRegistrationNumberAsync_NullRegistration_ReturnsNull()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        await SetupVesselsAsync(vessel);

        // Act
        var result = await _repository.GetByRegistrationNumberAsync(null!);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetByTypeAsync Tests

    [Test]
    public async Task GetByTypeAsync_Sailboat_ReturnsOnlySailboats()
    {
        // Arrange
        var vessels = new[]
        {
            CreateVessel(1, "Sea Breeze", "REG001", VesselType.Sailboat),
            CreateVessel(2, "Wind Dancer", "REG002", VesselType.Sailboat),
            CreateVessel(3, "Ocean Explorer", "REG003", VesselType.Motorboat),
            CreateVessel(4, "Wave Runner", "REG004", VesselType.Other)
        };

        await SetupVesselsAsync(vessels);

        // Act
        var result = await _repository.GetByTypeAsync(VesselType.Sailboat);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(v => v.VesselType == VesselType.Sailboat), Is.True);
        Assert.That(result.Select(v => v.Name), Is.EquivalentTo(new[] { "Sea Breeze", "Wind Dancer" }));
    }

    [TestCase(VesselType.Sailboat)]
    [TestCase(VesselType.Motorboat)]
    [TestCase(VesselType.Other)]
    public async Task GetByTypeAsync_AllTypes_ReturnsCorrectVessels(VesselType targetType)
    {
        // Arrange
        var vessels = Enum.GetValues<VesselType>()
            .Select((type, index) => CreateVessel(index + 1, $"Vessel{index + 1}", $"REG{index + 1:000}", type))
            .ToArray();

        await SetupVesselsAsync(vessels);

        // Act
        var result = await _repository.GetByTypeAsync(targetType);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().VesselType, Is.EqualTo(targetType));
    }

    [Test]
    public async Task GetByTypeAsync_NoMatchingType_ReturnsEmpty()
    {
        // Arrange
        var vessels = new[]
        {
            CreateVessel(1, "Sea Breeze", "REG001", VesselType.Sailboat),
            CreateVessel(2, "Wind Dancer", "REG002", VesselType.Sailboat)
        };

        await SetupVesselsAsync(vessels);

        // Act
        var result = await _repository.GetByTypeAsync(VesselType.Motorboat);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    #endregion

    #region SearchByNameAsync Tests

    [Test]
    public async Task SearchByNameAsync_PartialMatch_ReturnsMatchingVessels()
    {
        // Arrange
        var vessels = new[]
        {
            CreateVessel(1, "Sea Breeze", "REG001"),
            CreateVessel(2, "Sea Explorer", "REG002"),
            CreateVessel(3, "Ocean Wave", "REG003"),
            CreateVessel(4, "Wind Dancer", "REG004")
        };

        await SetupVesselsAsync(vessels);

        // Act
        var result = await _repository.SearchByNameAsync("Sea");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(v => v.Name), Is.EquivalentTo(new[] { "Sea Breeze", "Sea Explorer" }));
    }

    [Test]
    public async Task SearchByNameAsync_CaseInsensitive_ReturnsMatchingVessels()
    {
        // Arrange
        var vessels = new[]
        {
            CreateVessel(1, "Sea Breeze", "REG001"),
            CreateVessel(2, "OCEAN Explorer", "REG002"),
            CreateVessel(3, "wind dancer", "REG003")
        };

        await SetupVesselsAsync(vessels);

        // Act
        var result = await _repository.SearchByNameAsync("sea");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Sea Breeze"));
    }

    [Test]
    public async Task SearchByNameAsync_NoMatches_ReturnsEmpty()
    {
        // Arrange
        var vessels = new[]
        {
            CreateVessel(1, "Sea Breeze", "REG001"),
            CreateVessel(2, "Ocean Explorer", "REG002")
        };

        await SetupVesselsAsync(vessels);

        // Act
        var result = await _repository.SearchByNameAsync("Submarine");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task SearchByNameAsync_EmptySearchTerm_ReturnsEmpty()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        await SetupVesselsAsync(vessel);

        // Act
        var result = await _repository.SearchByNameAsync("");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task SearchByNameAsync_NullSearchTerm_ReturnsEmpty()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        await SetupVesselsAsync(vessel);

        // Act
        var result = await _repository.SearchByNameAsync(null!);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    #endregion

    #region Base Repository CRUD Tests

    [Test]
    public async Task GetByIdAsync_ValidId_ReturnsVessel()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        await SetupVesselsAsync(vessel);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Sea Breeze"));
        Assert.That(result.RegistrationNumber, Is.EqualTo("REG001"));
    }

    [Test]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        await SetupVesselsAsync(vessel);

        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllVessels()
    {
        // Arrange
        var vessels = new[]
        {
            CreateVessel(1, "Sea Breeze", "REG001"),
            CreateVessel(2, "Ocean Explorer", "REG002"),
            CreateVessel(3, "Wind Dancer", "REG003")
        };

        await SetupVesselsAsync(vessels);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.Select(v => v.Name), Is.EquivalentTo(new[] { "Sea Breeze", "Ocean Explorer", "Wind Dancer" }));
    }

    [Test]
    public async Task AddAsync_ValidVessel_AddsVessel()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");

        // Act
        var result = await _repository.AddAsync(vessel);
        await _repository.SaveChangesAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Sea Breeze"));

        var savedVessel = await _repository.GetByIdAsync(1);
        Assert.That(savedVessel, Is.Not.Null);
        Assert.That(savedVessel.Name, Is.EqualTo("Sea Breeze"));
    }

    [Test]
    public async Task UpdateAsync_ValidVessel_UpdatesVessel()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        await SetupVesselsAsync(vessel);

        // Act
        vessel.Name = "Ocean Breeze";
        vessel.Length = 30.0m;
        vessel.UpdatedAt = DateTime.UtcNow.AddHours(1);
        await _repository.UpdateAsync(vessel);
        await _repository.SaveChangesAsync();

        // Assert
        var updatedVessel = await _repository.GetByIdAsync(1);
        Assert.That(updatedVessel, Is.Not.Null);
        Assert.That(updatedVessel.Name, Is.EqualTo("Ocean Breeze"));
        Assert.That(updatedVessel.Length, Is.EqualTo(30.0m));
    }

    [Test]
    public async Task DeleteAsync_ValidId_SoftDeletesVessel()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        await SetupVesselsAsync(vessel);

        // Act
        await _repository.DeleteAsync(1);
        await _repository.SaveChangesAsync();

        // Assert - Repository methods should not return the soft deleted entity
        var deletedVessel = await _repository.GetByIdAsync(1);
        Assert.That(deletedVessel, Is.Null);

        // Verify it's not in GetAll either
        var allVessels = await _repository.GetAllAsync();
        Assert.That(allVessels.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task ExistsAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        await SetupVesselsAsync(vessel);

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsAsync_NonExistentId_ReturnsFalse()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        await SetupVesselsAsync(vessel);

        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task FindAsync_WithPredicate_ReturnsMatchingVessels()
    {
        // Arrange
        var vessels = new[]
        {
            CreateVessel(1, "Sea Breeze", "REG001", VesselType.Sailboat, 25.0m),
            CreateVessel(2, "Ocean Explorer", "REG002", VesselType.Sailboat, 35.0m),
            CreateVessel(3, "Wind Dancer", "REG003", VesselType.Motorboat, 40.0m)
        };

        await SetupVesselsAsync(vessels);

        // Act
        var result = await _repository.FindAsync(v => v.Length > 30.0m);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(v => v.Length > 30.0m), Is.True);
        Assert.That(result.Select(v => v.Name), Is.EquivalentTo(new[] { "Ocean Explorer", "Wind Dancer" }));
    }

    #endregion

    #region Soft Delete Tests

    [Test]
    public async Task GetByIdAsync_SoftDeletedEntity_ReturnsNull()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        vessel.IsDeleted = true;
        await _repository.AddAsync(vessel);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeVessel = CreateVessel(1, "Sea Breeze", "REG001");
        var deletedVessel = CreateVessel(2, "Ocean Explorer", "REG002");
        deletedVessel.IsDeleted = true;

        await _repository.AddAsync(activeVessel);
        await _repository.AddAsync(deletedVessel);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Sea Breeze"));
        Assert.That(result.All(v => !v.IsDeleted), Is.True);
    }

    [Test]
    public async Task FindAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeVessel = CreateVessel(1, "Sea Breeze", "REG001", VesselType.Sailboat, 25.0m);
        var deletedVessel = CreateVessel(2, "Ocean Explorer", "REG002", VesselType.Sailboat, 35.0m);
        deletedVessel.IsDeleted = true;
        var anotherActiveVessel = CreateVessel(3, "Wind Dancer", "REG003", VesselType.Motorboat, 30.0m);

        await _repository.AddAsync(activeVessel);
        await _repository.AddAsync(deletedVessel);
        await _repository.AddAsync(anotherActiveVessel);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(v => v.Length >= 25.0m);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(v => v.Name), Is.EquivalentTo(new[] { "Sea Breeze", "Wind Dancer" }));
        Assert.That(result.All(v => !v.IsDeleted), Is.True);
    }

    [Test]
    public async Task GetByRegistrationNumberAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeVessel = CreateVessel(1, "Sea Breeze", "REG001");
        var deletedVessel = CreateVessel(2, "Ocean Explorer", "REG001"); // Same registration but deleted
        deletedVessel.IsDeleted = true;

        await _repository.AddAsync(activeVessel);
        await _repository.AddAsync(deletedVessel);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetByRegistrationNumberAsync("REG001");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1)); // Should return the active one
        Assert.That(result.IsDeleted, Is.False);
    }

    [Test]
    public async Task GetByRegistrationNumberAsync_SoftDeletedRegistration_ReturnsNull()
    {
        // Arrange
        var deletedVessel = CreateVessel(1, "Sea Breeze", "REG001");
        deletedVessel.IsDeleted = true;
        await _repository.AddAsync(deletedVessel);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetByRegistrationNumberAsync("REG001");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByTypeAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeVessel = CreateVessel(1, "Sea Breeze", "REG001", VesselType.Sailboat);
        var deletedVessel = CreateVessel(2, "Ocean Explorer", "REG002", VesselType.Sailboat);
        deletedVessel.IsDeleted = true;
        var anotherActiveVessel = CreateVessel(3, "Wind Dancer", "REG003", VesselType.Sailboat);

        await _repository.AddAsync(activeVessel);
        await _repository.AddAsync(deletedVessel);
        await _repository.AddAsync(anotherActiveVessel);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTypeAsync(VesselType.Sailboat);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(v => v.Name), Is.EquivalentTo(new[] { "Sea Breeze", "Wind Dancer" }));
        Assert.That(result.All(v => !v.IsDeleted), Is.True);
    }

    [Test]
    public async Task SearchByNameAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeVessel = CreateVessel(1, "Sea Breeze", "REG001");
        var deletedVessel = CreateVessel(2, "Sea Explorer", "REG002");
        deletedVessel.IsDeleted = true;
        var anotherActiveVessel = CreateVessel(3, "Ocean Wave", "REG003");

        await _repository.AddAsync(activeVessel);
        await _repository.AddAsync(deletedVessel);
        await _repository.AddAsync(anotherActiveVessel);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.SearchByNameAsync("Sea");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Sea Breeze"));
        Assert.That(result.All(v => !v.IsDeleted), Is.True);
    }

    [Test]
    public async Task ExistsAsync_SoftDeletedEntity_ReturnsFalse()
    {
        // Arrange
        var vessel = CreateVessel(1, "Sea Breeze", "REG001");
        vessel.IsDeleted = true;
        await _repository.AddAsync(vessel);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion
} 