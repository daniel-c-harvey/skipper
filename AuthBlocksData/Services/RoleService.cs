using Microsoft.AspNetCore.Identity;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using AuthBlocksData.Data.Repositories;
using Data.Shared.Managers;
using Models.Shared.Common;
using NetBlocks.Models;
using System.Linq.Expressions;
using Models.Shared.Entities;

namespace AuthBlocksData.Services;

public class RoleService : ManagerBase<ApplicationRole, RoleModel, IRoleRepository>, IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(RoleManager<ApplicationRole> roleManager, IRoleRepository roleRepository) : base(roleRepository)
    {
        _roleManager = roleManager;
    }

    public override async Task<Result> Add(ApplicationRole entity)
    {
        try
        {
            var identityResult = await _roleManager.CreateAsync(entity);
            if (identityResult.Succeeded)
            {
                return Result.CreatePassResult();
            }
            else
            {
                return Result.CreateFailResult(identityResult.Errors.Select(error => error.Description).ToArray());
            }
        }
        catch (Exception ex)
        {
            return Result.CreateFailResult(ex.Message);
        }
    }

    public async Task<ApplicationRole?> FindByNameAsync(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }
    
    // Hierarchy methods
    public async Task<IEnumerable<ApplicationRole>> GetRootRolesAsync()
    {
        return await _repository.GetRootRolesAsync();
    }
    
    public async Task<IEnumerable<ApplicationRole>> GetChildRolesAsync(long parentRoleId)
    {
        return await _repository.GetChildRolesAsync(parentRoleId);
    }
    
    public async Task<IEnumerable<ApplicationRole>> GetAncestorsAsync(long roleId)
    {
        return await _repository.GetAncestorsAsync(roleId);
    }
} 