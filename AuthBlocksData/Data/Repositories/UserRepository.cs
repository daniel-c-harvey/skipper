using Microsoft.EntityFrameworkCore;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace AuthBlocksData.Data.Repositories;

public class UserRepository : Repository<AuthDbContext, ApplicationUser, UserModel>, IUserRepository
{
    public UserRepository(AuthDbContext context, ILogger<UserRepository> logger) : base(context, logger)
    {
    }

    // Identity-specific methods
    public async Task<ApplicationUser?> GetByUsernameAsync(string normalizedUsername)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUsername && !u.IsDeleted);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string normalizedEmail)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted);
    }

    

    
} 