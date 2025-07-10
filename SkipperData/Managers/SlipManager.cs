using Data.Shared.Data.Repositories;
using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class SlipManager : ManagerBase<SlipEntity, SlipModel>
{
    public SlipManager(IRepository<SlipEntity, SlipModel> repository) : base(repository) { }
}