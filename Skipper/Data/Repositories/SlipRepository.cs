using Skipper.Domain;
using Skipper.Domain.Entities;

namespace Skipper.Data.Repositories;

public class SlipRepository : Repository<Slip>
{
    public SlipRepository(SkipperContext context) : base(context) { }

    public async Task<IEnumerable<Slip>> GetByStatusAsync(SlipStatus status)
    {
        return await FindAsync(s => s.Status == status);
    }

    public async Task<IEnumerable<Slip>> GetAvailableAsync()
    {
        return await GetByStatusAsync(SlipStatus.Available);
    }

    public async Task<Slip?> GetBySlipNumberAsync(string slipNumber)
    {
        var results = await FindAsync(s => s.SlipNumber == slipNumber);
        return results.FirstOrDefault();
    }
} 