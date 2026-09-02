namespace Ecommerce.Users.Infrastructure;

using Ecommerce.Shared.Database;
using Ecommerce.Users.Infrastructure.Seeding;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services)
    {
        // Registra el Seeder para que Program.cs lo resuelva vía IDbSeeder
        services.AddScoped<IDbSeeder, DbSeeder>();

        return services;
    }
}