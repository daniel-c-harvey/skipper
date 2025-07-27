using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;
using SkipperModels;

namespace SkipperData.Data.Configurations;

public class OrderConfiguration : BaseEntityConfiguration<OrderEntity>
{
    public override void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        base.Configure(builder);
        
        // TPH Configuration - Single table for all order types
        builder.ToTable("orders");
        
        // Configure TPH discriminator
        builder.HasDiscriminator(x => x.OrderType)
            .HasValue<SlipReservationOrderEntity>(OrderType.SlipReservation);
            // Future order types will be added here:
            // .HasValue<ServiceOrderEntity>(OrderType.ServiceOrder)
            // .HasValue<PurchaseOrderEntity>(OrderType.PurchaseOrder)
            // .HasValue<StorageOrderEntity>(OrderType.StorageOrder);

        // Common order properties
        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.OrderDate)
            .IsRequired();

        builder.Property(x => x.OrderType)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(x => x.TotalAmount)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        // Customer relationship
        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Common indexes
        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => new { x.Status, x.OrderDate });
        builder.HasIndex(x => x.OrderType);
    }
}