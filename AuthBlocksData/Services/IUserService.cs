using Microsoft.AspNetCore.Identity;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Data.Shared.Managers;

namespace AuthBlocksData.Services;

public interface IUserService : IManager<ApplicationUser, UserModel>
{
    // Standard Identity operations
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByNameAsync(string userName);
    
    // Custom operations with soft delete
    Task<ApplicationUser?> GetActiveUserByIdAsync(long id);
    Task<ApplicationUser?> GetActiveUserByEmailAsync(string normalizedEmail);
    Task<IList<ApplicationUser>> GetActiveUsersInRoleAsync(string roleName);
    Task SoftDeleteUserAsync(ApplicationUser user);
    Task<ApplicationUser> UpdateUserWithReturnAsync(ApplicationUser user);
    
    // Role operations with soft delete
    Task<IList<string>> GetActiveUserRolesAsync(ApplicationUser user);
    Task AddUserToRoleAsync(ApplicationUser user, string roleName);
    Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName);
    
    // Password operations
    Task<bool> CheckPassword(ApplicationUser user, string password);
    Task UpdatePassword(ApplicationUser user, string password);
} 