using Microsoft.AspNetCore.Identity;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using AuthBlocksData.Data.Repositories;
using Data.Shared.Managers;
using Data.Shared.Data.Repositories;

namespace AuthBlocksData.Services;

public class UserService : ManagerBase<ApplicationUser, UserModel>, IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _userRepository;

    public UserService(UserManager<ApplicationUser> userManager, IUserRepository userRepository) 
        : base(userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    // Standard Identity operations - use UserManager
    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> FindByNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    // Custom operations with soft delete - use Repository
    public async Task<ApplicationUser?> GetActiveUserByIdAsync(long id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<ApplicationUser?> GetActiveUserByEmailAsync(string normalizedEmail)
    {
        return await _userRepository.GetByEmailAsync(normalizedEmail);
    }

    public async Task<IList<ApplicationUser>> GetActiveUsersInRoleAsync(string roleName)
    {
        return await _userRepository.GetUsersInRoleAsync(roleName);
    }

    public async Task SoftDeleteUserAsync(ApplicationUser user)
    {
        await _userRepository.DeleteAsync(user);
    }

    public async Task<ApplicationUser> UpdateUserWithReturnAsync(ApplicationUser user)
    {
        return await _userRepository.UpdateUserAsync(user);
    }

    // Role operations with soft delete
    public async Task<IList<string>> GetActiveUserRolesAsync(ApplicationUser user)
    {
        return await _userRepository.GetRolesAsync(user);
    }

    public async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
    {
        await _userRepository.AddToRoleAsync(user, roleName);
    }

    public async Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName)
    {
        await _userRepository.RemoveFromRoleAsync(user, roleName);
    }

    public async Task<bool> CheckPassword(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }
    
    public async Task UpdatePassword(ApplicationUser user, string password)
    {
        await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, password);
    }
} 