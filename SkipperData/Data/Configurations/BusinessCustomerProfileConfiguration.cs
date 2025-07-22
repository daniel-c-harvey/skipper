using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;
using Data.Shared.Data.Configurations;

namespace SkipperData.Data.Configurations
{
    public class BusinessCustomerProfileConfiguration : BaseEntityConfiguration<BusinessCustomerProfileEntity>
    {
        public override void Configure(EntityTypeBuilder<BusinessCustomerProfileEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("business_customer_profiles");

            builder.Property(x => x.BusinessName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.TaxId)
                .HasMaxLength(50);

            // Explicit junction entity relationship
            builder.HasMany(x => x.BusinessCustomerContacts)
                .WithOne(bc => bc.BusinessCustomerProfile)
                .HasForeignKey(bc => bc.BusinessCustomerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 