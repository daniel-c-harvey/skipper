using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Shared.Entities;

namespace SkipperModels.Entities
{
    public class VesselOwnerVesselEntity : BaseLinkageEntity
    {
        public long VesselOwnerProfileId { get; set; }
        public virtual VesselOwnerProfileEntity VesselOwnerProfile { get; set; }
        public long VesselId { get; set; }
        public virtual VesselEntity Vessel { get; set; }
    }
}