using AuthBlocksModels.Entities;
using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace AuthBlocksData.Data.Repositories;

public class PendingRegistrationRepository : Repository<AuthDbContext, PendingRegistration>, IPendingRegistrationRepository
{
    public PendingRegistrationRepository(AuthDbContext context, ILogger<Repository<AuthDbContext, PendingRegistration>> logger) : base(context, logger) { }

    protected override void UpdateModel(PendingRegistration target, PendingRegistration source)
    {
        base.UpdateModel(target, source);
        target.IsConsumed = source.IsConsumed;
        target.ConsumedAt = source.ConsumedAt;
        target.ExpiresAt = source.ExpiresAt;
    }
}