using Microsoft.AspNetCore.Mvc;
using Skipper.Domain.Entities;
using Skipper.Managers;
using System.Linq.Expressions;

namespace SkipperAPI.Controllers
{
    /// <summary>
    /// Ultra-minimal VesselController - now using nameof() for type safety!
    /// No hardcoded strings, no weird aliases, just clean and simple
    /// </summary>
    [Route("api/[controller]")]
    public class VesselController : BaseEntityController<Vessel>
    {
        public VesselController(IManager<Vessel> manager) : base(manager)
        {
            AddSortExpression(nameof(Vessel.Name), v => v.Name);
            AddSortExpression(nameof(Vessel.RegistrationNumber), v => v.RegistrationNumber);
            AddSortExpression(nameof(Vessel.Length), v => v.Length);
            AddSortExpression(nameof(Vessel.Beam), v => v.Beam);
            AddSortExpression(nameof(Vessel.VesselType), v => v.VesselType);
        }

        /// <summary>
        /// Vessel-specific search - only entity-specific code needed
        /// </summary>
        protected override Expression<Func<Vessel, bool>> BuildSearchPredicate(string? search)
            => string.IsNullOrEmpty(search) ? v => true 
               : v => v.Name.Contains(search) || v.RegistrationNumber.Contains(search);
    }
}