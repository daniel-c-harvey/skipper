using Microsoft.Extensions.Logging;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public class SlipRepositoryTests : RepositoryTestBase<SlipEntity>
{
    private SlipRepository _repository;
    protected ILogger<SlipRepository> Logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        Logger = TestSetup.CreateLogger<SlipRepository>();
        _repository = new SlipRepository(Context, Logger);
    }

    #region Helper Methods

    private SlipEntity CreateSlip(long id, SlipStatus status, string slipNumber, string locationCode = "LOC1")
    {
        return new SlipEntity
        {
            Id = id,
            Status = status,
            SlipNumber = slipNumber,
            LocationCode = locationCode,
            SlipClassificationId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    private async Task SetupSlipsAsync(params SlipEntity[] slips)
    {
        foreach (var slip in slips)
        {
            await _repository.AddAsync(slip);
        }
    }

    #endregion

    #region GetByStatusAsync Tests

    [Test]
    public async Task GetByStatusAsync_AvailableStatus_ReturnsOnlyAvailableSlips()
    {
        // Arrange
        var slips = new[]
        {
            CreateSlip(1, SlipStatus.Available, "A1"),
            CreateSlip(2, SlipStatus.Available, "A2"),
            CreateSlip(3, SlipStatus.Booked, "B1"),
            CreateSlip(4, SlipStatus.InUse, "C1")
        };

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.GetByStatusAsync(SlipStatus.Available);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(s => s.Status == SlipStatus.Available), Is.True);
        Assert.That(result.Select(s => s.SlipNumber), Is.EquivalentTo(new[] { "A1", "A2" }));
    }

    [Test]
    public async Task GetByStatusAsync_NoMatchingStatus_ReturnsEmpty()
    {
        // Arrange
        var slips = new[]
        {
            CreateSlip(1, SlipStatus.Available, "A1"),
            CreateSlip(2, SlipStatus.Booked, "B1")
        };

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.GetByStatusAsync(SlipStatus.Maintenance);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [TestCase(SlipStatus.Available)]
    [TestCase(SlipStatus.Booked)]
    [TestCase(SlipStatus.InUse)]
    [TestCase(SlipStatus.Maintenance)]
    [TestCase(SlipStatus.Archived)]
    public async Task GetByStatusAsync_AllStatuses_ReturnsCorrectSlips(SlipStatus targetStatus)
    {
        // Arrange
        var slips = Enum.GetValues<SlipStatus>()
            .Select((status, index) => CreateSlip(index + 1, status, $"S{index + 1}"))
            .ToArray();

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.GetByStatusAsync(targetStatus);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Status, Is.EqualTo(targetStatus));
    }

    #endregion

    #region GetAvailableAsync Tests

    [Test]
    public async Task GetAvailableAsync_ReturnsOnlyAvailableSlips()
    {
        // Arrange
        var slips = new[]
        {
            CreateSlip(1, SlipStatus.Available, "A1"),
            CreateSlip(2, SlipStatus.Available, "A2"),
            CreateSlip(3, SlipStatus.Booked, "B1"),
            CreateSlip(4, SlipStatus.InUse, "C1"),
            CreateSlip(5, SlipStatus.Available, "A3")
        };

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.GetAvailableAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.All(s => s.Status == SlipStatus.Available), Is.True);
        Assert.That(result.Select(s => s.SlipNumber), Is.EquivalentTo(new[] { "A1", "A2", "A3" }));
    }

    [Test]
    public async Task GetAvailableAsync_NoAvailableSlips_ReturnsEmpty()
    {
        // Arrange
        var slips = new[]
        {
            CreateSlip(1, SlipStatus.Booked, "B1"),
            CreateSlip(2, SlipStatus.InUse, "C1"),
            CreateSlip(3, SlipStatus.Maintenance, "M1")
        };

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.GetAvailableAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    #endregion

    #region GetBySlipNumberAsync Tests

    [Test]
    public async Task GetBySlipNumberAsync_ValidSlipNumber_ReturnsSlip()
    {
        // Arrange
        var slips = new[]
        {
            CreateSlip(1, SlipStatus.Available, "A1"),
            CreateSlip(2, SlipStatus.Booked, "B1"),
            CreateSlip(3, SlipStatus.InUse, "C1")
        };

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.GetBySlipNumberAsync("B1");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.SlipNumber, Is.EqualTo("B1"));
        Assert.That(result.Status, Is.EqualTo(SlipStatus.Booked));
    }

    [Test]
    public async Task GetBySlipNumberAsync_NonExistentSlipNumber_ReturnsNull()
    {
        // Arrange
        var slips = new[]
        {
            CreateSlip(1, SlipStatus.Available, "A1"),
            CreateSlip(2, SlipStatus.Booked, "B1")
        };

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.GetBySlipNumberAsync("X99");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetBySlipNumberAsync_EmptySlipNumber_ReturnsNull()
    {
        // Arrange
        var slips = new[]
        {
            CreateSlip(1, SlipStatus.Available, "A1")
        };

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.GetBySlipNumberAsync("");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetBySlipNumberAsync_NullSlipNumber_ReturnsNull()
    {
        // Arrange
        var slips = new[]
        {
            CreateSlip(1, SlipStatus.Available, "A1")
        };

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.GetBySlipNumberAsync(null!);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region Base Repository CRUD Tests

    [Test]
    public async Task GetByIdAsync_ValidId_ReturnsSlip()
    {
        // Arrange
        var slip = CreateSlip(1, SlipStatus.Available, "A1");
        await SetupSlipsAsync(slip);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.SlipNumber, Is.EqualTo("A1"));
    }

    [Test]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var slip = CreateSlip(1, SlipStatus.Available, "A1");
        await SetupSlipsAsync(slip);

        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllSlips()
    {
        // Arrange
        var slips = new[]
        {
            CreateSlip(1, SlipStatus.Available, "A1"),
            CreateSlip(2, SlipStatus.Booked, "B1"),
            CreateSlip(3, SlipStatus.InUse, "C1")
        };

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.Select(s => s.SlipNumber), Is.EquivalentTo(new[] { "A1", "B1", "C1" }));
    }

    [Test]
    public async Task AddAsync_ValidSlip_AddsSlip()
    {
        // Arrange
        var slip = CreateSlip(1, SlipStatus.Available, "A1");

        // Act
        var result = await _repository.AddAsync(slip);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.SlipNumber, Is.EqualTo("A1"));

        var savedSlip = await _repository.GetByIdAsync(1);
        Assert.That(savedSlip, Is.Not.Null);
        Assert.That(savedSlip.SlipNumber, Is.EqualTo("A1"));
    }

    [Test]
    public async Task UpdateAsync_ValidSlip_UpdatesSlip()
    {
        // Arrange
        var slip = CreateSlip(1, SlipStatus.Available, "A1");
        await SetupSlipsAsync(slip);

        // Act
        slip.Status = SlipStatus.Booked;
        slip.UpdatedAt = DateTime.UtcNow.AddHours(1);
        await _repository.UpdateAsync(slip);

        // Assert
        var updatedSlip = await _repository.GetByIdAsync(1);
        Assert.That(updatedSlip, Is.Not.Null);
        Assert.That(updatedSlip.Status, Is.EqualTo(SlipStatus.Booked));
    }

    [Test]
    public async Task DeleteAsync_ValidId_SoftDeletesSlip()
    {
        // Arrange
        var slip = CreateSlip(1, SlipStatus.Available, "A1");
        await SetupSlipsAsync(slip);

        // Act
        await _repository.DeleteAsync(1);

        // Assert - Repository methods should not return the soft deleted entity
        var deletedSlip = await _repository.GetByIdAsync(1);
        Assert.That(deletedSlip, Is.Null);

        // Verify it's not in GetAll either
        var allSlips = await _repository.GetAllAsync();
        Assert.That(allSlips.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task ExistsAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var slip = CreateSlip(1, SlipStatus.Available, "A1");
        await SetupSlipsAsync(slip);

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsAsync_NonExistentId_ReturnsFalse()
    {
        // Arrange
        var slip = CreateSlip(1, SlipStatus.Available, "A1");
        await SetupSlipsAsync(slip);

        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task FindAsync_WithPredicate_ReturnsMatchingSlips()
    {
        // Arrange
        var slips = new[]
        {
            CreateSlip(1, SlipStatus.Available, "A1", "LOC1"),
            CreateSlip(2, SlipStatus.Available, "A2", "LOC1"),
            CreateSlip(3, SlipStatus.Available, "B1", "LOC2")
        };

        await SetupSlipsAsync(slips);

        // Act
        var result = await _repository.FindAsync(s => s.LocationCode == "LOC1");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(s => s.LocationCode == "LOC1"), Is.True);
        Assert.That(result.Select(s => s.SlipNumber), Is.EquivalentTo(new[] { "A1", "A2" }));
    }

    #endregion

    #region Soft Delete Tests

    [Test]
    public async Task GetByIdAsync_SoftDeletedEntity_ReturnsNull()
    {
        // Arrange
        var slip = CreateSlip(1, SlipStatus.Available, "A1");
        slip.IsDeleted = true;
        await _repository.AddAsync(slip);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeSlip = CreateSlip(1, SlipStatus.Available, "A1");
        var deletedSlip = CreateSlip(2, SlipStatus.Booked, "B1");
        deletedSlip.IsDeleted = true;

        await _repository.AddAsync(activeSlip);
        await _repository.AddAsync(deletedSlip);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().SlipNumber, Is.EqualTo("A1"));
        Assert.That(result.All(s => !s.IsDeleted), Is.True);
    }

    [Test]
    public async Task FindAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeSlip = CreateSlip(1, SlipStatus.Available, "A1", "LOC1");
        var deletedSlip = CreateSlip(2, SlipStatus.Available, "A2", "LOC1");
        deletedSlip.IsDeleted = true;
        var anotherActiveSlip = CreateSlip(3, SlipStatus.Available, "A3", "LOC2");

        await _repository.AddAsync(activeSlip);
        await _repository.AddAsync(deletedSlip);
        await _repository.AddAsync(anotherActiveSlip);

        // Act
        var result = await _repository.FindAsync(s => s.LocationCode == "LOC1");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().SlipNumber, Is.EqualTo("A1"));
        Assert.That(result.All(s => !s.IsDeleted), Is.True);
    }

    [Test]
    public async Task GetByStatusAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeSlip = CreateSlip(1, SlipStatus.Available, "A1");
        var deletedSlip = CreateSlip(2, SlipStatus.Available, "A2");
        deletedSlip.IsDeleted = true;
        var anotherActiveSlip = CreateSlip(3, SlipStatus.Available, "A3");

        await _repository.AddAsync(activeSlip);
        await _repository.AddAsync(deletedSlip);
        await _repository.AddAsync(anotherActiveSlip);

        // Act
        var result = await _repository.GetByStatusAsync(SlipStatus.Available);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(s => s.SlipNumber), Is.EquivalentTo(new[] { "A1", "A3" }));
        Assert.That(result.All(s => !s.IsDeleted), Is.True);
    }

    [Test]
    public async Task GetAvailableAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeSlip = CreateSlip(1, SlipStatus.Available, "A1");
        var deletedSlip = CreateSlip(2, SlipStatus.Available, "A2");
        deletedSlip.IsDeleted = true;

        await _repository.AddAsync(activeSlip);
        await _repository.AddAsync(deletedSlip);

        // Act
        var result = await _repository.GetAvailableAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().SlipNumber, Is.EqualTo("A1"));
        Assert.That(result.All(s => !s.IsDeleted), Is.True);
    }

    [Test]
    public async Task GetBySlipNumberAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeSlip = CreateSlip(1, SlipStatus.Available, "A1");
        var deletedSlip = CreateSlip(2, SlipStatus.Available, "A1"); // Same slip number but deleted
        deletedSlip.IsDeleted = true;

        await _repository.AddAsync(activeSlip);
        await _repository.AddAsync(deletedSlip);

        // Act
        var result = await _repository.GetBySlipNumberAsync("A1");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1)); // Should return the active one
        Assert.That(result.IsDeleted, Is.False);
    }

    [Test]
    public async Task GetBySlipNumberAsync_SoftDeletedSlipNumber_ReturnsNull()
    {
        // Arrange
        var deletedSlip = CreateSlip(1, SlipStatus.Available, "A1");
        deletedSlip.IsDeleted = true;
        await _repository.AddAsync(deletedSlip);

        // Act
        var result = await _repository.GetBySlipNumberAsync("A1");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ExistsAsync_SoftDeletedEntity_ReturnsFalse()
    {
        // Arrange
        var slip = CreateSlip(1, SlipStatus.Available, "A1");
        slip.IsDeleted = true;
        await _repository.AddAsync(slip);

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion
} 