using System.Linq.Expressions;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Data.Shared.Managers;
using Microsoft.AspNetCore.Identity;
using Models.Shared.Common;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public interface IRoleService : IManager<ApplicationRole, RoleModel>
{
    Task<ApplicationRole?> FindByNameAsync(string roleName);
    Task<IEnumerable<ApplicationRole>> GetRootRolesAsync();
    Task<IEnumerable<ApplicationRole>> GetChildRolesAsync(long parentRoleId);
    Task<IEnumerable<ApplicationRole>> GetAncestorsAsync(long roleId);
}