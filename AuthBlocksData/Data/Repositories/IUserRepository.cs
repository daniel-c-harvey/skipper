using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Data.Shared.Data.Repositories;

namespace AuthBlocksData.Data.Repositories;

public interface IUserRepository : IRepository<ApplicationUser, UserModel>
{
    // Identity-specific methods
    Task<ApplicationUser?> GetByUsernameAsync(string normalizedUsername);
    Task<ApplicationUser?> GetByEmailAsync(string normalizedEmail);
    Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName);
    Task<ApplicationUser> CreateAsync(ApplicationUser user);
    Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
    Task DeleteAsync(ApplicationUser user);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<bool> IsInRoleAsync(ApplicationUser user, string roleName);
    Task AddToRoleAsync(ApplicationUser user, string roleName);
    Task RemoveFromRoleAsync(ApplicationUser user, string roleName);
} 