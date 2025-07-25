using Microsoft.AspNetCore.Identity;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksData.Data.Repositories;
using AuthBlocksModels.Converters;
using AuthBlocksModels.Models;
using Data.Shared.Managers;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public class RoleService : ManagerBase<ApplicationRole, RoleModel, IRoleRepository, RoleEntityToModelConverter>, IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(RoleManager<ApplicationRole> roleManager, IRoleRepository roleRepository) : base(roleRepository)
    {
        _roleManager = roleManager;
    }

    public override async Task<Result> Add(RoleModel model)
    {
        try
        {
            var identityResult = await _roleManager.CreateAsync(RoleEntityToModelConverter.Convert(model));
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

    public async Task<ResultContainer<RoleModel>> FindByNameAsync(string roleName)
    {
        try
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null) return ResultContainer<RoleModel>.CreatePassResult().Inform("Role not found");
            return ResultContainer<RoleModel>.CreatePassResult(RoleEntityToModelConverter.Convert(role));
        }
        catch (Exception e)
        {
            return ResultContainer<RoleModel>.CreateFailResult(e.Message);
        }
    }
    
    // Hierarchy methods
    public async Task<ResultContainer<IEnumerable<RoleModel>>> GetRootRolesAsync()
    {
        try
        {
            var roles = await Repository.GetRootRolesAsync();
            return ResultContainer<IEnumerable<RoleModel>>.CreatePassResult(roles.Select(RoleEntityToModelConverter.Convert));
        }
        catch (Exception e)
        {
            return ResultContainer<IEnumerable<RoleModel>>.CreateFailResult(e.Message);
        }
    }
    
    public async Task<ResultContainer<IEnumerable<RoleModel>>> GetChildRolesAsync(long parentRoleId)
    {
        try
        {
            return ResultContainer<IEnumerable<RoleModel>>.CreatePassResult
            (
                (await Repository.GetChildRolesAsync(parentRoleId))
                .Select(RoleEntityToModelConverter.Convert)
            );
        }
        catch (Exception e)
        {
            return ResultContainer<IEnumerable<RoleModel>>.CreateFailResult(e.Message);
        }
    }
    
    public async Task<ResultContainer<IEnumerable<RoleModel>>> GetAncestorsAsync(long roleId)
    {

        try
        {
            return ResultContainer<IEnumerable<RoleModel>>.CreatePassResult
            (
                (await Repository.GetAncestorsAsync(roleId))
                .Select(RoleEntityToModelConverter.Convert)
            );
        }
        catch (Exception e)
        {
            return ResultContainer<IEnumerable<RoleModel>>.CreateFailResult(e.Message);
        }
    }
} 