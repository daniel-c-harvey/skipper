using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class VesselOwnerCustomerConfiguration : IEntityTypeConfiguration<VesselOwnerCustomerEntity>
{
    public void Configure(EntityTypeBuilder<VesselOwnerCustomerEntity> builder)
    {
        // Vessel owner specific properties
        builder.Property(x => x.LicenseNumber)
            .HasMaxLength(50);
        
        builder.Property(x => x.LicenseExpiryDate);

        // Contact relationship
        builder.Property(x => x.ContactId)
            .IsRequired();

        builder.HasOne(x => x.Contact)
            .WithMany()
            .HasForeignKey(x => x.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        // VesselOwnerVessels relationship will be configured in VesselOwnerVesselConfiguration
    }
}