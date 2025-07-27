using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class SlipReservationOrderConfiguration : IEntityTypeConfiguration<SlipReservationOrderEntity>
{
    public void Configure(EntityTypeBuilder<SlipReservationOrderEntity> builder)
    {
        // Slip-specific properties
        builder.Property(x => x.SlipId)
            .IsRequired();
            
        builder.Property(x => x.VesselId)
            .IsRequired();
            
        builder.Property(x => x.StartDate)
            .IsRequired();
            
        builder.Property(x => x.EndDate)
            .IsRequired();
            
        builder.Property(x => x.PriceRate)
            .IsRequired();
            
        builder.Property(x => x.PriceUnit)
            .HasConversion<string>()
            .IsRequired();
            
        builder.Property(x => x.RentalStatus)
            .HasConversion<string>()
            .IsRequired();

        // Slip-specific relationships
        builder.HasOne(x => x.SlipEntity)
            .WithMany()
            .HasForeignKey(x => x.SlipId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(x => x.VesselEntity)
            .WithMany()
            .HasForeignKey(x => x.VesselId)
            .OnDelete(DeleteBehavior.Restrict);

        // Slip-specific indexes
        builder.HasIndex(x => x.SlipId);
        builder.HasIndex(x => x.VesselId);
        builder.HasIndex(x => x.RentalStatus);
        builder.HasIndex(x => new { x.SlipId, x.StartDate, x.EndDate });
    }
}