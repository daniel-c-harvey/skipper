using SkipperData.Data.Repositories;
using SkipperModels.Composites;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class SlipReservationOrderManager : OrderManager<VesselOwnerProfileEntity,
                                                        SlipReservationOrderCompositeEntity,
                                                        VesselOwnerOrderEntity,
                                                        SlipReservationEntity,
                                                        SlipReservationOrderCompositeModel,
                                                        SlipReservationModel,
                                                        SlipReservationOrderRepository,
                                                        SlipReservationOrderEntityToModelConverter,
                                                        SlipReservationEntityToModelConverter>
{
    public SlipReservationOrderManager(SlipReservationOrderRepository repository) : base(repository)
    {
    }
}