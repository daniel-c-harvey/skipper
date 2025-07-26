using Microsoft.EntityFrameworkCore;
using AuthBlocksModels.Entities.Identity;
using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace AuthBlocksData.Data.Repositories;

public class RoleRepository : Repository<AuthDbContext, ApplicationRole>, IRoleRepository
{
    public RoleRepository(AuthDbContext context, ILogger<Repository<AuthDbContext, ApplicationRole>> logger) : base(context, logger) { }

    protected override void UpdateModel(ApplicationRole target, ApplicationRole source)
    {
        base.UpdateModel(target, source);
        target.ParentRoleId = source.ParentRoleId;
        target.Name = source.Name;
        target.NormalizedName = source.NormalizedName;
    }
    
    public async Task<ApplicationRole?> GetByNameAsync(string normalizedName)
    {
        return await Context.Roles
            .Include(r => r.ParentRole)
            .Include(r => r.ChildRoles)
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedName && !r.IsDeleted);
    }
    
    // Simple hierarchy methods
    public async Task<IEnumerable<ApplicationRole>> GetRootRolesAsync()
    {
        return await Context.Roles
            .Include(r => r.ChildRoles)
            .Where(r => r.ParentRoleId == null && !r.IsDeleted)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ApplicationRole>> GetChildRolesAsync(long parentRoleId)
    {
        return await Context.Roles
            .Include(r => r.ChildRoles)
            .Where(r => r.ParentRoleId == parentRoleId && !r.IsDeleted)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ApplicationRole>> GetAncestorsAsync(long roleId)
    {
        var ancestors = new List<ApplicationRole>();
        var currentRole = await Context.Roles
            .Include(r => r.ParentRole)
            .FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted);
            
        while (currentRole?.ParentRole != null)
        {
            ancestors.Add(currentRole.ParentRole);
            currentRole = currentRole.ParentRole;
        }
        
        return ancestors;
    }
} 