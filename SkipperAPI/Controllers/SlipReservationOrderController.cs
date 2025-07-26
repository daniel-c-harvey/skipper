using Microsoft.AspNetCore.Mvc;
using SkipperData.Data.Repositories;
using SkipperData.Managers;
using SkipperModels.Composites;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SlipReservationOrderController 
    : OrderController<
        VesselOwnerProfileEntity,
        SlipReservationOrderCompositeEntity,
        VesselOwnerOrderEntity,
        SlipReservationEntity,
        SlipReservationOrderCompositeModel,
        SlipReservationModel,
        SlipReservationOrderManager,
        SlipReservationOrderRepository,
        SlipReservationOrderEntityToModelConverter,
        SlipReservationEntityToModelConverter>
{
    public SlipReservationOrderController(SlipReservationOrderManager manager) 
    : base(manager)
    {
    }
}