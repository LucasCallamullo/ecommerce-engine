using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Carts.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCartsModule(this IServiceCollection services)
    {
        // Register application services, validators, and dependency injection
        return services;
    }
}
