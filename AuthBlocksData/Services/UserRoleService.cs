using AuthBlocksData.Data.Repositories;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Data.Shared.Managers;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public class UserRoleService : ManagerBase<ApplicationUserRole, UserRoleModel, IUserRoleRepository>, IUserRoleService
{
    public UserRoleService(IUserRoleRepository repository) : base(repository)
    {
    }

    public async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
    {
        // TODO error handling and results
        await _repository.AddToRoleAsync(user, roleName);
    }

    public async Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName)
    {
        await _repository.RemoveFromRoleAsync(user, roleName);
    }

    public async Task<ResultContainer<IEnumerable<ApplicationRole>>> GetByUser(ApplicationUser user)
    {
        try
        {
            return ResultContainer<IEnumerable<ApplicationRole>>.CreatePassResult(await _repository.GetRolesAsync(user));
        }
        catch (Exception ex)
        {
            return ResultContainer<IEnumerable<ApplicationRole>>.CreateFailResult(ex.Message);
        }
    }
}