﻿using System.Linq.Expressions;
using API.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkipperData.Managers;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers;

[Route("api/[controller]")]
public class SlipClassificationController : BaseModelController<SlipClassificationEntity, SlipClassificationModel, SlipClassificationManager>
{
    public SlipClassificationController(SlipClassificationManager manager) : base(manager)
    {
        AddSortExpression(nameof(SlipClassificationEntity.Name), sc => sc.Name);
        AddSortExpression(nameof(SlipClassificationEntity.Description), sc => sc.Description);
        AddSortExpression(nameof(SlipClassificationEntity.BasePrice), sc => sc.BasePrice);
        AddSortExpression(nameof(SlipClassificationEntity.MaxLength), sc => sc.MaxLength);
        AddSortExpression(nameof(SlipClassificationEntity.MaxBeam), sc => sc.MaxBeam);
    }

    protected override Expression<Func<SlipClassificationEntity, bool>> BuildSearchPredicate(string? search)
        => string.IsNullOrEmpty(search)
            ? s => true
            : s => EF.Functions.ILike(s.Name, $"%{search}%") || 
                EF.Functions.ILike(s.Description, $"%{search}%") || 
                EF.Functions.ILike(s.BasePrice.ToString(), $"%{search}%");
}