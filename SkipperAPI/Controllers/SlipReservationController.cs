using System.Linq.Expressions;
using API.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkipperData.Managers;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers;

[Route("api/[controller]")]
public class SlipReservationController : BaseModelController<SlipReservationEntity, SlipReservationModel, SlipReservationManager>
{
    public SlipReservationController(SlipReservationManager manager) : base(manager)
    {
        AddSortExpression(nameof(SlipReservationEntity.SlipEntity.SlipNumber), e => e.SlipEntity.SlipNumber);
        AddSortExpression(nameof(SlipReservationEntity.VesselEntity.RegistrationNumber), e => e.VesselEntity.RegistrationNumber);
        AddSortExpression(nameof(SlipReservationEntity.VesselEntity.Name), e => e.VesselEntity.Name);
        AddSortExpression(nameof(SlipReservationEntity.StartDate), e => e.StartDate);
        AddSortExpression(nameof(SlipReservationEntity.EndDate), e => e.EndDate);
        AddSortExpression(nameof(SlipReservationEntity.PriceRate), e => e.PriceRate);
        AddSortExpression(nameof(SlipReservationEntity.PriceUnit), e => e.PriceUnit);
        AddSortExpression(nameof(SlipReservationEntity.Status), e => e.Status);
    }

    protected override Expression<Func<SlipReservationEntity, bool>> BuildSearchPredicate(string? search)
        => string.IsNullOrEmpty(search)
            ? s => true
            : s => EF.Functions.Like(s.SlipEntity.SlipNumber, $"%{search}%") ||
                   EF.Functions.Like(s.VesselEntity.RegistrationNumber, $"%{search}%") ||
                   EF.Functions.Like(s.VesselEntity.Name, $"%{search}%") ||
                   EF.Functions.Like(s.Status.ToString(), $"%{search}%");

} 