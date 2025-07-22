using Data.Shared.Managers;
using Microsoft.Extensions.DependencyInjection;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public static class ManagerRegistration
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped<VesselManager>();
        services.AddScoped<SlipManager>();
        services.AddScoped<SlipClassificationManager>();
        services.AddScoped<SlipReservationManager>();
        
        return services;
    }
}