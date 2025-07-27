using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class BusinessCustomerConfiguration : IEntityTypeConfiguration<BusinessCustomerEntity>
{
    public void Configure(EntityTypeBuilder<BusinessCustomerEntity> builder)
    {
        // Business specific properties
        builder.Property(x => x.BusinessName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.TaxId)
            .HasMaxLength(50);

        // BusinessCustomerContacts relationship will be configured in BusinessCustomerContactsConfiguration
    }
} 