using AuthBlocksModels.Entities;
using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace AuthBlocksData.Data.Repositories;

public class PendingRegistrationRepository : Repository<AuthDbContext, PendingRegistration>, IPendingRegistrationRepository
{
    public PendingRegistrationRepository(AuthDbContext context, ILogger<Repository<AuthDbContext, PendingRegistration>> logger) : base(context, logger) { }
    
    // public Task<PendingRegistration?> GetByEmailAsync(string email)
}