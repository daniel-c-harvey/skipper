using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Models;
using Web.Shared.ApiClients;

namespace AuthBlocksWeb.ApiClients;

public interface IPendingRegistrationClient : IModelClient<PendingRegistrationModel>
{
    Task<TokenCreationResult> CreatePendingRegistration(string email);
}