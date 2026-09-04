namespace Ecommerce.Products.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Products.Infrastructure.Seeding;
using Ecommerce.Products.Infrastructure.Services;
using Ecommerce.Shared.Database;

/// <summary>
/// Provides extension methods for registering infrastructure-level services, file parsers, and seeders for the Products module.
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Registers Excel parsing utilities, catalog database seeders, and infrastructure services for the Products module into the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to append registrations to.</param>
    /// <returns>The modified service collection instance for method chaining.</returns>
    public static IServiceCollection AddProductsInfrastructure(this IServiceCollection services)
    {
        // Register MiniExcel stream parser implementation
        services.AddScoped<IProductExcelParser, ProductExcelParser>();

        // Register module database seeder resolved dynamically via IDbSeeder during startup
        services.AddScoped<IDbSeeder, ProductSeeder>();

        return services;
    }
}