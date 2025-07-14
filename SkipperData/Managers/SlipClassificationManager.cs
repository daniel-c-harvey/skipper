using Data.Shared.Data.Repositories;
using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class SlipClassificationManager : ManagerBase<SlipClassificationEntity, SlipClassificationModel, SlipClassificationRepository>
{
    public SlipClassificationManager(SlipClassificationRepository repository) : base(repository) { }
}