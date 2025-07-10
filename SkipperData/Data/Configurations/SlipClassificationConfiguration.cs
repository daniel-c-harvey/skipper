using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Data.Configurations;

public class SlipClassificationConfiguration : BaseEntityConfiguration<SlipClassificationEntity, SlipClassificationModel>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SlipClassificationEntity> builder)
    {
        builder.ToTable("slip_classifications");
        
        builder.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(e => e.MaxLength)
            .HasPrecision(19, 5)
            .IsRequired();
            
        builder.Property(e => e.MaxBeam)
            .HasPrecision(19, 5)
            .IsRequired();
            
        builder.Property(e => e.BasePrice)
            .IsRequired();
            
        builder.Property(e => e.Description);
        
        // Indexes for better query performance
        builder.HasIndex(e => e.Name);
        
        builder.HasIndex(e => e.BasePrice);
    }
}
