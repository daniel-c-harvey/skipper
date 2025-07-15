using AuthBlocksModels.Entities.Identity;
using Data.Shared.Managers;

namespace AuthBlocksData.Services;

public interface IRoleService : IManager<ApplicationRole>
{
    Task<ApplicationRole?> FindByNameAsync(string roleName);
    Task<IEnumerable<ApplicationRole>> GetRootRolesAsync();
    Task<IEnumerable<ApplicationRole>> GetChildRolesAsync(long parentRoleId);
    Task<IEnumerable<ApplicationRole>> GetAncestorsAsync(long roleId);
}