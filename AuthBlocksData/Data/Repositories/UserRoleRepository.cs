using AuthBlocksModels.Entities.Identity;
using Data.Shared.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthBlocksData.Data.Repositories;

public class UserRoleRepository : Repository<AuthDbContext, ApplicationUserRole>, IUserRoleRepository
{
    public UserRoleRepository(AuthDbContext context, ILogger<Repository<AuthDbContext, ApplicationUserRole>> logger) : base(context, logger)
    {
    }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName)
    {
        return await _context.UserRoles
            .Join(_context.Users, ur => ur.UserId, u => u.Id, (ur, u) => new { UserRole = ur, User = u} )
            .Join(_context.Roles, uru => uru.UserRole.RoleId, r => r.Id, (uru, r) => new { UserRole = uru.UserRole, User = uru.User, Role = r } )
            .Where(x => !x.UserRole.IsDeleted && ! x.User.IsDeleted && !x.Role.IsDeleted && x.Role.Name == roleName)
            .Select(x => x.User)
            .ToListAsync();
    }
    
    public async Task<IList<ApplicationRole>> GetRolesAsync(ApplicationUser user)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == user.Id && !ur.IsDeleted)
            .Join(_context.Roles.Where(r => !r.IsDeleted),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r)
            .ToListAsync();
    }
    

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == user.Id && !ur.IsDeleted)
            .Join(_context.Roles.Where(r => !r.IsDeleted && r.Name == roleName),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => ur.UserId)
            .AnyAsync();
    }

    public async Task AddToRoleAsync(ApplicationUser user, string roleName)
    {
        
        
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName && !r.IsDeleted);
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);
        }
        else if (existingUserRole.IsDeleted)
        {
            existingUserRole.IsDeleted = false;
            existingUserRole.UpdatedAt = DateTime.UtcNow;
            _context.UserRoles.Update(existingUserRole);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName && !r.IsDeleted);
        if (role == null)
            return;

        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id && !ur.IsDeleted);

        if (userRole != null)
        {
            userRole.IsDeleted = true;
            userRole.UpdatedAt = DateTime.UtcNow;
            _context.UserRoles.Update(userRole);
            await _context.SaveChangesAsync();
        }
    }
}