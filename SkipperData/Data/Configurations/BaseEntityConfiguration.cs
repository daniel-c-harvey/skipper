using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
            
        builder.Property(e => e.UpdatedAt)
            .IsRequired();
            
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
            
        // Add index on IsDeleted for performance when filtering soft-deleted records
        builder.HasIndex(e => e.IsDeleted);
        
        // Call derived configuration
        ConfigureEntity(builder);
    }
    
    protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
} 