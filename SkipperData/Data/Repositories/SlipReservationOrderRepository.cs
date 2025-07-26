using Microsoft.Extensions.Logging;
using SkipperModels;
using SkipperModels.Composites;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class SlipReservationOrderRepository : OrderRepository<VesselOwnerProfileEntity, SlipReservationOrderCompositeEntity, VesselOwnerOrderEntity, SlipReservationEntity>
{
    public SlipReservationOrderRepository(SkipperContext context,
                                          ILogger<SlipReservationOrderRepository> logger) 
    : base(context, OrderType.SlipReservation, logger)
    {
        
    }
}