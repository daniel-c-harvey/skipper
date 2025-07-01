using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using SkipperData.Managers;
using SkipperModels.Entities;

namespace SkipperAPI.Controllers;

[Route("api/[controller]")]
public class SlipClassificationController : BaseModelController<SlipClassificationEntity, SlipClassificationModel>
{
    public SlipClassificationController(IManager<SlipClassificationEntity, SlipClassificationModel> manager) : base(manager)
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
            : s => s.Name.Contains(search) || 
                s.Description.Contains(search) || 
                s.BasePrice.ToString().Contains(search);
}