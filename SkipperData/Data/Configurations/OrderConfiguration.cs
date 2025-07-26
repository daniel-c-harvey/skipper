using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Shared.Entities;
using SkipperModels.Entities;
using SkipperModels;

namespace SkipperData.Data.Configurations
{
    public abstract class OrderConfiguration<TCustomerProfile, TOrderEntity> : BaseLinkageEntityConfiguration<TOrderEntity>
    where TCustomerProfile : CustomerProfileBaseEntity
    where TOrderEntity : OrderEntity<TCustomerProfile>
    {
        public override void Configure(EntityTypeBuilder<TOrderEntity> builder)
        {
            base.Configure(builder);

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

            builder.HasKey(x => new { x.OrderTypeId, x.OrderType });
            
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

            // Index for efficient querying by status and date
            builder.HasIndex(x => new { x.Status, x.OrderDate });
        }
    }
} 