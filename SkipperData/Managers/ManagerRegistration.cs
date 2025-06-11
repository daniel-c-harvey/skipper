using Microsoft.Extensions.DependencyInjection;
using SkipperModels.Entities;

namespace SkipperData.Managers;

public static class ManagerRegistration
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped<IManager<Vessel>, VesselManager>();
        services.AddScoped<IManager<Slip>, SlipManager>();
        
        return services;
    }
}