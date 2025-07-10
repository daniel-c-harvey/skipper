using Data.Shared.Data.Repositories;
using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class RentalAgreementManager : ManagerBase<RentalAgreementEntity, RentalAgreementModel>
{
    public RentalAgreementManager(IRepository<RentalAgreementEntity, RentalAgreementModel> repository) : base(repository) { }
}