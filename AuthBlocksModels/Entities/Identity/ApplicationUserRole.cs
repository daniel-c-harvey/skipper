using AuthBlocksModels.Models;
using Microsoft.AspNetCore.Identity;
using Models.Shared.Entities;

namespace AuthBlocksModels.Entities.Identity;

public class ApplicationUserRole : IdentityUserRole<long>, IEntity<ApplicationUserRole, UserRoleModel>
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
    public long RoleId { get; set; } 
    public virtual ApplicationRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public static UserRoleModel CreateModel(ApplicationUserRole entity)
    {
        return new UserRoleModel
        {
            Id = entity.Id,
            User = ApplicationUser.CreateModel(entity.User),
            Role = ApplicationRole.CreateModel(entity.Role),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }
}