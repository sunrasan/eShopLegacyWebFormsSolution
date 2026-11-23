using eShopLegacyWebForms.Domain.Interfaces.Repositories;
using eShopLegacyWebForms.Infrastructure.Data;
using eShopLegacyWebForms.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShopLegacyWebForms.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering Infrastructure layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Infrastructure layer services to the DI container.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var useMockData = configuration.GetValue<bool>("UseMockData");

        if (!useMockData)
        {
            // Register EF Core DbContext
            services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("CatalogDbContext"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            // Register repositories
            services.AddScoped<ICatalogItemRepository, CatalogItemRepository>();
            services.AddScoped<ICatalogBrandRepository, CatalogBrandRepository>();
            services.AddScoped<ICatalogTypeRepository, CatalogTypeRepository>();
        }

        return services;
    }
}
