using System.Reflection;
using Ecommerce.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Shared.Database;

// AppDbContext represents the active session with the database.
public class AppDbContext : DbContext
{
    // Holds the collection of module infrastructure assemblies injected via Dependency Injection.
    // Enables dynamic registration of Fluent API configurations (IEntityTypeConfiguration) 
    // without coupling the Shared project directly to feature domain/infrastructure assemblies.
    private readonly IEnumerable<Assembly> _moduleAssemblies;

    // Primary constructor.
    // Receives configuration options (such as connection string or database provider defined in Program.cs)
    // and the collection of registered module assemblies.
    // The .NET Dependency Injection container automatically instantiates this on every HTTP request.
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IEnumerable<Assembly> moduleAssemblies) : base(options)
    {
        _moduleAssemblies = moduleAssemblies;
    }

    // Model building method using Fluent API.
    // Equivalent to JPA annotations (@Table, @Column, @ManyToOne) or Hibernate XML mappings.
    // Executes once when EF Core initializes the application to map C# classes to SQL tables.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Calls the base Entity Framework Core configuration logic
        base.OnModelCreating(modelBuilder);

        // NOTE: SQLite does not support schemas (unlike PostgreSQL or SQL Server).
        // If migrating to PostgreSQL/SQL Server later, you can use: modelBuilder.HasDefaultSchema("ecommerce");
        // Omitted here to prevent runtime conflicts with SQLite.

        // 1. Automatically scans and applies all 'IEntityTypeConfiguration' implementations
        // within this assembly (Shared project).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // 2. Dynamically scans and applies all 'IEntityTypeConfiguration' implementations
        // from each external module assembly injected via DI (Product, Order, etc.).
        // Prevents cluttering this file with explicit references to all feature modules.
        foreach (var assembly in _moduleAssemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }

    // Overrides EF Core's default asynchronous SaveChanges execution.
    // Intercepts the persistence pipeline before SQL commands are generated and sent to the database.
    // This allows centralized, automated management of auditing metadata (CreatedAt and UpdatedAt)
    // across all entities implementing IAuditableEntity, avoiding manual setting in service layers or repositories.
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Thanks to the IAuditableEntity interface abstraction, this single call intercepts all entities 
        // regardless of whether their primary key (TKey) is int, long, Guid, or string.
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            // EntityState.Added represents new entities about to be inserted into the database.
            if (entry.State == EntityState.Added)
            {
                // Guarantees CreatedAt is always set using UTC time upon initial creation.
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            // EntityState.Modified represents existing entities with updated properties.
            else if (entry.State == EntityState.Modified)
            {
                // Automatically updates the timestamp whenever EF Core detects changes on an existing record.
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            // Intercepts physical deletion attempts to convert them into logical soft deletes.
            else if (entry.State == EntityState.Deleted)
            {
                // Changes the EF Core tracking state from 'Deleted' to 'Modified' 
                // to execute an UPDATE instead of SQL DELETE,
                entry.State = EntityState.Modified;

                // setting the logical deletion flag to true and recording the update timestamp in UTC.
                entry.Entity.IsDeleted = true;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Delegates the actual SQL command generation and execution to EF Core's base implementation.
        return base.SaveChangesAsync(cancellationToken);
    }
}