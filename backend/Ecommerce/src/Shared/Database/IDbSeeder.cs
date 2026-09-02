namespace Ecommerce.Shared.Database;

/// <summary>
/// Defines a contract for executing database seeding logic across independent modules during application initialization.
/// </summary>
public interface IDbSeeder
{
    /// <summary>
    /// Executes the module-specific data seeding routine asynchronously.
    /// </summary>
    /// <param name="dbContext">The application database context used to interact with data stores.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous seeding operation.</returns>
    Task SeedAsync(AppDbContext dbContext);
}