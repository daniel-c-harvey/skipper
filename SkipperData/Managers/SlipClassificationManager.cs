using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class SlipClassificationManager : Manager<SlipClassificationEntity, SlipClassificationModel, SlipClassificationRepository, SlipClassificationEntityToModelConverter>
{
    public SlipClassificationManager(SlipClassificationRepository repository) : base(repository) { }
}