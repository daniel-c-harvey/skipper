using Microsoft.Extensions.DependencyInjection;
using Skipper.Data.Repositories;

namespace Skipper.Data;

public static class RepositoryRegistration
{
    public static IServiceCollection AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<SlipRepository>();
        services.AddScoped<VesselRepository>();
        services.AddScoped<RentalAgreementRepository>();
        services.AddScoped<SlipClassificationRepository>();
        
        return services;
    }
} 