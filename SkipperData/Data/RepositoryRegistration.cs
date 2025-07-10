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
        // services.AddScoped(typeof(IRepository<,>), typeof(Repository<,,>));
        services.AddScoped<IRepository<SlipEntity, SlipModel>, SlipRepository>();
        services.AddScoped<IRepository<VesselEntity, VesselModel>, VesselRepository>();
        services.AddScoped<IRepository<SlipClassificationEntity, SlipClassificationModel>, SlipClassificationRepository>();
        services.AddScoped<IRepository<RentalAgreementEntity, RentalAgreementModel>, RentalAgreementRepository>();
        
        return services;
    }
} 