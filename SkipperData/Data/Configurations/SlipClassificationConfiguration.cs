using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class SlipClassificationConfiguration : BaseEntityConfiguration<SlipClassificationEntity>
{
    public override void Configure(EntityTypeBuilder<SlipClassificationEntity> builder)
    {
        builder.ToTable("slip_classifications");
        
        base.Configure(builder);
        
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
