using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Shared.Entities;

namespace Data.Shared.Data.Configurations;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> 
where TEntity : class, IEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
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
    
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
} 