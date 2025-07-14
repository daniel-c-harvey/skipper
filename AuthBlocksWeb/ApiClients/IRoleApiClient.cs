using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Models.Shared.Models;
using NetBlocks.Models;
using Web.Shared.ApiClients;

namespace AuthBlocksWeb.ApiClients;

public interface IRoleApiClient : IModelControllerClient<RoleModel, ApplicationRole>
{
} 