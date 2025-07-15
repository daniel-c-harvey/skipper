using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Data.Shared.Managers;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public interface IUserService : IManager<ApplicationUser, UserModel>
{
    public Task<Result> Add(UserModel model, string password);
    
    // Standard Identity operations
    Task<UserModel?> FindByEmailAsync(string email);
    Task<UserModel?> FindByNameAsync(string userName);
    
    // Password operations
    Task<bool> CheckPassword(UserModel user, string password);
    Task UpdatePassword(UserModel user, string password);
} 