using Microsoft.Extensions.Logging;
using SkipperModels;
using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Data.Repositories;

public class RentalAgreementRepository : Repository<RentalAgreementEntity, RentalAgreementModel>
{
    public RentalAgreementRepository(SkipperContext context, ILogger<RentalAgreementRepository> logger) : base(context, logger) { }

    public async Task<IEnumerable<RentalAgreementEntity>> GetByStatusAsync(RentalStatus status)
    {
        return await FindAsync(ra => ra.Status == status);
    }

    public async Task<PagedResult<RentalAgreementEntity>> GetByStatusPagedAsync(RentalStatus status, PagingParameters<RentalAgreementEntity> pagingParameters)
    {
        return await GetPagedAsync(ra => ra.Status == status, pagingParameters);
    }

    public async Task<IEnumerable<RentalAgreementEntity>> GetActiveAsync()
    {
        return await GetByStatusAsync(RentalStatus.Active);
    }

    public async Task<PagedResult<RentalAgreementEntity>> GetActivePagedAsync(PagingParameters<RentalAgreementEntity> pagingParameters)
    {
        return await GetByStatusPagedAsync(RentalStatus.Active, pagingParameters);
    }

    public async Task<IEnumerable<RentalAgreementEntity>> GetBySlipIdAsync(long slipId)
    {
        return await FindAsync(ra => ra.SlipId == slipId);
    }

    public async Task<PagedResult<RentalAgreementEntity>> GetBySlipIdPagedAsync(long slipId, PagingParameters<RentalAgreementEntity> pagingParameters)
    {
        return await GetPagedAsync(ra => ra.SlipId == slipId, pagingParameters);
    }

    public async Task<IEnumerable<RentalAgreementEntity>> GetByVesselIdAsync(long vesselId)
    {
        return await FindAsync(ra => ra.VesselId == vesselId);
    }

    public async Task<PagedResult<RentalAgreementEntity>> GetByVesselIdPagedAsync(long vesselId, PagingParameters<RentalAgreementEntity> pagingParameters)
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