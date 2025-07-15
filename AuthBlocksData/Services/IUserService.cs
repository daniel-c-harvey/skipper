using AuthBlocksModels.Entities.Identity;
using Data.Shared.Managers;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public interface IUserService : IManager<ApplicationUser>
{
    public Task<Result> Add(ApplicationUser entity, string password);
    
    // Standard Identity operations
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByNameAsync(string userName);
    
    // Password operations
    Task<bool> CheckPassword(ApplicationUser user, string password);
    Task UpdatePassword(ApplicationUser user, string password);
} 