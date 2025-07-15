using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;

namespace SkipperData.Managers;

public class SlipManager : ManagerBase<SlipEntity, SlipRepository>
{
    public SlipManager(SlipRepository repository) : base(repository) { }
}