using Skipper.Data.Repositories;
using SkipperModels.Entities;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public class SlipClassificationRepositoryTests : RepositoryTestBase<SlipClassification>
{
    private SlipClassificationRepository _repository;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _repository = new SlipClassificationRepository(Context);
    }

    #region Helper Methods

    private SlipClassification CreateSlipClassification(long id, string name, decimal maxLength, decimal maxBeam, int basePrice, string description = "Test description")
    {
        return new SlipClassification
        {
            Id = id,
            Name = name,
            MaxLength = maxLength,
            MaxBeam = maxBeam,
            BasePrice = basePrice,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    private async Task SetupClassificationsAsync(params SlipClassification[] classifications)
    {
        foreach (var classification in classifications)
        {
            await _repository.AddAsync(classification);
        }
        await _repository.SaveChangesAsync();
    }

    #endregion

    #region GetByNameAsync Tests

    [Test]
    public async Task GetByNameAsync_ValidName_ReturnsClassification()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50),
            CreateSlipClassification(2, "Medium Slip", 35.0m, 12.0m, 75),
            CreateSlipClassification(3, "Large Slip", 50.0m, 16.0m, 100)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.GetByNameAsync("Medium Slip");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Medium Slip"));
        Assert.That(result.MaxLength, Is.EqualTo(35.0m));
        Assert.That(result.MaxBeam, Is.EqualTo(12.0m));
        Assert.That(result.BasePrice, Is.EqualTo(75));
    }

    [Test]
    public async Task GetByNameAsync_NonExistentName_ReturnsNull()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50),
            CreateSlipClassification(2, "Medium Slip", 35.0m, 12.0m, 75)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.GetByNameAsync("Super Large Slip");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByNameAsync_EmptyName_ReturnsNull()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        await SetupClassificationsAsync(classification);

        // Act
        var result = await _repository.GetByNameAsync("");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByNameAsync_NullName_ReturnsNull()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        await SetupClassificationsAsync(classification);

        // Act
        var result = await _repository.GetByNameAsync(null!);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region SearchByNameAsync Tests

    [Test]
    public async Task SearchByNameAsync_PartialMatch_ReturnsMatchingClassifications()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50),
            CreateSlipClassification(2, "Small Boat Slip", 30.0m, 9.0m, 60),
            CreateSlipClassification(3, "Medium Slip", 35.0m, 12.0m, 75),
            CreateSlipClassification(4, "Large Slip", 50.0m, 16.0m, 100)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.SearchByNameAsync("Small");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Small Slip", "Small Boat Slip" }));
    }

    [Test]
    public async Task SearchByNameAsync_CaseInsensitive_ReturnsMatchingClassifications()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50),
            CreateSlipClassification(2, "MEDIUM slip", 35.0m, 12.0m, 75),
            CreateSlipClassification(3, "large SLIP", 50.0m, 16.0m, 100)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.SearchByNameAsync("slip");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Small Slip", "MEDIUM slip", "large SLIP" }));
    }

    [Test]
    public async Task SearchByNameAsync_NoMatches_ReturnsEmpty()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50),
            CreateSlipClassification(2, "Medium Slip", 35.0m, 12.0m, 75)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.SearchByNameAsync("Berth");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task SearchByNameAsync_EmptySearchTerm_ReturnsEmpty()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        await SetupClassificationsAsync(classification);

        // Act
        var result = await _repository.SearchByNameAsync("");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task SearchByNameAsync_NullSearchTerm_ReturnsEmpty()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        await SetupClassificationsAsync(classification);

        // Act
        var result = await _repository.SearchByNameAsync(null!);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    #endregion

    #region GetByPriceRangeAsync Tests

    [Test]
    public async Task GetByPriceRangeAsync_ValidRange_ReturnsClassificationsInRange()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Budget Slip", 25.0m, 8.0m, 40),
            CreateSlipClassification(2, "Standard Slip", 30.0m, 9.0m, 60),
            CreateSlipClassification(3, "Premium Slip", 35.0m, 12.0m, 80),
            CreateSlipClassification(4, "Luxury Slip", 50.0m, 16.0m, 120)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.GetByPriceRangeAsync(50, 90);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(c => c.BasePrice >= 50 && c.BasePrice <= 90), Is.True);
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Standard Slip", "Premium Slip" }));
    }

    [Test]
    public async Task GetByPriceRangeAsync_InclusiveBoundaries_ReturnsClassificationsAtBoundaries()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Lower Boundary", 25.0m, 8.0m, 50),
            CreateSlipClassification(2, "Inside Range", 30.0m, 9.0m, 75),
            CreateSlipClassification(3, "Upper Boundary", 35.0m, 12.0m, 100),
            CreateSlipClassification(4, "Outside Range", 50.0m, 16.0m, 120)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.GetByPriceRangeAsync(50, 100);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Lower Boundary", "Inside Range", "Upper Boundary" }));
    }

    [Test]
    public async Task GetByPriceRangeAsync_NoClassificationsInRange_ReturnsEmpty()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Low Price", 25.0m, 8.0m, 30),
            CreateSlipClassification(2, "High Price", 50.0m, 16.0m, 150)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.GetByPriceRangeAsync(50, 100);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetByPriceRangeAsync_MinGreaterThanMax_ReturnsEmpty()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Standard Slip", 30.0m, 9.0m, 75);
        await SetupClassificationsAsync(classification);

        // Act
        var result = await _repository.GetByPriceRangeAsync(100, 50);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetByPriceRangeAsync_NegativeValues_HandlesCorrectly()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Free Slip", 25.0m, 8.0m, 0),
            CreateSlipClassification(2, "Standard Slip", 30.0m, 9.0m, 50)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.GetByPriceRangeAsync(-10, 25);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Free Slip"));
    }

    #endregion

    #region GetCompatibleForVesselAsync Tests

    [Test]
    public async Task GetCompatibleForVesselAsync_VesselFitsMultipleClassifications_ReturnsAllCompatible()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50),    // Compatible: 20x6 fits (20<=25, 6<=8)
            CreateSlipClassification(2, "Medium Slip", 30.0m, 10.0m, 75),  // Compatible: 20x6 fits (20<=30, 6<=10)
            CreateSlipClassification(3, "Large Slip", 15.0m, 16.0m, 100),  // Not compatible: length too small (20>15)
            CreateSlipClassification(4, "Wide Slip", 25.0m, 5.0m, 60)     // Not compatible: beam too small (6>5)
        };

        await SetupClassificationsAsync(classifications);

        // Act - Vessel with length=20, beam=6
        var result = await _repository.GetCompatibleForVesselAsync(20.0m, 6.0m);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Small Slip", "Medium Slip" }));
    }

    [Test]
    public async Task GetCompatibleForVesselAsync_VesselAtExactBoundaries_ReturnsCompatible()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Exact Fit Slip", 20.0m, 6.0m, 75),
            CreateSlipClassification(2, "Too Small Slip", 19.0m, 5.0m, 50)
        };

        await SetupClassificationsAsync(classifications);

        // Act - Vessel exactly at max boundaries: length=20, beam=6
        var result = await _repository.GetCompatibleForVesselAsync(20.0m, 6.0m);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Exact Fit Slip"));
    }

    [Test]
    public async Task GetCompatibleForVesselAsync_VesselTooLarge_ReturnsEmpty()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50),
            CreateSlipClassification(2, "Medium Slip", 35.0m, 12.0m, 75)
        };

        await SetupClassificationsAsync(classifications);

        // Act - Vessel too large: length=40, beam=15
        var result = await _repository.GetCompatibleForVesselAsync(40.0m, 15.0m);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetCompatibleForVesselAsync_ZeroDimensions_ReturnsAllClassifications()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50),
            CreateSlipClassification(2, "Medium Slip", 35.0m, 12.0m, 75)
        };

        await SetupClassificationsAsync(classifications);

        // Act - Zero dimensions should fit in all slips
        var result = await _repository.GetCompatibleForVesselAsync(0.0m, 0.0m);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetCompatibleForVesselAsync_NegativeDimensions_ReturnsAllClassifications()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Standard Slip", 30.0m, 10.0m, 75);
        await SetupClassificationsAsync(classification);

        // Act - Negative dimensions should technically fit (though unrealistic)
        var result = await _repository.GetCompatibleForVesselAsync(-5.0m, -3.0m);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    #endregion

    #region Base Repository CRUD Tests

    [Test]
    public async Task GetByIdAsync_ValidId_ReturnsSlipClassification()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        await SetupClassificationsAsync(classification);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Small Slip"));
        Assert.That(result.BasePrice, Is.EqualTo(50));
    }

    [Test]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        await SetupClassificationsAsync(classification);

        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllSlipClassifications()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50),
            CreateSlipClassification(2, "Medium Slip", 35.0m, 12.0m, 75),
            CreateSlipClassification(3, "Large Slip", 50.0m, 16.0m, 100)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Small Slip", "Medium Slip", "Large Slip" }));
    }

    [Test]
    public async Task AddAsync_ValidSlipClassification_AddsSlipClassification()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);

        // Act
        var result = await _repository.AddAsync(classification);
        await _repository.SaveChangesAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Small Slip"));

        var savedClassification = await _repository.GetByIdAsync(1);
        Assert.That(savedClassification, Is.Not.Null);
        Assert.That(savedClassification.Name, Is.EqualTo("Small Slip"));
    }

    [Test]
    public async Task UpdateAsync_ValidSlipClassification_UpdatesSlipClassification()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        await SetupClassificationsAsync(classification);

        // Act
        classification.Name = "Updated Small Slip";
        classification.BasePrice = 60;
        classification.UpdatedAt = DateTime.UtcNow.AddHours(1);
        await _repository.UpdateAsync(classification);
        await _repository.SaveChangesAsync();

        // Assert
        var updatedClassification = await _repository.GetByIdAsync(1);
        Assert.That(updatedClassification, Is.Not.Null);
        Assert.That(updatedClassification.Name, Is.EqualTo("Updated Small Slip"));
        Assert.That(updatedClassification.BasePrice, Is.EqualTo(60));
    }

    [Test]
    public async Task DeleteAsync_ValidId_SoftDeletesSlipClassification()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        await SetupClassificationsAsync(classification);

        // Act
        await _repository.DeleteAsync(1);
        await _repository.SaveChangesAsync();

        // Assert - Repository methods should not return the soft deleted entity
        var deletedClassification = await _repository.GetByIdAsync(1);
        Assert.That(deletedClassification, Is.Null);

        // Verify it's not in GetAll either
        var allClassifications = await _repository.GetAllAsync();
        Assert.That(allClassifications.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task ExistsAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        await SetupClassificationsAsync(classification);

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsAsync_NonExistentId_ReturnsFalse()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        await SetupClassificationsAsync(classification);

        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task FindAsync_WithPredicate_ReturnsMatchingSlipClassifications()
    {
        // Arrange
        var classifications = new[]
        {
            CreateSlipClassification(1, "Budget Slip", 25.0m, 8.0m, 40),
            CreateSlipClassification(2, "Standard Slip", 30.0m, 9.0m, 75),
            CreateSlipClassification(3, "Premium Slip", 35.0m, 12.0m, 120)
        };

        await SetupClassificationsAsync(classifications);

        // Act
        var result = await _repository.FindAsync(c => c.BasePrice >= 70);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(c => c.BasePrice >= 70), Is.True);
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Standard Slip", "Premium Slip" }));
    }

    #endregion

    #region Soft Delete Tests

    [Test]
    public async Task GetByIdAsync_SoftDeletedEntity_ReturnsNull()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Deleted Slip", 25.0m, 8.0m, 50);
        classification.IsDeleted = true;
        await _repository.AddAsync(classification);
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
        var activeClassification = CreateSlipClassification(1, "Active Slip", 25.0m, 8.0m, 50);
        var deletedClassification = CreateSlipClassification(2, "Deleted Slip", 35.0m, 12.0m, 75);
        deletedClassification.IsDeleted = true;

        await _repository.AddAsync(activeClassification);
        await _repository.AddAsync(deletedClassification);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Active Slip"));
        Assert.That(result.All(c => !c.IsDeleted), Is.True);
    }

    [Test]
    public async Task FindAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var smallSlip = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        var deletedSlip = CreateSlipClassification(2, "Small Deleted Slip", 30.0m, 9.0m, 60);
        deletedSlip.IsDeleted = true;
        var mediumSlip = CreateSlipClassification(3, "Medium Slip", 35.0m, 12.0m, 75);

        await _repository.AddAsync(smallSlip);
        await _repository.AddAsync(deletedSlip);
        await _repository.AddAsync(mediumSlip);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(c => c.Name.Contains("Small"));

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Small Slip"));
        Assert.That(result.All(c => !c.IsDeleted), Is.True);
    }

    [Test]
    public async Task SearchByNameAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var smallSlip = CreateSlipClassification(1, "Small Slip", 25.0m, 8.0m, 50);
        var deletedSlip = CreateSlipClassification(2, "Small Deleted Slip", 30.0m, 9.0m, 60);
        deletedSlip.IsDeleted = true;
        var premiumSlip = CreateSlipClassification(3, "Small Premium Slip", 35.0m, 12.0m, 75);

        await _repository.AddAsync(smallSlip);
        await _repository.AddAsync(deletedSlip);
        await _repository.AddAsync(premiumSlip);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.SearchByNameAsync("Small");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Small Slip", "Small Premium Slip" }));
        Assert.That(result.All(c => !c.IsDeleted), Is.True);
    }

    [Test]
    public async Task GetByPriceRangeAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var budgetSlip = CreateSlipClassification(1, "Budget Slip", 25.0m, 8.0m, 50);
        var deletedSlip = CreateSlipClassification(2, "Deleted Budget Slip", 30.0m, 9.0m, 55);
        deletedSlip.IsDeleted = true;
        var standardSlip = CreateSlipClassification(3, "Standard Slip", 35.0m, 12.0m, 75);

        await _repository.AddAsync(budgetSlip);
        await _repository.AddAsync(deletedSlip);
        await _repository.AddAsync(standardSlip);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetByPriceRangeAsync(40, 80);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Budget Slip", "Standard Slip" }));
        Assert.That(result.All(c => !c.IsDeleted), Is.True);
    }

    [Test]
    public async Task GetCompatibleForVesselAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeSlip = CreateSlipClassification(1, "Active Compatible Slip", 25.0m, 8.0m, 50);
        var deletedSlip = CreateSlipClassification(2, "Deleted Compatible Slip", 30.0m, 9.0m, 60);
        deletedSlip.IsDeleted = true;

        await _repository.AddAsync(activeSlip);
        await _repository.AddAsync(deletedSlip);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetCompatibleForVesselAsync(20.0m, 6.0m);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Active Compatible Slip"));
        Assert.That(result.All(c => !c.IsDeleted), Is.True);
    }

    [Test]
    public async Task ExistsAsync_SoftDeletedEntity_ReturnsFalse()
    {
        // Arrange
        var classification = CreateSlipClassification(1, "Deleted Slip", 25.0m, 8.0m, 50);
        classification.IsDeleted = true;
        await _repository.AddAsync(classification);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion
} 