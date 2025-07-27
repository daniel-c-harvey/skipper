using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class IndividualCustomerEntityConfiguration : IEntityTypeConfiguration<IndividualCustomerEntity>
{
    public void Configure(EntityTypeBuilder<IndividualCustomerEntity> builder)
    {
        // Individual specific properties
        builder.Property(x => x.DateOfBirth);

        builder.Property(x => x.PreferredContactMethod)
            .HasMaxLength(50);
    }
} 