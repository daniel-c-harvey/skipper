using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public class RentalAgreementRepositoryTests : RepositoryTestBase<RentalAgreement>
{
    private RentalAgreementRepository _repository;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _repository = new RentalAgreementRepository(Context);
    }

    #region Helper Methods

    private RentalAgreement CreateRentalAgreement(long id, long slipId, long vesselId, DateTime startDate, DateTime endDate, RentalStatus status = RentalStatus.Active, int priceRate = 100)
    {
        return new RentalAgreement
        {
            Id = id,
            SlipId = slipId,
            VesselId = vesselId,
            StartDate = startDate,
            EndDate = endDate,
            Status = status,
            PriceRate = priceRate,
            PriceUnit = PriceUnit.PerDay,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    private async Task SetupRentalsAsync(params RentalAgreement[] rentals)
    {
        foreach (var rental in rentals)
        {
            await _repository.AddAsync(rental);
        }
        await _repository.SaveChangesAsync();
    }

    #endregion

    #region GetByStatusAsync Tests

    [Test]
    public async Task GetByStatusAsync_ActiveStatus_ReturnsOnlyActiveRentals()
    {
        // Arrange
        var rentals = new[]
        {
            CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active),
            CreateRentalAgreement(2, 2, 2, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active),
            CreateRentalAgreement(3, 3, 3, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Pending),
            CreateRentalAgreement(4, 4, 4, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Expired)
        };

        await SetupRentalsAsync(rentals);

        // Act
        var result = await _repository.GetByStatusAsync(RentalStatus.Active);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(r => r.Status == RentalStatus.Active), Is.True);
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 2L }));
    }

    [TestCase(RentalStatus.Quoted)]
    [TestCase(RentalStatus.Pending)]
    [TestCase(RentalStatus.Active)]
    [TestCase(RentalStatus.Expired)]
    [TestCase(RentalStatus.Cancelled)]
    public async Task GetByStatusAsync_AllStatuses_ReturnsCorrectRentals(RentalStatus targetStatus)
    {
        // Arrange
        var rentals = Enum.GetValues<RentalStatus>()
            .Select((status, index) => CreateRentalAgreement(index + 1, index + 1, index + 1, DateTime.Today, DateTime.Today.AddDays(7), status))
            .ToArray();

        await SetupRentalsAsync(rentals);

        // Act
        var result = await _repository.GetByStatusAsync(targetStatus);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Status, Is.EqualTo(targetStatus));
    }

    [Test]
    public async Task GetByStatusAsync_NoMatchingStatus_ReturnsEmpty()
    {
        // Arrange
        var rentals = new[]
        {
            CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active),
            CreateRentalAgreement(2, 2, 2, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Pending)
        };

        await SetupRentalsAsync(rentals);

        // Act
        var result = await _repository.GetByStatusAsync(RentalStatus.Cancelled);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    #endregion

    #region GetActiveAsync Tests

    [Test]
    public async Task GetActiveAsync_ReturnsOnlyActiveRentals()
    {
        // Arrange
        var rentals = new[]
        {
            CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active),
            CreateRentalAgreement(2, 2, 2, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active),
            CreateRentalAgreement(3, 3, 3, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Pending),
            CreateRentalAgreement(4, 4, 4, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Expired),
            CreateRentalAgreement(5, 5, 5, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active)
        };

        await SetupRentalsAsync(rentals);

        // Act
        var result = await _repository.GetActiveAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.All(r => r.Status == RentalStatus.Active), Is.True);
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 2L, 5L }));
    }

    [Test]
    public async Task GetActiveAsync_NoActiveRentals_ReturnsEmpty()
    {
        // Arrange
        var rentals = new[]
        {
            CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Pending),
            CreateRentalAgreement(2, 2, 2, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Expired),
            CreateRentalAgreement(3, 3, 3, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Cancelled)
        };

        await SetupRentalsAsync(rentals);

        // Act
        var result = await _repository.GetActiveAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    #endregion

    #region GetBySlipIdAsync Tests

    [Test]
    public async Task GetBySlipIdAsync_ValidSlipId_ReturnsRentalsForSlip()
    {
        // Arrange
        var rentals = new[]
        {
            CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7)),
            CreateRentalAgreement(2, 1, 2, DateTime.Today.AddDays(10), DateTime.Today.AddDays(17)),
            CreateRentalAgreement(3, 2, 3, DateTime.Today, DateTime.Today.AddDays(7)),
            CreateRentalAgreement(4, 1, 4, DateTime.Today.AddDays(20), DateTime.Today.AddDays(27))
        };

        await SetupRentalsAsync(rentals);

        // Act
        var result = await _repository.GetBySlipIdAsync(1);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.All(r => r.SlipId == 1), Is.True);
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 2L, 4L }));
    }

    [Test]
    public async Task GetBySlipIdAsync_NonExistentSlipId_ReturnsEmpty()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        await SetupRentalsAsync(rental);

        // Act
        var result = await _repository.GetBySlipIdAsync(999);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    #endregion

    #region GetByVesselIdAsync Tests

    [Test]
    public async Task GetByVesselIdAsync_ValidVesselId_ReturnsRentalsForVessel()
    {
        // Arrange
        var rentals = new[]
        {
            CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7)),
            CreateRentalAgreement(2, 2, 1, DateTime.Today.AddDays(10), DateTime.Today.AddDays(17)),
            CreateRentalAgreement(3, 3, 2, DateTime.Today, DateTime.Today.AddDays(7)),
            CreateRentalAgreement(4, 4, 1, DateTime.Today.AddDays(20), DateTime.Today.AddDays(27))
        };

        await SetupRentalsAsync(rentals);

        // Act
        var result = await _repository.GetByVesselIdAsync(1);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.All(r => r.VesselId == 1), Is.True);
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 2L, 4L }));
    }

    [Test]
    public async Task GetByVesselIdAsync_NonExistentVesselId_ReturnsEmpty()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        await SetupRentalsAsync(rental);

        // Act
        var result = await _repository.GetByVesselIdAsync(999);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    #endregion

    #region HasConflictAsync Tests (Complex Logic)

    [Test]
    public async Task HasConflictAsync_OverlappingDates_ReturnsTrue()
    {
        // Arrange
        var existingRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);
        await SetupRentalsAsync(existingRental);

        // Act - New rental overlaps in the middle
        var result = await _repository.HasConflictAsync(1, DateTime.Today.AddDays(3), DateTime.Today.AddDays(10));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasConflictAsync_StartDateOverlaps_ReturnsTrue()
    {
        // Arrange
        var existingRental = CreateRentalAgreement(1, 1, 1, DateTime.Today.AddDays(5), DateTime.Today.AddDays(12), RentalStatus.Active);
        await SetupRentalsAsync(existingRental);

        // Act - New rental starts before existing and ends during existing
        var result = await _repository.HasConflictAsync(1, DateTime.Today.AddDays(2), DateTime.Today.AddDays(8));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasConflictAsync_EndDateOverlaps_ReturnsTrue()
    {
        // Arrange
        var existingRental = CreateRentalAgreement(1, 1, 1, DateTime.Today.AddDays(2), DateTime.Today.AddDays(9), RentalStatus.Active);
        await SetupRentalsAsync(existingRental);

        // Act - New rental starts during existing and ends after existing
        var result = await _repository.HasConflictAsync(1, DateTime.Today.AddDays(5), DateTime.Today.AddDays(12));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasConflictAsync_CompletelyEnclosed_ReturnsTrue()
    {
        // Arrange
        var existingRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(14), RentalStatus.Active);
        await SetupRentalsAsync(existingRental);

        // Act - New rental is completely within existing rental
        var result = await _repository.HasConflictAsync(1, DateTime.Today.AddDays(3), DateTime.Today.AddDays(10));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasConflictAsync_CompletelyEncloses_ReturnsTrue()
    {
        // Arrange
        var existingRental = CreateRentalAgreement(1, 1, 1, DateTime.Today.AddDays(5), DateTime.Today.AddDays(12), RentalStatus.Active);
        await SetupRentalsAsync(existingRental);

        // Act - New rental completely encloses existing rental
        var result = await _repository.HasConflictAsync(1, DateTime.Today, DateTime.Today.AddDays(20));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasConflictAsync_ExactSameDates_ReturnsTrue()
    {
        // Arrange
        var existingRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);
        await SetupRentalsAsync(existingRental);

        // Act - New rental has exact same dates
        var result = await _repository.HasConflictAsync(1, DateTime.Today, DateTime.Today.AddDays(7));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasConflictAsync_AdjacentDatesStartAfterEnd_ReturnsFalse()
    {
        // Arrange
        var existingRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);
        await SetupRentalsAsync(existingRental);

        // Act - New rental starts exactly when existing ends
        var result = await _repository.HasConflictAsync(1, DateTime.Today.AddDays(7), DateTime.Today.AddDays(14));

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasConflictAsync_AdjacentDatesEndBeforeStart_ReturnsFalse()
    {
        // Arrange
        var existingRental = CreateRentalAgreement(1, 1, 1, DateTime.Today.AddDays(7), DateTime.Today.AddDays(14), RentalStatus.Active);
        await SetupRentalsAsync(existingRental);

        // Act - New rental ends exactly when existing starts
        var result = await _repository.HasConflictAsync(1, DateTime.Today, DateTime.Today.AddDays(7));

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasConflictAsync_DifferentSlip_ReturnsFalse()
    {
        // Arrange
        var existingRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);
        await SetupRentalsAsync(existingRental);

        // Act - Same dates but different slip
        var result = await _repository.HasConflictAsync(2, DateTime.Today, DateTime.Today.AddDays(7));

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasConflictAsync_NonActiveStatus_ReturnsFalse()
    {
        // Arrange
        var existingRentals = new[]
        {
            CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Pending),
            CreateRentalAgreement(2, 1, 2, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Expired),
            CreateRentalAgreement(3, 1, 3, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Cancelled)
        };

        await SetupRentalsAsync(existingRentals);

        // Act - Overlapping dates but no active rentals
        var result = await _repository.HasConflictAsync(1, DateTime.Today, DateTime.Today.AddDays(7));

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasConflictAsync_MultipleActiveRentals_OneConflicts_ReturnsTrue()
    {
        // Arrange
        var existingRentals = new[]
        {
            CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(3), RentalStatus.Active),
            CreateRentalAgreement(2, 1, 2, DateTime.Today.AddDays(10), DateTime.Today.AddDays(17), RentalStatus.Active),
            CreateRentalAgreement(3, 1, 3, DateTime.Today.AddDays(20), DateTime.Today.AddDays(27), RentalStatus.Active)
        };

        await SetupRentalsAsync(existingRentals);

        // Act - New rental conflicts with the second one
        var result = await _repository.HasConflictAsync(1, DateTime.Today.AddDays(8), DateTime.Today.AddDays(15));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasConflictAsync_NoExistingRentals_ReturnsFalse()
    {
        // Arrange - No existing rentals

        // Act
        var result = await _repository.HasConflictAsync(1, DateTime.Today, DateTime.Today.AddDays(7));

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region Base Repository CRUD Tests

    [Test]
    public async Task GetByIdAsync_ValidId_ReturnsRentalAgreement()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        await SetupRentalsAsync(rental);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.SlipId, Is.EqualTo(1));
        Assert.That(result.VesselId, Is.EqualTo(1));
    }

    [Test]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        await SetupRentalsAsync(rental);

        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllRentalAgreements()
    {
        // Arrange
        var rentals = new[]
        {
            CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7)),
            CreateRentalAgreement(2, 2, 2, DateTime.Today, DateTime.Today.AddDays(7)),
            CreateRentalAgreement(3, 3, 3, DateTime.Today, DateTime.Today.AddDays(7))
        };

        await SetupRentalsAsync(rentals);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 2L, 3L }));
    }

    [Test]
    public async Task AddAsync_ValidRentalAgreement_AddsRentalAgreement()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));

        // Act
        var result = await _repository.AddAsync(rental);
        await _repository.SaveChangesAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.SlipId, Is.EqualTo(1));

        var savedRental = await _repository.GetByIdAsync(1);
        Assert.That(savedRental, Is.Not.Null);
        Assert.That(savedRental.SlipId, Is.EqualTo(1));
    }

    [Test]
    public async Task UpdateAsync_ValidRentalAgreement_UpdatesRentalAgreement()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        await SetupRentalsAsync(rental);

        // Act
        rental.Status = RentalStatus.Expired;
        rental.PriceRate = 150;
        rental.UpdatedAt = DateTime.UtcNow.AddHours(1);
        await _repository.UpdateAsync(rental);
        await _repository.SaveChangesAsync();

        // Assert
        var updatedRental = await _repository.GetByIdAsync(1);
        Assert.That(updatedRental, Is.Not.Null);
        Assert.That(updatedRental.Status, Is.EqualTo(RentalStatus.Expired));
        Assert.That(updatedRental.PriceRate, Is.EqualTo(150));
    }

    [Test]
    public async Task DeleteAsync_ValidId_SoftDeletesRentalAgreement()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        await SetupRentalsAsync(rental);

        // Act
        await _repository.DeleteAsync(1);
        await _repository.SaveChangesAsync();

        // Assert - Repository methods should not return the soft deleted entity
        var deletedRental = await _repository.GetByIdAsync(1);
        Assert.That(deletedRental, Is.Null);

        // Verify it's not in GetAll either
        var allRentals = await _repository.GetAllAsync();
        Assert.That(allRentals.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task ExistsAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        await SetupRentalsAsync(rental);

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsAsync_NonExistentId_ReturnsFalse()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        await SetupRentalsAsync(rental);

        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task FindAsync_WithPredicate_ReturnsMatchingRentalAgreements()
    {
        // Arrange
        var rentals = new[]
        {
            CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active, 100),
            CreateRentalAgreement(2, 2, 2, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active, 150),
            CreateRentalAgreement(3, 3, 3, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Pending, 200)
        };

        await SetupRentalsAsync(rentals);

        // Act
        var result = await _repository.FindAsync(r => r.PriceRate >= 150);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(r => r.PriceRate >= 150), Is.True);
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 2L, 3L }));
    }

    #endregion

    #region Soft Delete Tests

    [Test]
    public async Task GetByIdAsync_SoftDeletedEntity_ReturnsNull()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        rental.IsDeleted = true;
        await _repository.AddAsync(rental);
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
        var activeRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        var deletedRental = CreateRentalAgreement(2, 2, 2, DateTime.Today, DateTime.Today.AddDays(7));
        deletedRental.IsDeleted = true;

        await _repository.AddAsync(activeRental);
        await _repository.AddAsync(deletedRental);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Id, Is.EqualTo(1));
        Assert.That(result.All(r => !r.IsDeleted), Is.True);
    }

    [Test]
    public async Task FindAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active, 100);
        var deletedRental = CreateRentalAgreement(2, 2, 2, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active, 100);
        deletedRental.IsDeleted = true;
        var pendingRental = CreateRentalAgreement(3, 3, 3, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Pending, 100);

        await _repository.AddAsync(activeRental);
        await _repository.AddAsync(deletedRental);
        await _repository.AddAsync(pendingRental);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(r => r.PriceRate == 100);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 3L }));
        Assert.That(result.All(r => !r.IsDeleted), Is.True);
    }

    [Test]
    public async Task GetByStatusAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);
        var deletedRental = CreateRentalAgreement(2, 2, 2, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);
        deletedRental.IsDeleted = true;
        var anotherActiveRental = CreateRentalAgreement(3, 3, 3, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);

        await _repository.AddAsync(activeRental);
        await _repository.AddAsync(deletedRental);
        await _repository.AddAsync(anotherActiveRental);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetByStatusAsync(RentalStatus.Active);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(new[] { 1L, 3L }));
        Assert.That(result.All(r => !r.IsDeleted), Is.True);
    }

    [Test]
    public async Task GetActiveAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);
        var deletedRental = CreateRentalAgreement(2, 2, 2, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);
        deletedRental.IsDeleted = true;

        await _repository.AddAsync(activeRental);
        await _repository.AddAsync(deletedRental);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Id, Is.EqualTo(1));
        Assert.That(result.All(r => !r.IsDeleted), Is.True);
    }

    [Test]
    public async Task GetBySlipIdAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        var deletedRental = CreateRentalAgreement(2, 1, 2, DateTime.Today, DateTime.Today.AddDays(7));
        deletedRental.IsDeleted = true;

        await _repository.AddAsync(activeRental);
        await _repository.AddAsync(deletedRental);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySlipIdAsync(1);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Id, Is.EqualTo(1));
        Assert.That(result.All(r => !r.IsDeleted), Is.True);
    }

    [Test]
    public async Task GetByVesselIdAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        var deletedRental = CreateRentalAgreement(2, 2, 1, DateTime.Today, DateTime.Today.AddDays(7));
        deletedRental.IsDeleted = true;

        await _repository.AddAsync(activeRental);
        await _repository.AddAsync(deletedRental);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetByVesselIdAsync(1);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Single().Id, Is.EqualTo(1));
        Assert.That(result.All(r => !r.IsDeleted), Is.True);
    }

    [Test]
    public async Task HasConflictAsync_FiltersSoftDeletedEntities()
    {
        // Arrange
        var activeRental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);
        var deletedRental = CreateRentalAgreement(2, 1, 2, DateTime.Today, DateTime.Today.AddDays(7), RentalStatus.Active);
        deletedRental.IsDeleted = true;

        await _repository.AddAsync(activeRental);
        await _repository.AddAsync(deletedRental);
        await _repository.SaveChangesAsync();

        // Act - Should conflict with active rental but not deleted one
        var result = await _repository.HasConflictAsync(1, DateTime.Today, DateTime.Today.AddDays(7));

        // Assert
        Assert.That(result, Is.True); // Should still conflict because of the active rental
    }

    [Test]
    public async Task ExistsAsync_SoftDeletedEntity_ReturnsFalse()
    {
        // Arrange
        var rental = CreateRentalAgreement(1, 1, 1, DateTime.Today, DateTime.Today.AddDays(7));
        rental.IsDeleted = true;
        await _repository.AddAsync(rental);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion
} 