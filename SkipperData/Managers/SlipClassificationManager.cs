using SkipperData.Data.Repositories;
using SkipperModels.Entities;

namespace SkipperData.Managers;

public class SlipClassificationManager : ManagerBase<SlipClassificationEntity, SlipClassificationModel>
{
    public SlipClassificationManager(IRepository<SlipClassificationEntity, SlipClassificationModel> repository) : base(repository) { }
}