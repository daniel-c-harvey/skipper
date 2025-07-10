using Microsoft.EntityFrameworkCore;
using AuthBlocksModels.Entities.Identity;

namespace AuthBlocksData.Data.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AuthDbContext _context;

    public RoleRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationRole?> GetByIdAsync(long id)
    {
        return await _context.Roles
            .Include(r => r.ParentRole)
            .Include(r => r.ChildRoles)
            .FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
    }

    public async Task<ApplicationRole?> GetByNameAsync(string normalizedName)
    {
        return await _context.Roles
            .Include(r => r.ParentRole)
            .Include(r => r.ChildRoles)
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedName && !r.Deleted);
    }

    public async Task<ApplicationRole> CreateAsync(ApplicationRole role)
    {
        role.Created = DateTime.UtcNow;
        role.Modified = DateTime.UtcNow;
        
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<ApplicationRole> UpdateAsync(ApplicationRole role)
    {
        role.Modified = DateTime.UtcNow;
        
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task DeleteAsync(ApplicationRole role)
    {
        role.Deleted = true;
        role.Modified = DateTime.UtcNow;
        
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }
    
    // Simple hierarchy methods
    public async Task<IEnumerable<ApplicationRole>> GetRootRolesAsync()
    {
        return await _context.Roles
            .Include(r => r.ChildRoles)
            .Where(r => r.ParentRoleId == null && !r.Deleted)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ApplicationRole>> GetChildRolesAsync(long parentRoleId)
    {
        return await _context.Roles
            .Include(r => r.ChildRoles)
            .Where(r => r.ParentRoleId == parentRoleId && !r.Deleted)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ApplicationRole>> GetAncestorsAsync(long roleId)
    {
        var ancestors = new List<ApplicationRole>();
        var currentRole = await _context.Roles
            .Include(r => r.ParentRole)
            .FirstOrDefaultAsync(r => r.Id == roleId && !r.Deleted);
            
        while (currentRole?.ParentRole != null)
        {
            ancestors.Add(currentRole.ParentRole);
            currentRole = currentRole.ParentRole;
        }
        
        return ancestors;
    }
    
    public async Task<IEnumerable<ApplicationRole>> GetAllAsync()
    {
        return await _context.Roles
            .Include(r => r.ParentRole)
            .Include(r => r.ChildRoles)
            .Where(r => !r.Deleted)
            .ToListAsync();
    }
} 