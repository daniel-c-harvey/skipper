using Microsoft.AspNetCore.Mvc;
using SkipperData.Managers;
using System.Linq.Expressions;
using API.Shared.Controllers;
using Microsoft.EntityFrameworkCore;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers
{
    [Route("api/[controller]")]
    public class SlipController : ModelController<SlipEntity, SlipModel, SlipManager>
    {
        public SlipController(SlipManager manager) : base(manager)
        {
            AddSortExpression(nameof(SlipEntity.SlipNumber), s => s.SlipNumber);
            AddSortExpression(nameof(SlipEntity.LocationCode), s => s.LocationCode);
            AddSortExpression(nameof(SlipEntity.Status), s => s.Status);
            AddSortExpression(nameof(SlipEntity.SlipClassificationId), s => s.SlipClassificationId);
        }
        
        protected override Expression<Func<SlipEntity, bool>> BuildSearchPredicate(string? search)
            => string.IsNullOrEmpty(search) ? s => true 
               : s => EF.Functions.ILike(s.SlipNumber, $"%{search}%") || 
                      EF.Functions.ILike(s.LocationCode, $"%{search}%") || 
                      EF.Functions.ILike(s.SlipClassificationEntity.Name, $"%{search}%");
    }
} 