using AuthBlocksModels.Entities.Identity;

namespace AuthBlocksData.Data.Repositories;

public interface IRoleRepository
{
    Task<ApplicationRole?> GetByIdAsync(long id);
    Task<ApplicationRole?> GetByNameAsync(string normalizedName);
    Task<ApplicationRole> CreateAsync(ApplicationRole role);
    Task<ApplicationRole> UpdateAsync(ApplicationRole role);
    Task DeleteAsync(ApplicationRole role);
    
    // Simple hierarchy methods
    Task<IEnumerable<ApplicationRole>> GetRootRolesAsync();
    Task<IEnumerable<ApplicationRole>> GetChildRolesAsync(long parentRoleId);
    Task<IEnumerable<ApplicationRole>> GetAncestorsAsync(long roleId);
    Task<IEnumerable<ApplicationRole>> GetAllAsync();
} 