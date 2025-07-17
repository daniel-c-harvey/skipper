using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksData.Data.Repositories;
using AuthBlocksModels.Converters;
using AuthBlocksModels.Models;
using Data.Shared.Managers;
using Models.Shared.Common;
using NetBlocks.Models;
using NetBlocks.Utilities;

namespace AuthBlocksData.Services;

public class UserService : ManagerBase<ApplicationUser, UserModel, IUserRepository, UserEntityToModelConverter>, IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;


    public UserService(UserManager<ApplicationUser> userManager, IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository) 
        : base(userRepository)
    {
        _userManager = userManager;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
    }
    
    public async Task<ResultContainer<PagedResult<UserModel>>> GetPage(long userId, Expression<Func<ApplicationUser, bool>> predicate, PagingParameters<ApplicationUser> pagingParameters)
    {
        var result = await base.GetPage(predicate, pagingParameters);
        if (result.Value?.Items is null) return ResultContainer<PagedResult<UserModel>>.CreateFailResult(result.Messages.Select(m => m.Message).ToArray());
        
        await DecorateCanDelete(userId, result.Value.Items);
         
        return result;
    }

    private async Task DecorateCanDelete(long currentUserId, IEnumerable<UserModel> users)
    {
        var currentUserRoles = await _userRoleRepository.GetRolesAsync(currentUserId);
        foreach (UserModel user in users)
        {
            var userRoles = await _userRoleRepository.GetRolesAsync(user.Id);
            if (CanDeleteRoleCheck(userRoles, currentUserRoles))
            {
                user.CanDelete = user.Id != currentUserId;
            }
        }
    }

    private static bool CanDeleteRoleCheck(IEnumerable<ApplicationRole> userRoles, IEnumerable<ApplicationRole> currentUserRoles)
    {
        foreach (var role in userRoles)
        {
            foreach (var currentUserRole in currentUserRoles)
            {
                if (role.IsAncestorOf(currentUserRole))
                {
                    return false;
                }
            }
        }
        return true; // cleared all role relationships for delete
    }

    public override async Task<Result> Add(UserModel model)
    {
        try
        {
            var identityResult = await _userManager.CreateAsync(UserEntityToModelConverter.Convert(model));
            return identityResult.Succeeded ? Result.CreatePassResult() : Result.CreateFailResult(identityResult.Errors.Select(error => error.Description).ToArray());
        }
        catch (Exception ex)
        {
            return Result.CreateFailResult(ex.Message);
        }
    }
    
    public async Task<Result> Add(UserModel model, string password)
    {
        try 
        {
            var identityResult = await _userManager.CreateAsync(UserEntityToModelConverter.Convert(model), password);
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

    public async Task<UserModel?> FindByEmailAsync(string email)
    {
        var entity = await _userManager.FindByEmailAsync(email);
        return entity is null ? null : UserEntityToModelConverter.Convert(entity);
    }

    public async Task<UserModel?> FindByNameAsync(string userName)
    {
        var entity = await _userManager.FindByNameAsync(userName);
        return entity is null ? null : UserEntityToModelConverter.Convert(entity);
    }

    public async Task<bool> CheckPassword(UserModel user, string password)
    {
        var entity = await _userManager.FindByIdAsync(user.Id.ToString());
        if (entity is null) return false;
        return await _userManager.CheckPasswordAsync(entity, password);
    }
    
    public async Task UpdatePassword(UserModel user, string password)
    {
        var entity = await _userManager.FindByIdAsync(user.Id.ToString());
        if (entity is null) return;
        await _userManager.RemovePasswordAsync(entity);
        await _userManager.AddPasswordAsync(entity, password);
    }
} 