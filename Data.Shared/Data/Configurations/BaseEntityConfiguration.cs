using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Shared.Entities;
using Models.Shared.Models;

namespace Data.Shared.Data.Configurations;

public abstract class BaseEntityConfiguration<TEntity, TDto> : IEntityTypeConfiguration<TEntity> 
where TEntity : class, IEntity<TEntity, TDto>
where TDto : class, IModel<TDto, TEntity>
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