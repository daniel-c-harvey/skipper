using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;

namespace SkipperData.Managers;

public class SlipClassificationManager : ManagerBase<SlipClassificationEntity, SlipClassificationRepository>
{
    public SlipClassificationManager(SlipClassificationRepository repository) : base(repository) { }
}