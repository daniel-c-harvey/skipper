using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class RentalAgreementRepository : Repository<SkipperContext, RentalAgreementEntity>
{
    public RentalAgreementRepository(SkipperContext context, ILogger<RentalAgreementRepository> logger) : base(context, logger) { }
    
    protected override void UpdateModel(RentalAgreementEntity target, RentalAgreementEntity source)
    {
        base.UpdateModel(target, source);
        target.EndDate = source.EndDate;
        target.StartDate = source.StartDate;
        target.Status = source.Status;
        target.SlipId = source.SlipId;
        target.VesselId = source.VesselId;
        target.PriceUnit = source.PriceUnit;
        target.PriceRate = source.PriceRate;
    }

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