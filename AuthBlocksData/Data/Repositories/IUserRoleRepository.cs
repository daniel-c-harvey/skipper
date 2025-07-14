using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Data.Shared.Data.Repositories;

namespace AuthBlocksData.Data.Repositories;

public interface IUserRoleRepository : IRepository<ApplicationUserRole, UserRoleModel>
{
    Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName);
    Task<IList<ApplicationRole>> GetRolesAsync(ApplicationUser user);
    Task<bool> IsInRoleAsync(ApplicationUser user, string roleName);
    Task AddToRoleAsync(ApplicationUser user, string roleName);
    Task RemoveFromRoleAsync(ApplicationUser user, string roleName);
}