// using Data.Shared.Data.Configurations;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using SkipperModels.Entities;
//
// namespace SkipperData.Data.Configurations;
//
// public class ServiceTypeConfiguration : BaseEntityConfiguration<ServiceTypeEntity>
// {
//     public override void Configure(EntityTypeBuilder<ServiceTypeEntity> builder)
//     {
//         base.Configure(builder);
//         
//         builder.ToTable("service_types");
//         
//         // Service type properties
//         builder.Property(x => x.Name)
//             .IsRequired()
//             .HasMaxLength(100);
//             
//         builder.Property(x => x.Description)
//             .HasMaxLength(500);
//             
//         builder.Property(x => x.BasePrice)
//             .IsRequired();
//
//         // Indexes
//         builder.HasIndex(x => x.Name)
//             .IsUnique();
//     }
// } 