using System.Reflection;
using Ecommerce.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Shared.Database;

/// <summary>
/// Represents the central database context session managed by Entity Framework Core.
/// Coordinates domain entity persistence, dynamic module assembly mapping, and centralized audit tracking.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly IEnumerable<Assembly> _moduleAssemblies;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options configuring connection strings and providers.</param>
    /// <param name="moduleAssemblies">The collection of registered feature module infrastructure assemblies injected via DI.</param>
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IEnumerable<Assembly> moduleAssemblies) : base(options)
    {
        _moduleAssemblies = moduleAssemblies;
    }

    /// <summary>
    /// Configures entity mappings and relationships using the Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the database model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Execute default base Entity Framework Core model configuration logic
        base.OnModelCreating(modelBuilder);

        // NOTE: SQLite does not support schemas (unlike PostgreSQL or SQL Server).
        // If migrating to PostgreSQL/SQL Server later, you can use: modelBuilder.HasDefaultSchema("ecommerce");
        // Omitted here to prevent runtime conflicts with SQLite.

        // 2. Automatically scan and apply entity configurations defined within the Shared assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // 3. Dynamically scan and apply entity configurations from all injected feature module assemblies
        foreach (var assembly in _moduleAssemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }

    /// <summary>
    /// Intercepts entity persistence operations prior to committing changes to the database.
    /// Automatically manages audit timestamps (<c>CreatedAt</c>, <c>UpdatedAt</c>) and converts physical deletions into soft deletes.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation returning the number of state entries written.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Retrieve all tracked entity entries implementing the IAuditableEntity contract
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            // 2. Set creation UTC timestamp for newly added entities
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            // 3. Update modification UTC timestamp when existing entity properties change
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            // 4. Intercept physical deletion requests and convert them to soft delete UPDATE operations
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        // 5. Delegate execution to base EF Core persistence engine
        return base.SaveChangesAsync(cancellationToken);
    }
}