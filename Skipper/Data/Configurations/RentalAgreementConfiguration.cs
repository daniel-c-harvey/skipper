using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skipper.Domain.Entities;
using Skipper.Domain;

namespace Skipper.Data.Configurations;

public class RentalAgreementConfiguration : BaseEntityConfiguration<RentalAgreement>
{
    protected override void ConfigureEntity(EntityTypeBuilder<RentalAgreement> builder)
    {
        builder.ToTable("rental_agreements");
        
        builder.Property(e => e.StartDate)
            .IsRequired();
            
        builder.Property(e => e.EndDate)
            .IsRequired();
            
        builder.Property(e => e.PriceRate)
            .IsRequired();
            
        builder.Property(e => e.PriceUnit)
            .HasConversion<string>()
            .IsRequired();
            
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .IsRequired();
        
        // Foreign key relationship with Slip
        builder.HasOne(e => e.Slip)
            .WithMany()
            .HasForeignKey(e => e.SlipId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Foreign key relationship with Vessel
        builder.HasOne(e => e.Vessel)
            .WithMany()
            .HasForeignKey(e => e.VesselId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes for better query performance
        builder.HasIndex(e => e.SlipId);
        
        builder.HasIndex(e => e.VesselId);
        
        // builder.HasIndex(e => e.StartDate);
        
        // builder.HasIndex(e => e.EndDate);
        
        builder.HasIndex(e => e.Status);
    }
} 