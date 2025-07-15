using AuthBlocksModels.Entities.Identity;
using Data.Shared.Managers;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public interface IUserRoleService : IManager<ApplicationUserRole>
{
    Task AddUserToRoleAsync(ApplicationUser user, string roleName);
    Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName);
    Task<ResultContainer<IEnumerable<ApplicationRole>>> GetByUser(ApplicationUser user);
}