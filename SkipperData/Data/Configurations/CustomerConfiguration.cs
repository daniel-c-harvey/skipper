using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;
using SkipperModels;

namespace SkipperData.Data.Configurations
{
    public abstract class CustomerConfiguration<TCustomer, TCustomerProfile> : BaseEntityConfiguration<TCustomer>
        where TCustomer : CustomerEntity<TCustomerProfile>
        where TCustomerProfile : CustomerProfileBaseEntity
    {
        public override void Configure(EntityTypeBuilder<TCustomer> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.AccountNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.CustomerProfileType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.CustomerProfileId)
                .IsRequired();

            // Index for efficient querying by account number
            builder.HasIndex(x => x.AccountNumber)
                .IsUnique();

            // Index for efficient querying by customer profile
            builder.HasIndex(x => new { x.CustomerProfileId, x.CustomerProfileType });
        }
    }
} 