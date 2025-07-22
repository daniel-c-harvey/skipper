using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations
{
    public class AddressConfiguration : BaseEntityConfiguration<AddressEntity>
    {
        public override void Configure(EntityTypeBuilder<AddressEntity> builder)
        {
            builder.ToTable("addresses");

            base.Configure(builder);

            builder.Property(e => e.Address1)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Address2)
                .HasMaxLength(255);

            builder.Property(e => e.City)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.State)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.ZipCode)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Country)
                .HasMaxLength(255)
                .IsRequired();
        }
    }
}