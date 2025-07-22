using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;
using Data.Shared.Data.Configurations;

namespace SkipperData.Data.Configurations
{
    public class VesselOwnerProfileConfiguration : BaseEntityConfiguration<VesselOwnerProfileEntity>
    {
        public override void Configure(EntityTypeBuilder<VesselOwnerProfileEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("vessel_owner_profiles");

            builder.Property(x => x.ContactId)
                .IsRequired();

            builder.HasOne(x => x.Contact)
                .WithMany()
                .HasForeignKey(x => x.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.VesselOwnerVessels)
                .WithOne(x => x.VesselOwnerProfile)
                .HasForeignKey(x => x.VesselOwnerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 