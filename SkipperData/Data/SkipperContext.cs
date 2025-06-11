using Microsoft.EntityFrameworkCore;
using SkipperData.Data.Configurations;
using SkipperModels.Entities;

namespace SkipperData.Data;

public class SkipperContext : DbContext
{
    public SkipperContext(DbContextOptions<SkipperContext> options) : base(options) { }

    public DbSet<Slip> Slips { get; set; }
    public DbSet<SlipClassification> SlipClassifications { get; set; }
    public DbSet<Vessel> Vessels { get; set; }
    public DbSet<RentalAgreement> RentalAgreements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Global configs
        modelBuilder.HasDefaultSchema("skipper");

        // Entity configs
        modelBuilder.ApplyConfiguration(new SlipConfiguration());
        modelBuilder.ApplyConfiguration(new SlipClassificationConfiguration());
        modelBuilder.ApplyConfiguration(new VesselConfiguration());
        modelBuilder.ApplyConfiguration(new RentalAgreementConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}

public static class SkipperContextBuilder
{
    /// <summary>
    /// Creates a SkipperContext with the provided connection string for console applications
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Configured SkipperContext instance</returns>
    public static SkipperContext CreateContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SkipperContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new SkipperContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Creates a SkipperContext with custom DbContextOptions for console applications
    /// </summary>
    /// <param name="options">Pre-configured DbContextOptions</param>
    /// <returns>SkipperContext instance</returns>
    public static SkipperContext CreateContext(DbContextOptions<SkipperContext> options)
    {
        return new SkipperContext(options);
    }
}
