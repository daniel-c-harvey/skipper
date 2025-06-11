using Skipper.Domain;
using Skipper.Domain.Entities;

namespace Skipper.Data.Repositories;

public class RentalAgreementRepository : Repository<RentalAgreement>
{
    public RentalAgreementRepository(SkipperContext context) : base(context) { }

    public async Task<IEnumerable<RentalAgreement>> GetByStatusAsync(RentalStatus status)
    {
        return await FindAsync(ra => ra.Status == status);
    }

    public async Task<IEnumerable<RentalAgreement>> GetActiveAsync()
    {
        return await GetByStatusAsync(RentalStatus.Active);
    }

    public async Task<IEnumerable<RentalAgreement>> GetBySlipIdAsync(long slipId)
    {
        return await FindAsync(ra => ra.SlipId == slipId);
    }

    public async Task<IEnumerable<RentalAgreement>> GetByVesselIdAsync(long vesselId)
    {
        return await FindAsync(ra => ra.VesselId == vesselId);
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