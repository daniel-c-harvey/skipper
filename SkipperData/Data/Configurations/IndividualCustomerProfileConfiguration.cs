using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;
using Data.Shared.Data.Configurations;

namespace SkipperData.Data.Configurations
{
    public class IndividualCustomerProfileConfiguration : BaseEntityConfiguration<IndividualCustomerProfileEntity>
    {
        public override void Configure(EntityTypeBuilder<IndividualCustomerProfileEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("individual_customer_profiles");

            builder.Property(x => x.ContactId)
                .IsRequired();

            builder.HasOne(x => x.Contact)
                .WithMany()
                .HasForeignKey(x => x.ContactId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 