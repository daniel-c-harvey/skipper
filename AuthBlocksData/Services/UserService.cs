using Microsoft.AspNetCore.Identity;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksData.Data.Repositories;
using AuthBlocksModels.Converters;
using AuthBlocksModels.Models;
using Data.Shared.Managers;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public class UserService : ManagerBase<ApplicationUser, UserModel, IUserRepository, UserEntityToModelConverter>, IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager, IUserRepository userRepository) 
        : base(userRepository)
    {
        _userManager = userManager;
    }

    public override async Task<Result> Add(UserModel model)
    {
        try 
        {
            var identityResult = await _userManager.CreateAsync(UserEntityToModelConverter.Convert(model));
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
        return await _userManager.CheckPasswordAsync(UserEntityToModelConverter.Convert(user), password);
    }
    
    public async Task UpdatePassword(UserModel user, string password)
    {
        var entity = UserEntityToModelConverter.Convert(user);
        await _userManager.RemovePasswordAsync(entity);
        await _userManager.AddPasswordAsync(entity, password);
    }
} 