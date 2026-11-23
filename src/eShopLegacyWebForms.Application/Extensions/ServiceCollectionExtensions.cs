using AutoMapper;
using eShopLegacyWebForms.Application.Mappings;
using eShopLegacyWebForms.Application.Services;
using eShopLegacyWebForms.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShopLegacyWebForms.Application.Extensions;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Application layer services to the DI container.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register AutoMapper manually (without AutoMapper.Extensions.Microsoft.DependencyInjection)
        services.AddSingleton<IMapper>(sp =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        });

        // Register services based on configuration
        var useMockDataStr = configuration["UseMockData"];
        var useMockData = string.IsNullOrEmpty(useMockDataStr) || bool.Parse(useMockDataStr);

        if (useMockData)
        {
            services.AddSingleton<ICatalogService, CatalogServiceMock>();
        }
        else
        {
            services.AddScoped<ICatalogService, CatalogService>();
        }

        return services;
    }
}
