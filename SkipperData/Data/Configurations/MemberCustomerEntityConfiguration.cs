using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class MemberCustomerEntityConfiguration : IEntityTypeConfiguration<MemberCustomerEntity>
{
    public void Configure(EntityTypeBuilder<MemberCustomerEntity> builder)
    {
        // Member specific properties
        builder.Property(x => x.MembershipNumber)
            .HasMaxLength(50);

        builder.Property(x => x.MemberSince);

        builder.Property(x => x.MembershipLevel)
            .HasMaxLength(50);
    }
} 