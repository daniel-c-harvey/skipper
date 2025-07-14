using AuthBlocksModels.Entities.Identity;
using Models.Shared.Models;

namespace AuthBlocksModels.Models;

public class UserRoleModel : IModel<UserRoleModel, ApplicationUserRole>
{
    public long Id { get; set; }
    public UserModel User { get; set; }
    public RoleModel Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public static ApplicationUserRole CreateEntity(UserRoleModel dto)
    {
        return new ApplicationUserRole
        {
            Id = dto.Id,
            UserId = dto.User.Id,
            RoleId = dto.Role.Id,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
        };
    }
}