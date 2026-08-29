using Ecommerce.Products.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Products.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Product module application services into the central Dependency Injection container.
    /// </summary>
    public static IServiceCollection AddProductModule(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IVariantService, VariantService>();

        return services;
    }
}