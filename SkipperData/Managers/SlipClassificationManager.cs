using Data.Shared.Data.Repositories;
using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class SlipClassificationManager : ManagerBase<SlipClassificationEntity, SlipClassificationModel>
{
    public SlipClassificationManager(IRepository<SlipClassificationEntity, SlipClassificationModel> repository) : base(repository) { }
}