using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrderEntity>
{
    public void Configure(EntityTypeBuilder<ServiceOrderEntity> builder)
    {
        // Service-specific properties
        builder.Property(x => x.ServiceTypeId)
            .IsRequired();
            
        builder.Property(x => x.ScheduledDate)
            .IsRequired();
            
        builder.Property(x => x.ServiceDescription)
            .IsRequired()
            .HasMaxLength(1000);
            
        builder.Property(x => x.LaborHours)
            .IsRequired();
            
        builder.Property(x => x.PartsCost)
            .IsRequired();
            
        builder.Property(x => x.ServiceStatus)
            .HasConversion<string>()
            .IsRequired();

        // Service-specific relationships
        builder.HasOne(x => x.ServiceType)
            .WithMany()
            .HasForeignKey(x => x.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Service-specific indexes
        builder.HasIndex(x => x.ServiceTypeId);
        builder.HasIndex(x => x.ScheduledDate);
        builder.HasIndex(x => x.ServiceStatus);
    }
} 