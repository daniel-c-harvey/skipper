using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;
using SkipperModels;

namespace SkipperData.Data.Configurations;

public class OrderConfiguration : BaseEntityConfiguration<OrderEntity<CustomerEntity>>
{
    public override void Configure(EntityTypeBuilder<OrderEntity<CustomerEntity>> builder)
    {
        base.Configure(builder);
        
        // TPH Configuration - Single table for all order types
        builder.ToTable("orders");
        
        // Configure TPH discriminator
        builder.HasDiscriminator(x => x.OrderType)
            .HasValue<SlipReservationOrderEntity>(OrderType.SlipReservation)
            .HasValue<ServiceOrderEntity>(OrderType.ServiceOrder)
            .HasValue<PurchaseOrderEntity>(OrderType.PurchaseOrder);
            // Future order types will be added here:
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

        // Customer relationship - works with the base CustomerEntity
        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();
        builder.HasIndex(x => x.OrderType);
        builder.HasIndex(x => new { x.Status, x.OrderDate });
    }
}