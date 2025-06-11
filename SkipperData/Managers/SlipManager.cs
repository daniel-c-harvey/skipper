using SkipperData.Data.Repositories;
using SkipperModels.Entities;

namespace SkipperData.Managers;

public class SlipManager : ManagerBase<Slip>
{
    public SlipManager(IRepository<Slip> repository) : base(repository) { }
}