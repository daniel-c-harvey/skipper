using Microsoft.Extensions.Logging;
using SkipperModels;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class RentalAgreementRepository : Repository<RentalAgreement>
{
    public RentalAgreementRepository(SkipperContext context, ILogger<RentalAgreementRepository> logger) : base(context, logger) { }

    public async Task<IEnumerable<RentalAgreement>> GetByStatusAsync(RentalStatus status)
    {
        return await FindAsync(ra => ra.Status == status);
    }

    public async Task<PagedResult<RentalAgreement>> GetByStatusPagedAsync(RentalStatus status, PagingParameters<RentalAgreement> pagingParameters)
    {
        return await GetPagedAsync(ra => ra.Status == status, pagingParameters);
    }

    public async Task<IEnumerable<RentalAgreement>> GetActiveAsync()
    {
        return await GetByStatusAsync(RentalStatus.Active);
    }

    public async Task<PagedResult<RentalAgreement>> GetActivePagedAsync(PagingParameters<RentalAgreement> pagingParameters)
    {
        return await GetByStatusPagedAsync(RentalStatus.Active, pagingParameters);
    }

    public async Task<IEnumerable<RentalAgreement>> GetBySlipIdAsync(long slipId)
    {
        return await FindAsync(ra => ra.SlipId == slipId);
    }

    public async Task<PagedResult<RentalAgreement>> GetBySlipIdPagedAsync(long slipId, PagingParameters<RentalAgreement> pagingParameters)
    {
        return await GetPagedAsync(ra => ra.SlipId == slipId, pagingParameters);
    }

    public async Task<IEnumerable<RentalAgreement>> GetByVesselIdAsync(long vesselId)
    {
        return await FindAsync(ra => ra.VesselId == vesselId);
    }

    public async Task<PagedResult<RentalAgreement>> GetByVesselIdPagedAsync(long vesselId, PagingParameters<RentalAgreement> pagingParameters)
    {
        return await GetPagedAsync(ra => ra.VesselId == vesselId, pagingParameters);
    }

    public async Task<bool> HasConflictAsync(long slipId, DateTime startDate, DateTime endDate)
    {
        var conflicts = await FindAsync(ra => 
            ra.SlipId == slipId && 
            ra.Status == RentalStatus.Active &&
            ra.StartDate < endDate && 
            ra.EndDate > startDate);
        
        return conflicts.Any();
    }
} 