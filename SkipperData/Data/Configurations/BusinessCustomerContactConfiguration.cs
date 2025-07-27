using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class BusinessCustomerContactConfiguration : BaseLinkageEntityConfiguration<BusinessCustomerContactsEntity>
{
    public override void Configure(EntityTypeBuilder<BusinessCustomerContactsEntity> builder)
    {
        builder.ToTable("business_customer_contacts");
        
        base.Configure(builder);
        
        builder.HasKey(e => new { e.BusinessCustomerId, e.ContactId });
        
        builder.Property(e => e.IsPrimary)
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.Property(e => e.IsEmergency)
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.HasOne(e => e.BusinessCustomer)
            .WithMany(e => e.BusinessCustomerContacts)
            .HasForeignKey(e => e.BusinessCustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(e => e.Contact)
            .WithMany()
            .HasForeignKey(e => e.ContactId)
            .IsRequired();
    }
}