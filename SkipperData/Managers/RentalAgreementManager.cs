using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class RentalAgreementManager : ManagerBase<RentalAgreementEntity, RentalAgreementModel, RentalAgreementRepository, RentalAgreementEntityToModelConverter>
{
    public RentalAgreementManager(RentalAgreementRepository repository) : base(repository) { }
}