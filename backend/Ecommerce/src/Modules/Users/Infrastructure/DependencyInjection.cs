namespace Ecommerce.Users.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Ecommerce.Shared.Database;
using Ecommerce.Users.Infrastructure.Seeding;

/// <summary>
/// Provides extension methods for registering infrastructure-level dependencies and seeders for the Users module.
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Registers database seeders and infrastructure services for the Users module into the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to append registrations to.</param>
    /// <returns>The modified service collection instance for method chaining.</returns>
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services)
    {
        // Register module database seeder resolved dynamically via IDbSeeder in Program.cs
        services.AddScoped<IDbSeeder, UserRolesSeeder>();

        return services;
    }
}