using Data.Shared.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Data;

public static class RepositoryRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Slip-related repositories
        services.AddScoped<SlipRepository>();
        services.AddScoped<VesselRepository>();
        services.AddScoped<SlipClassificationRepository>();
        services.AddScoped<SlipReservationRepository>();
        services.AddScoped<SlipReservationOrderRepository>();
        
        // Customer repositories (TPH)
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IVesselOwnerCustomerRepository, VesselOwnerCustomerRepository>();
        services.AddScoped<IBusinessCustomerRepository, BusinessCustomerRepository>();
        
        return services;
    }
} 