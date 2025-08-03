using Data.Shared.Managers;
using Microsoft.Extensions.DependencyInjection;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public static class ManagerRegistration
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        // Slip-related managers
        services.AddScoped<VesselManager>();
        services.AddScoped<SlipManager>();
        services.AddScoped<SlipClassificationManager>();
        services.AddScoped<SlipReservationOrderManager>();
        
        // Customer managers (TPH)
        services.AddScoped<VesselOwnerCustomerManager>();
        services.AddScoped<BusinessCustomerManager>();
        
        return services;
    }
}