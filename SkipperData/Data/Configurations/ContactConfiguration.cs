using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Shared.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations
{
    public class ContactConfiguration : BaseEntityConfiguration<ContactEntity>
    {
        public override void Configure(EntityTypeBuilder<ContactEntity> builder)
        {
            builder.ToTable("contacts");

            base.Configure(builder);

            builder.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Email)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(15);

            builder.HasOne(e => e.Address)
                .WithMany()
                .HasForeignKey(e => e.AddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}