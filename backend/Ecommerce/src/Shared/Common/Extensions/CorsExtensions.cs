namespace Ecommerce.Shared.Common.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for setting up Cross-Origin Resource Sharing (CORS) 
/// within the application's Dependency Injection container.
/// </summary>
public static class CorsExtensions
{
    /// <summary>
    /// Registers a custom CORS policy named "DefaultCors" to allow requests from the React frontend client.
    /// </summary>
    /// <param name="services">The service collection instance.</param>
    /// <param name="config">The application configuration properties (reserved for dynamic origin loading).</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for method chaining.</returns>
    public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration config)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCors", policy =>
            {
                // Vite / React Client Origin
                policy.WithOrigins("http://localhost:5173") 
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials(); // Enables credentials (cookies, auth headers)
            });
        });

        return services;
    }
}