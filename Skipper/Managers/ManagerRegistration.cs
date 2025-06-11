using Microsoft.Extensions.DependencyInjection;
using Skipper.Domain.Entities;

namespace Skipper.Managers;

public static class ManagerRegistration
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped<IManager<Vessel>, VesselManager>();
        services.AddScoped<IManager<Slip>, SlipManager>();
        
        return services;
    }
}