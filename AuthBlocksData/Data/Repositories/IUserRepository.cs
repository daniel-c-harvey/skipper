using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Data.Shared.Data.Repositories;

namespace AuthBlocksData.Data.Repositories;

public interface IUserRepository : IRepository<ApplicationUser, UserModel>
{
    // Identity-specific methods
    Task<ApplicationUser?> GetByUsernameAsync(string normalizedUsername);
    Task<ApplicationUser?> GetByEmailAsync(string normalizedEmail);
} 