using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class VesselConfiguration : BaseEntityConfiguration<VesselEntity>
{
    public override void Configure(EntityTypeBuilder<VesselEntity> builder)
    {
        builder.ToTable("vessels");
        
        base.Configure(builder);
        
        builder.Property(e => e.RegistrationNumber)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(e => e.Length)
            .HasPrecision(19, 5)
            .IsRequired();
            
        builder.Property(e => e.Beam)
            .HasPrecision(19, 5)
            .IsRequired();
            
        builder.Property(e => e.VesselType)
            .HasConversion<string>()
            .IsRequired();
        
        // Indexes for better query performance
        builder.HasIndex(e => e.RegistrationNumber)
            .IsUnique();
            
        builder.HasIndex(e => e.Name);
        
        builder.HasIndex(e => e.VesselType);
    }
} 