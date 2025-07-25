using Microsoft.EntityFrameworkCore;
using SkipperData.Data.Configurations;
using SkipperModels.Entities;

namespace SkipperData.Data;

public class SkipperContext : DbContext
{
    public SkipperContext(DbContextOptions<SkipperContext> options) : base(options) { }

    public DbSet<SlipEntity> Slips { get; set; }
    public DbSet<SlipClassificationEntity> SlipClassifications { get; set; }
    public DbSet<VesselEntity> Vessels { get; set; }
    public DbSet<RentalAgreementEntity> RentalAgreements { get; set; }
    
    // Customer entities
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<VesselOwnerProfileEntity> VesselOwnerProfiles { get; set; }
    public DbSet<IndividualCustomerProfileEntity> IndividualCustomerProfiles { get; set; }
    public DbSet<BusinessCustomerProfileEntity> BusinessCustomerProfiles { get; set; }
    public DbSet<MemberCustomerProfileEntity> MemberCustomerProfiles { get; set; }
    
    // Supporting entities  
    public DbSet<ContactEntity> Contacts { get; set; }
    public DbSet<AddressEntity> Addresses { get; set; }
    public DbSet<VesselOwnerVesselEntity> VesselOwnerVessels { get; set; }
    public DbSet<BusinessCustomerContactsEntity> BusinessCustomerContacts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Global configs
        modelBuilder.HasDefaultSchema("skipper");

        // Entity configs
        modelBuilder.ApplyConfiguration(new SlipConfiguration());
        modelBuilder.ApplyConfiguration(new SlipClassificationConfiguration());
        modelBuilder.ApplyConfiguration(new VesselConfiguration());
        modelBuilder.ApplyConfiguration(new RentalAgreementConfiguration());
        
        // Customer configurations
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new VesselOwnerProfileConfiguration());
        modelBuilder.ApplyConfiguration(new IndividualCustomerProfileConfiguration());
        modelBuilder.ApplyConfiguration(new BusinessCustomerProfileConfiguration());
        modelBuilder.ApplyConfiguration(new MemberCustomerProfileConfiguration());
        
        // Supporting configurations
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new ContactConfiguration());
        modelBuilder.ApplyConfiguration(new VesselOwnerVesselConfiguration());
        modelBuilder.ApplyConfiguration(new BusinessCustomerContactConfiguration());

        // Set global collation for all string properties
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties().Where(p => p.ClrType == typeof(string)))
            {
                property.SetAnnotation("Relational:Collation", "en-US-x-icu");
            }
        }
        
        
        var dateTimeConverter = new UtcDateTimeConverter();
            
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
            }
        }
        
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
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.SetPostgresVersion(16, 9);
        });
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
