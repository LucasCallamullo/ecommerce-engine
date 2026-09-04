using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using Mapster;

using Ecommerce.Products.Application.Services.Internals;
using Ecommerce.Products.Application.Interfaces;

namespace Ecommerce.Products.Application;

public static class DependencyInjection
{
    /// <summary> Registers Product module application services into the central Dependency Injection container.</summary>
    public static IServiceCollection AddProductModule(this IServiceCollection services)
    {
        // 1. Application Services Registration
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IVariantService, VariantService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();

        // Register ProductImportService implementation for IProductImportService
        services.AddScoped<IProductImportService, ProductImportService>();

        // 2. Automatic scanning of ALL AbstractValidators in the Application assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // 3. Automatic scanning of ALL Mapster IRegister configurations in the Application assembly
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        return services;
    }
}