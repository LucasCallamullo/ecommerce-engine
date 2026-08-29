using Ecommerce.Products.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Ecommerce.Products.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Product module application services into the central Dependency Injection container.
    /// </summary>
    public static IServiceCollection AddProductModule(this IServiceCollection services)
    {
        // 1. Application Services Registration
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IVariantService, VariantService>();

        // 2. Automatic scanning of ALL AbstractValidators in the Application assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}