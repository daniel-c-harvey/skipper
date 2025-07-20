using AuthBlocksData.Data.Repositories;
using AuthBlocksModels.Converters;
using AuthBlocksModels.Entities;
using AuthBlocksModels.Models;
using Data.Shared.Managers;
using NetBlocks.Models;

namespace AuthBlocksData.Services;

public class PendingRegistrationService : ManagerBase<PendingRegistration, PendingRegistrationModel, IPendingRegistrationRepository, PendingRegistrationEntityToModelConverter>, IPendingRegistrationService
{
    public PendingRegistrationService(IPendingRegistrationRepository repository) : base(repository)
    {
    }

    public async Task<ResultContainer<PendingRegistrationModel>> FindByEmail(string email)
    {
        try
        {
            var matches = await Repository.FindAsync(x => x.PendingUserEmail == email);
            if (matches.Count() != 1 || matches.FirstOrDefault() is not PendingRegistration pendingRegistration)
            {
                return ResultContainer<PendingRegistrationModel>.CreateFailResult("No pending registration found");
            }
            
            return ResultContainer<PendingRegistrationModel>.CreatePassResult
            (
                PendingRegistrationEntityToModelConverter.Convert(pendingRegistration)
            );
        }
        catch (Exception e)
        {
            return ResultContainer<PendingRegistrationModel>.CreateFailResult(e.Message);
        }
        
    }
}