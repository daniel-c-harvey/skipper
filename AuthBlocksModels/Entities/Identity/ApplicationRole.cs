using Microsoft.AspNetCore.Identity;

namespace AuthBlocksModels.Entities.Identity;

public class ApplicationRole : IdentityRole<long>
{
    public bool Deleted { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    
    // Simple hierarchy - just parent reference
    public long? ParentRoleId { get; set; }
    
    // Navigation properties
    public virtual ApplicationRole? ParentRole { get; set; }
    public virtual ICollection<ApplicationRole> ChildRoles { get; set; } = new List<ApplicationRole>();
    
    // Simple helper properties
    public bool IsRootRole => ParentRoleId == null;
    public bool HasChildren => ChildRoles.Any();
}