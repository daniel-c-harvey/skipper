using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;
using SkipperModels;

namespace SkipperData.Data.Configurations
{
    public class OrderConfiguration : BaseEntityConfiguration<OrderEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("orders");

            builder.Property(x => x.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.OrderDate)
                .IsRequired();

            builder.Property(x => x.OrderType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.OrderTypeId)
                .IsRequired();

            builder.Property(x => x.TotalAmount)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>();

            // Foreign key relationship with Customer
            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index for efficient querying by order number
            builder.HasIndex(x => x.OrderNumber)
                .IsUnique();

            // Index for efficient querying by customer
            builder.HasIndex(x => x.CustomerId);

            // Index for efficient querying by order type
            builder.HasIndex(x => new { x.OrderTypeId, x.OrderType });

            // Index for efficient querying by status and date
            builder.HasIndex(x => new { x.Status, x.OrderDate });
        }
    }
} 