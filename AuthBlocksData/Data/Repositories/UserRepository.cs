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

    // Identity-specific methods (keep these)
    public async Task<ApplicationUser?> GetByUsernameAsync(string normalizedUsername)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUsername && !u.IsDeleted);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string normalizedEmail)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted);
    }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .Where(u => _context.UserRoles
                .Where(ur => !ur.Deleted)
                .Join(_context.Roles.Where(r => !r.Deleted && r.Name == roleName),
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => ur.UserId)
                .Contains(u.Id))
            .ToListAsync();
    }

    public async Task<ApplicationUser> CreateAsync(ApplicationUser user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(ApplicationUser user)
    {
        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == user.Id && !ur.Deleted)
            .Join(_context.Roles.Where(r => !r.Deleted),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r.Name!)
            .ToListAsync();
    }

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == user.Id && !ur.Deleted)
            .Join(_context.Roles.Where(r => !r.Deleted && r.Name == roleName),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => ur.UserId)
            .AnyAsync();
    }

    public async Task AddToRoleAsync(ApplicationUser user, string roleName)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName && !r.Deleted);
        if (role == null)
            throw new InvalidOperationException($"Role '{roleName}' not found.");

        var existingUserRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

        if (existingUserRole == null)
        {
            var userRole = new ApplicationUserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);
        }
        else if (existingUserRole.Deleted)
        {
            existingUserRole.Deleted = false;
            existingUserRole.Modified = DateTime.UtcNow;
            _context.UserRoles.Update(existingUserRole);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName && !r.Deleted);
        if (role == null)
            return;

        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id && !ur.Deleted);

        if (userRole != null)
        {
            userRole.Deleted = true;
            userRole.Modified = DateTime.UtcNow;
            _context.UserRoles.Update(userRole);
            await _context.SaveChangesAsync();
        }
    }
} 