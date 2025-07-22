using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations
{
    public class VesselOwnerVesselConfiguration : BaseLinkageEntityConfiguration<VesselOwnerVesselEntity>
    {
        public override void Configure(EntityTypeBuilder<VesselOwnerVesselEntity> builder)
        {
            builder.ToTable("vessel_owner_vessels");

            base.Configure(builder);

            builder.HasOne(e => e.VesselOwnerProfile)
                .WithMany(e => e.VesselOwnerVessels)
                .HasForeignKey(e => e.VesselOwnerProfileId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(e => e.Vessel)
                .WithOne()
                .HasForeignKey<VesselOwnerVesselEntity>(e => e.VesselId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}