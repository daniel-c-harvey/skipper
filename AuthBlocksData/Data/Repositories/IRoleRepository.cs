using AuthBlocksModels.Entities.Identity;
using Data.Shared.Data.Repositories;

namespace AuthBlocksData.Data.Repositories;

public interface IRoleRepository : IRepository<ApplicationRole>
{
    Task<ApplicationRole?> GetByNameAsync(string normalizedName);

    // Simple hierarchy methods
    Task<IEnumerable<ApplicationRole>> GetRootRolesAsync();
    Task<IEnumerable<ApplicationRole>> GetChildRolesAsync(long parentRoleId);
    Task<IEnumerable<ApplicationRole>> GetAncestorsAsync(long roleId);
    Task<IEnumerable<ApplicationRole>> GetAllAsync();
} 