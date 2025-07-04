using Microsoft.AspNetCore.Mvc;
using SkipperData.Managers;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers
{
    [Route("api/[controller]")]
    public class VesselController : BaseModelController<VesselEntity, VesselModel>
    {
        public VesselController(IManager<VesselEntity, VesselModel> manager) : base(manager)
        {
            AddSortExpression(nameof(VesselEntity.Name), v => v.Name);
            AddSortExpression(nameof(VesselEntity.RegistrationNumber), v => v.RegistrationNumber);
            AddSortExpression(nameof(VesselEntity.Length), v => v.Length);
            AddSortExpression(nameof(VesselEntity.Beam), v => v.Beam);
            AddSortExpression(nameof(VesselEntity.VesselType), v => v.VesselType);
        }

        /// <summary>
        /// Vessel-specific search - only entity-specific code needed
        /// </summary>
        protected override Expression<Func<VesselEntity, bool>> BuildSearchPredicate(string? search)
            => string.IsNullOrEmpty(search) ? v => true 
               : v => EF.Functions.ILike(v.Name, $"%{search}%") ||
                    EF.Functions.ILike(v.RegistrationNumber, $"%{search}%");
    }
}