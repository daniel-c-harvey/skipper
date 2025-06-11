using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Skipper.Data.Repositories;
using Skipper.Domain.Entities;

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

    /// <summary>
    /// Creates a SkipperContext with the provided connection string for console applications
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Configured SkipperContext instance</returns>
    public static SkipperContext CreateContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SkipperContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new SkipperContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Creates a SkipperContext with custom DbContextOptions for console applications
    /// </summary>
    /// <param name="options">Pre-configured DbContextOptions</param>
    /// <returns>SkipperContext instance</returns>
    public static SkipperContext CreateContext(DbContextOptions<SkipperContext> options)
    {
        return new SkipperContext(options);
    }

    /// <summary>
    /// Creates a SlipRepository instance for console applications
    /// </summary>
    /// <param name="context">SkipperContext instance</param>
    /// <returns>SlipRepository instance</returns>
    public static SlipRepository CreateSlipRepository(SkipperContext context)
    {
        return new SlipRepository(context);
    }

    /// <summary>
    /// Creates a SlipRepository instance with connection string for console applications
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>SlipRepository instance with its own context</returns>
    public static SlipRepository CreateSlipRepository(string connectionString)
    {
        var context = CreateContext(connectionString);
        return new SlipRepository(context);
    }

    /// <summary>
    /// Creates a VesselRepository instance for console applications
    /// </summary>
    /// <param name="context">SkipperContext instance</param>
    /// <returns>VesselRepository instance</returns>
    public static VesselRepository CreateVesselRepository(SkipperContext context)
    {
        return new VesselRepository(context);
    }

    /// <summary>
    /// Creates a VesselRepository instance with connection string for console applications
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>VesselRepository instance with its own context</returns>
    public static VesselRepository CreateVesselRepository(string connectionString)
    {
        var context = CreateContext(connectionString);
        return new VesselRepository(context);
    }

    /// <summary>
    /// Creates a RentalAgreementRepository instance for console applications
    /// </summary>
    /// <param name="context">SkipperContext instance</param>
    /// <returns>RentalAgreementRepository instance</returns>
    public static RentalAgreementRepository CreateRentalAgreementRepository(SkipperContext context)
    {
        return new RentalAgreementRepository(context);
    }

    /// <summary>
    /// Creates a RentalAgreementRepository instance with connection string for console applications
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>RentalAgreementRepository instance with its own context</returns>
    public static RentalAgreementRepository CreateRentalAgreementRepository(string connectionString)
    {
        var context = CreateContext(connectionString);
        return new RentalAgreementRepository(context);
    }

    /// <summary>
    /// Creates a SlipClassificationRepository instance for console applications
    /// </summary>
    /// <param name="context">SkipperContext instance</param>
    /// <returns>SlipClassificationRepository instance</returns>
    public static SlipClassificationRepository CreateSlipClassificationRepository(SkipperContext context)
    {
        return new SlipClassificationRepository(context);
    }

    /// <summary>
    /// Creates a SlipClassificationRepository instance with connection string for console applications
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>SlipClassificationRepository instance with its own context</returns>
    public static SlipClassificationRepository CreateSlipClassificationRepository(string connectionString)
    {
        var context = CreateContext(connectionString);
        return new SlipClassificationRepository(context);
    }

    /// <summary>
    /// Creates a generic Repository instance for console applications
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    /// <param name="context">SkipperContext instance</param>
    /// <returns>Repository instance</returns>
    public static IRepository<T> CreateRepository<T>(SkipperContext context) where T : BaseEntity
    {
        return new Repository<T>(context);
    }

    /// <summary>
    /// Creates a generic Repository instance with connection string for console applications
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Repository instance with its own context</returns>
    public static IRepository<T> CreateRepository<T>(string connectionString) where T : BaseEntity
    {
        var context = CreateContext(connectionString);
        return new Repository<T>(context);
    }

    /// <summary>
    /// Creates all repositories sharing the same context for console applications
    /// This is more efficient when you need multiple repositories in the same unit of work
    /// </summary>
    /// <param name="context">Shared SkipperContext instance</param>
    /// <returns>Tuple containing all repository instances</returns>
    public static (SlipRepository SlipRepo, VesselRepository VesselRepo, 
                   RentalAgreementRepository RentalRepo, SlipClassificationRepository ClassificationRepo) 
        CreateAllRepositories(SkipperContext context)
    {
        return (
            new SlipRepository(context),
            new VesselRepository(context),
            new RentalAgreementRepository(context),
            new SlipClassificationRepository(context)
        );
    }

    /// <summary>
    /// Creates all repositories with shared context using connection string for console applications
    /// This is more efficient when you need multiple repositories in the same unit of work
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Tuple containing all repository instances sharing the same context</returns>
    public static (SlipRepository SlipRepo, VesselRepository VesselRepo, 
                   RentalAgreementRepository RentalRepo, SlipClassificationRepository ClassificationRepo) 
        CreateAllRepositories(string connectionString)
    {
        var context = CreateContext(connectionString);
        return CreateAllRepositories(context);
    }
} 