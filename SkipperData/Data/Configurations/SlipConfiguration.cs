using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class SlipConfiguration : BaseEntityConfiguration<SlipEntity, SlipModel>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SlipEntity> builder)
    {
        builder.ToTable("slips");
        
        builder.Property(e => e.SlipNumber)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(e => e.LocationCode)
            .HasMaxLength(20)
            .IsRequired();
            
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .IsRequired();
        
        // Foreign key relationship with SlipClassification
        builder.HasOne(e => e.SlipClassificationEntity)
            .WithMany(e => e.Slips)
            .HasForeignKey(e => e.SlipClassificationId);
        
        // Indexes for better query performance
        builder.HasIndex(e => e.SlipNumber)
            .IsUnique();
            
        // builder.HasIndex(e => e.LocationCode);
        
        builder.HasIndex(e => e.Status);
    }
} 