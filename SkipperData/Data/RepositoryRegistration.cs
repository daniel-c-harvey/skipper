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
        services.AddScoped<SlipRepository>();
        services.AddScoped<VesselRepository>();
        services.AddScoped<SlipClassificationRepository>();
        services.AddScoped<SlipReservationRepository>();
        
        return services;
    }
} 