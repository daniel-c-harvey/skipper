using System.Linq.Expressions;
using AuthBlocksModels.Entities;
using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;

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
        target.RoleIds = source.RoleIds;
    }

    public override async Task<PendingRegistration?> GetByIdAsync(long id)
    {
        var pendingRegistration = await base.GetByIdAsync(id);
        PopulatePendingRegistrationRoles(pendingRegistration);
        return pendingRegistration;
    }

    public override async Task<IEnumerable<PendingRegistration>> GetAllAsync()
    {
        var pendingRegistrations = await base.GetAllAsync();
        PopulatePendingRegistrationRoles(pendingRegistrations);
        return pendingRegistrations;
    }

    public override async Task<IEnumerable<PendingRegistration>> FindAsync(Expression<Func<PendingRegistration, bool>> predicate)
    {
        var pendingRegistrations = await base.FindAsync(predicate);
        PopulatePendingRegistrationRoles(pendingRegistrations);
        return pendingRegistrations;
    }

    public override async Task<PagedResult<PendingRegistration>> GetPagedAsync(PagingParameters<PendingRegistration> pagingParameters)
    {
        var page = await base.GetPagedAsync(pagingParameters);
        PopulatePendingRegistrationRoles(page.Items);
        return page;
    }

    public override async Task<PagedResult<PendingRegistration>> GetPagedAsync(Expression<Func<PendingRegistration, bool>> predicate, PagingParameters<PendingRegistration> pagingParameters)
    {
        var page = await base.GetPagedAsync(predicate, pagingParameters);
        PopulatePendingRegistrationRoles(page.Items);
        return page;
    }

    public override async Task<PendingRegistration> AddAsync(PendingRegistration entity)
    {
        var pendingRegistration = await base.AddAsync(entity);
        PopulatePendingRegistrationRoles(pendingRegistration);
        return pendingRegistration;
    }

    private void PopulatePendingRegistrationRoles(IEnumerable<PendingRegistration>? pendingRegistrations)
    {
        foreach (var pendingRegistration in pendingRegistrations ?? [])
        {
            PopulatePendingRegistrationRoles(pendingRegistration);
        }
    }
    
    private void PopulatePendingRegistrationRoles(PendingRegistration? pendingRegistration)
    {
        if (pendingRegistration is { RoleIds: not null })
            pendingRegistration.Roles = Context.Roles.Where(r => pendingRegistration.RoleIds.Contains(r.Id)).ToList();
    }
}