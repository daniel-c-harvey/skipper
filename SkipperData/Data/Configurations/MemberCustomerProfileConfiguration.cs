using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;
using Data.Shared.Data.Configurations;

namespace SkipperData.Data.Configurations
{
    public class MemberCustomerProfileConfiguration : BaseEntityConfiguration<MemberCustomerProfileEntity>
    {
        public override void Configure(EntityTypeBuilder<MemberCustomerProfileEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("MemberCustomerProfiles");

            builder.Property(x => x.ContactId)
                .IsRequired();

            builder.Property(x => x.MembershipLevel)
                .HasMaxLength(50);

            builder.HasOne(x => x.Contact)
                .WithMany()
                .HasForeignKey(x => x.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for membership queries
            builder.HasIndex(x => x.MembershipStartDate);
            builder.HasIndex(x => x.MembershipEndDate);
            builder.HasIndex(x => x.MembershipLevel);
        }
    }
} 