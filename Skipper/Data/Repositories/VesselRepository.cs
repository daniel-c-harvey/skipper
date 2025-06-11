using Skipper.Domain;
using Skipper.Domain.Entities;

namespace Skipper.Data.Repositories;

public class VesselRepository : Repository<Vessel>
{
    public VesselRepository(SkipperContext context) : base(context) { }

    public async Task<Vessel?> GetByRegistrationNumberAsync(string registrationNumber)
    {
        var results = await FindAsync(v => v.RegistrationNumber == registrationNumber);
        return results.FirstOrDefault();
    }

    public async Task<IEnumerable<Vessel>> GetByTypeAsync(VesselType type)
    {
        return await FindAsync(v => v.VesselType == type);
    }

    public async Task<IEnumerable<Vessel>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return Enumerable.Empty<Vessel>();
            
        return await FindAsync(v => v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }
} 