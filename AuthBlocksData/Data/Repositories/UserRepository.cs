using Microsoft.EntityFrameworkCore;
using AuthBlocksModels.Entities.Identity;
using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace AuthBlocksData.Data.Repositories;

public class UserRepository : Repository<AuthDbContext, ApplicationUser>, IUserRepository
{
    public UserRepository(AuthDbContext context, ILogger<UserRepository> logger) : base(context, logger)
    {
    }

    protected override void UpdateModel(ApplicationUser target, ApplicationUser source)
    {
        base.UpdateModel(target, source);
        target.IsDeactivated = source.IsDeactivated;
        target.UserName = source.UserName;
        target.NormalizedUserName = source.NormalizedUserName;
        target.Email = source.Email;
        target.NormalizedEmail = source.NormalizedEmail;
        target.EmailConfirmed = source.EmailConfirmed;
        target.PhoneNumber = source.PhoneNumber;
        target.PhoneNumberConfirmed = source.PhoneNumberConfirmed;
        target.LockoutEnd = source.LockoutEnd;
        target.LockoutEnabled = source.LockoutEnabled;
        target.AccessFailedCount = source.AccessFailedCount;
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