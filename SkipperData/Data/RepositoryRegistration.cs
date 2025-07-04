using Microsoft.Extensions.DependencyInjection;
using SkipperData.Data.Repositories;

namespace SkipperData.Data;

public static class RepositoryRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<SlipRepository>();
        services.AddScoped<VesselRepository>();
        services.AddScoped<SlipClassificationRepository>();
        services.AddScoped<RentalAgreementRepository>();
        
        return services;
    }
} 