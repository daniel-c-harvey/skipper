// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using SkipperModels.Entities;
//
// namespace SkipperData.Data.Configurations;
//
// public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrderEntity>
// {
//     public void Configure(EntityTypeBuilder<PurchaseOrderEntity> builder)
//     {
//         // Purchase-specific properties
//         builder.Property(x => x.PurchaseOrderNumber)
//             .IsRequired()
//             .HasMaxLength(50);
//             
//         builder.Property(x => x.ExpectedDeliveryDate)
//             .IsRequired();
//             
//         builder.Property(x => x.ShippingAddress)
//             .IsRequired()
//             .HasMaxLength(500);
//             
//         builder.Property(x => x.BillingAddress)
//             .IsRequired()
//             .HasMaxLength(500);
//             
//         builder.Property(x => x.PurchaseStatus)
//             .HasConversion<string>()
//             .IsRequired();
//             
//         builder.Property(x => x.TermsAndConditions)
//             .HasMaxLength(2000);
//
//         // Purchase-specific indexes
//         builder.HasIndex(x => x.PurchaseOrderNumber)
//             .IsUnique();
//         builder.HasIndex(x => x.ExpectedDeliveryDate);
//         builder.HasIndex(x => x.PurchaseStatus);
//     }
// } 