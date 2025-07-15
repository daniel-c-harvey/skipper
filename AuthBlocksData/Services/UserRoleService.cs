using AuthBlocksData.Data.Repositories;
using AuthBlocksModels.Entities.Identity;
using Data.Shared.Managers;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public class UserRoleService : ManagerBase<ApplicationUserRole, IUserRoleRepository>, IUserRoleService
{
    public UserRoleService(IUserRoleRepository repository) : base(repository)
    {
    }

    public async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
    {
        // TODO error handling and results
        await Repository.AddToRoleAsync(user, roleName);
    }

    public async Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName)
    {
        await Repository.RemoveFromRoleAsync(user, roleName);
    }

    public async Task<ResultContainer<IEnumerable<ApplicationRole>>> GetByUser(ApplicationUser user)
    {
        try
        {
            return ResultContainer<IEnumerable<ApplicationRole>>.CreatePassResult(await Repository.GetRolesAsync(user));
        }
        catch (Exception ex)
        {
            return ResultContainer<IEnumerable<ApplicationRole>>.CreateFailResult(ex.Message);
        }
    }
}