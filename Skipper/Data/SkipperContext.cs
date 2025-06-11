using System;
using Microsoft.EntityFrameworkCore;
using Skipper.Data.Configurations;
using Skipper.Domain.Entities;

namespace Skipper.Data;

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
