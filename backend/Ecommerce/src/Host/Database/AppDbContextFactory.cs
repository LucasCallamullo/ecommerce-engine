using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Ecommerce.Shared.Database;
using Ecommerce.Products.Infrastructure;

namespace Ecommerce.Host.Database;

/// <summary>
/// Design-time factory for <see cref="AppDbContext"/>.
/// Used exclusively by the EF Core Command-Line Interface (CLI) tool (e.g., 'dotnet ef migrations')
/// to instantiate the database context when running migration commands outside the application runtime.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Creates a new instance of <see cref="AppDbContext"/> with explicit design-time configurations 
    /// and module infrastructure assembly registrations.
    /// </summary>
    /// <param name="args">Arguments passed by the design-time tool.</param>
    /// <returns>A fully configured instance of <see cref="AppDbContext"/> for schema design tasks.</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // SQLite provider configuration used strictly for generating design-time migrations.
        optionsBuilder
            .UseSqlite("Data Source=ecommerce_local.db")
            .UseSnakeCaseNamingConvention();        // this is for comptaible with program.cs config

        // Explicit list of feature module infrastructure assemblies containing Fluent API configurations.
        var moduleAssemblies = new List<Assembly>
        {
            typeof(AssemblyReference).Assembly
        };

        return new AppDbContext(optionsBuilder.Options, moduleAssemblies);
    }
}