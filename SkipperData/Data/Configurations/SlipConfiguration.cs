using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class SlipConfiguration : BaseEntityConfiguration<SlipEntity>
{
    public override void Configure(EntityTypeBuilder<SlipEntity> builder)
    {
        builder.ToTable("slips");
        
        base.Configure(builder);
        
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
            .WithMany()
            .HasForeignKey(e => e.SlipClassificationId);
        
        // Indexes for better query performance
        builder.HasIndex(e => e.SlipNumber)
            .IsUnique();
            
        // builder.HasIndex(e => e.LocationCode);
        
        builder.HasIndex(e => e.Status);
    }
} 