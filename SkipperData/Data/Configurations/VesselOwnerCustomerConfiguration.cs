using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class VesselOwnerCustomerConfiguration : CustomerConfiguration<VesselOwnerCustomerEntity, VesselOwnerProfileEntity>
{
    public override void Configure(EntityTypeBuilder<VesselOwnerCustomerEntity> builder)
    {
        builder.ToTable("vessel_owner_customers");
        
        base.Configure(builder);
        
        builder.Property(x => x.LicenseNumber)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(x=> x.LicenseExpiryDate)
            .IsRequired();
    }
}