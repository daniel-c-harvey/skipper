using Microsoft.AspNetCore.Mvc;
using SkipperData.Managers;
using System.Linq.Expressions;
using SkipperModels.Entities;

namespace SkipperAPI.Controllers
{
    /// <summary>
    /// SlipController - demonstrates the simple nameof() approach
    /// Clean, type-safe, no hardcoded strings
    /// </summary>
    [Route("api/[controller]")]
    public class SlipController : BaseEntityController<Slip>
    {
        public SlipController(IManager<Slip> manager) : base(manager)
        {
            AddSortExpression(nameof(Slip.SlipNumber), s => s.SlipNumber);
            AddSortExpression(nameof(Slip.LocationCode), s => s.LocationCode);
            AddSortExpression(nameof(Slip.Status), s => s.Status);
            AddSortExpression(nameof(Slip.SlipClassificationId), s => s.SlipClassificationId);
        }
        
        protected override Expression<Func<Slip, bool>> BuildSearchPredicate(string? search)
            => string.IsNullOrEmpty(search) ? s => true 
               : s => s.SlipNumber.Contains(search) || s.LocationCode.Contains(search);
    }
} 