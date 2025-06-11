using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class SlipConfiguration : BaseEntityConfiguration<Slip>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Slip> builder)
    {
        builder.ToTable("slips");
        
        builder.Property(e => e.SlipNumber)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(e => e.LocationCode)
            .HasMaxLength(20)
            .IsRequired();
            
        builder.Property(e => e.Status)
            .HasConversion<int>()
            .IsRequired();
        
        // Foreign key relationship with SlipClassification
        builder.HasOne(e => e.SlipClassification)
            .WithMany()
            .HasForeignKey(e => e.SlipClassificationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes for better query performance
        builder.HasIndex(e => e.SlipNumber)
            .IsUnique();
            
        // builder.HasIndex(e => e.LocationCode);
        
        builder.HasIndex(e => e.Status);
    }
} 