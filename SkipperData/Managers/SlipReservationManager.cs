using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class SlipReservationManager : Manager<SlipReservationEntity, SlipReservationModel, SlipReservationRepository, SlipReservationEntityToModelConverter>
{
    public SlipReservationManager(SlipReservationRepository repository) : base(repository) { }
} 