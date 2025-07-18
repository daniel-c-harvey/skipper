using AuthBlocksData.Data.Repositories;
using AuthBlocksModels.Converters;
using AuthBlocksModels.Entities;
using AuthBlocksModels.Models;
using Data.Shared.Managers;

namespace AuthBlocksData.Services;

public class PendingRegistrationService : ManagerBase<PendingRegistration, PendingRegistrationModel, IPendingRegistrationRepository, PendingRegistrationEntityToModelConverter>, IPendingRegistrationService
{
    public PendingRegistrationService(IPendingRegistrationRepository repository) : base(repository)
    {
    }
    
}