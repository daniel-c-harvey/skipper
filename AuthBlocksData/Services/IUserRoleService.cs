using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Data.Shared.Managers;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public interface IUserRoleService : IManager<ApplicationUserRole, UserRoleModel>
{
    Task AddUserToRoleAsync(ApplicationUser user, string roleName);
    Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName);
    Task<ResultContainer<IEnumerable<ApplicationRole>>> GetByUser(ApplicationUser user);
}