using Microsoft.Extensions.DependencyInjection;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public static class ManagerRegistration
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped<IManager<VesselEntity, VesselModel>, VesselManager>();
        services.AddScoped<IManager<SlipEntity, SlipModel>, SlipManager>();
        services.AddScoped<IManager<SlipClassificationEntity, SlipClassificationModel>, SlipClassificationManager>();
        
        return services;
    }
}