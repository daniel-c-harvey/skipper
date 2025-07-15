using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;

namespace SkipperData.Managers;

public class RentalAgreementManager : ManagerBase<RentalAgreementEntity, RentalAgreementRepository>
{
    public RentalAgreementManager(RentalAgreementRepository repository) : base(repository) { }
}