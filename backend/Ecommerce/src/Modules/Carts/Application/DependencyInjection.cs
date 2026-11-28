using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using Mapster;

using Ecommerce.Carts.Application.Services.Internals;
using Ecommerce.Carts.Application.Interfaces;

namespace Ecommerce.Carts.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCartsModule(this IServiceCollection services)
    {
        // 1. Application Services Registration
        services.AddScoped<ICartService, CartService>();

        // 2. Automatic scanning of ALL AbstractValidators in the Application assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // 3. Automatic scanning of ALL Mapster IRegister configurations in the Application assembly
        // var config = TypeAdapterConfig.GlobalSettings;
        // config.Scan(Assembly.GetExecutingAssembly());

        return services;
    }
}
