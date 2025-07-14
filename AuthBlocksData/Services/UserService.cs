using Microsoft.AspNetCore.Identity;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using AuthBlocksData.Data.Repositories;
using Data.Shared.Managers;
using Data.Shared.Data.Repositories;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public class UserService : ManagerBase<ApplicationUser, UserModel, IUserRepository>, IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager, IUserRepository userRepository) 
        : base(userRepository)
    {
        _userManager = userManager;
    }

    public override async Task<Result> Add(ApplicationUser entity)
    {
        try 
        {
            var identityResult = await _userManager.CreateAsync(entity);
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
    
    public async Task<Result> Add(ApplicationUser entity, string password)
    {
        try 
        {
            var identityResult = await _userManager.CreateAsync(entity, password);
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

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> FindByNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<bool> CheckPassword(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }
    
    public async Task UpdatePassword(ApplicationUser user, string password)
    {
        await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, password);
    }
} 