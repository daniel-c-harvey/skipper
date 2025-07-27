using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;
using SkipperModels;

namespace SkipperData.Data.Configurations
{
    public class CustomerConfiguration : BaseEntityConfiguration<CustomerEntity>
    {
        public override void Configure(EntityTypeBuilder<CustomerEntity> builder)
        {
            base.Configure(builder);
            
            // TPH Configuration - Single table for all customer types
            builder.ToTable("customers");
            
            // Configure TPH discriminator
            builder.HasDiscriminator(x => x.CustomerProfileType)
                .HasValue<VesselOwnerCustomerEntity>(CustomerProfileType.VesselOwnerProfile)
                .HasValue<BusinessCustomerEntity>(CustomerProfileType.BusinessCustomerProfile)
                .HasValue<IndividualCustomerEntity>(CustomerProfileType.IndividualCustomerProfile)
                .HasValue<MemberCustomerEntity>(CustomerProfileType.MemberCustomerProfile);

            // Common customer properties
            builder.Property(x => x.AccountNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Indexes for efficient querying
            builder.HasIndex(x => x.AccountNumber)
                .IsUnique();
        }
    }
} 