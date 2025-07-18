using AuthBlocksModels.Entities;
using AuthBlocksModels.Models;
using Data.Shared.Managers;

namespace AuthBlocksData.Services;

public interface IPendingRegistrationService : IManager<PendingRegistration, PendingRegistrationModel>
{
    
}