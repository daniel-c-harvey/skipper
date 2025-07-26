using Models.Shared.Converters;
using SkipperModels.Composites;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters;

public class SlipReservationOrderEntityToModelConverter : OrderEntityToModelConverter<VesselOwnerProfileEntity,
                                                                                      SlipReservationOrderCompositeEntity,
                                                                                      VesselOwnerOrderEntity,
                                                                                      SlipReservationEntity,
                                                                                      SlipReservationOrderCompositeModel,
                                                                                      SlipReservationModel,
                                                                                      SlipReservationEntityToModelConverter>
{
    
}