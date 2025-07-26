using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class SlipManager : Manager<SlipEntity, SlipModel, SlipRepository, SlipEntityToModelConverter>
{
    public SlipManager(SlipRepository repository) : base(repository) { }
}