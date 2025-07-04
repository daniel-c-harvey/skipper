using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkipperData.Managers;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers;

[Route("api/[controller]")]
public class RentalAgreementController : BaseModelController<RentalAgreementEntity, RentalAgreementModel>
{
    public RentalAgreementController(IManager<RentalAgreementEntity, RentalAgreementModel> manager) : base(manager)
    {
        AddSortExpression(nameof(RentalAgreementEntity.SlipEntity.SlipNumber), e => e.SlipEntity.SlipNumber);
        AddSortExpression(nameof(RentalAgreementEntity.VesselEntity.RegistrationNumber), e => e.VesselEntity.RegistrationNumber);
        AddSortExpression(nameof(RentalAgreementEntity.VesselEntity.Name), e => e.VesselEntity.Name);
        AddSortExpression(nameof(RentalAgreementEntity.StartDate), e => e.StartDate);
        AddSortExpression(nameof(RentalAgreementEntity.EndDate), e => e.EndDate);
        AddSortExpression(nameof(RentalAgreementEntity.PriceRate), e => e.PriceRate);
        AddSortExpression(nameof(RentalAgreementEntity.PriceUnit), e => e.PriceUnit);
        AddSortExpression(nameof(RentalAgreementEntity.Status), e => e.Status);
    }

    protected override Expression<Func<RentalAgreementEntity, bool>> BuildSearchPredicate(string? search)
        => string.IsNullOrEmpty(search)
            ? s => true
            : s => EF.Functions.Like(s.SlipEntity.SlipNumber, $"%{search}%") ||
                   EF.Functions.Like(s.VesselEntity.RegistrationNumber, $"%{search}%") ||
                   EF.Functions.Like(s.VesselEntity.Name, $"%{search}%") ||
                   EF.Functions.Like(s.Status.ToString(), $"%{search}%");

}