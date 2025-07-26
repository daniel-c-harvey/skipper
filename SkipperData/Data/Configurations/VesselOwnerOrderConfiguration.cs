using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkipperModels.Entities;

namespace SkipperData.Data.Configurations;

public class VesselOwnerOrderConfiguration : OrderConfiguration<VesselOwnerProfileEntity, VesselOwnerOrderEntity>
{
    public override void Configure(EntityTypeBuilder<VesselOwnerOrderEntity> builder)
    {
        builder.ToTable("vessel_owner_orders");
        
        base.Configure(builder);
    }
}