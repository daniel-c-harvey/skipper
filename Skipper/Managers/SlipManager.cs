using Skipper.Data.Repositories;
using Skipper.Domain.Entities;

namespace Skipper.Managers;

public class SlipManager : ManagerBase<Slip>
{
    public SlipManager(IRepository<Slip> repository) : base(repository) { }
}