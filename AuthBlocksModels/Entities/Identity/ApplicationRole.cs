using Microsoft.AspNetCore.Identity;
using Models.Shared.Entities;
using AuthBlocksModels.Models;

namespace AuthBlocksModels.Entities.Identity;

public class ApplicationRole : IdentityRole<long>, IEntity<ApplicationRole, RoleModel>
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Simple hierarchy - just parent reference
    public long? ParentRoleId { get; set; }
    
    // Navigation properties
    public virtual ApplicationRole? ParentRole { get; set; }
    public virtual ICollection<ApplicationRole> ChildRoles { get; set; } = new List<ApplicationRole>();
    
    // Simple helper properties
    public bool IsRootRole => ParentRoleId == null;
    public bool HasChildren => ChildRoles.Any();
    
    public static RoleModel CreateModel(ApplicationRole entity)
    {
        return new RoleModel
        {
            Id = entity.Id,
            Name = entity.Name ?? string.Empty,
            NormalizedName = entity.NormalizedName ?? string.Empty,
            ConcurrencyStamp = entity.ConcurrencyStamp,
            ParentRoleId = entity.ParentRoleId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }
}