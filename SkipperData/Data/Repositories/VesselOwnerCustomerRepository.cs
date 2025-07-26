using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class VesselOwnerCustomerRepository : Repository<SkipperContext, VesselOwnerCustomerEntity>
{
    public VesselOwnerCustomerRepository(SkipperContext context, ILogger<VesselOwnerCustomerRepository> logger) 
        : base(context, logger)
    {
    }
} 