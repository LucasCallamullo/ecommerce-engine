namespace Ecommerce.Products.Infrastructure.Seeding;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Database;

/// <summary>
/// Seeds the catalog with initial products, categories, and brands using the bulk import pipeline.
/// Provides explicit logging and runtime exceptions for missing seed files or execution failures.
/// </summary>
public class ProductSeeder(
    IProductImportService importService, 
    ILogger<ProductSeeder> logger) : IDbSeeder
{
    private readonly IProductImportService _importService = importService;
    private readonly ILogger<ProductSeeder> _logger = logger;

    /// <inheritdoc/>
    public async Task SeedAsync(AppDbContext dbContext)
    {
        _logger.LogInformation("--> Executing ProductSeeder...");

        // 1. Check if products already exist
        var hasProducts = await dbContext.Set<Product>().AnyAsync();
        if (hasProducts)
        {
            _logger.LogWarning("--> ProductSeeder skipped: Database already contains existing products.");
            return;
        }

        // 2. Locate the seed spreadsheet relative to the execution binary
        var seedFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData.xlsx");
        _logger.LogInformation("--> Looking for seed file at path: {Path}", seedFilePath);

        if (!File.Exists(seedFilePath))
        {
            // Throw an explicit exception to fail fast if the file is missing from output directory
            throw new FileNotFoundException(
                $"Seed file not found at '{seedFilePath}'. Ensure 'SeedData.xlsx' exists in Data/ and its .csproj has <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>.");
        }

        // 3. Execute bulk import through the Application Service
        _logger.LogInformation("--> Starting bulk product import from seed file...");
        await using var stream = File.OpenRead(seedFilePath);
        var result = await _importService.ImportFromExcelAsync(stream);

        if (result.FailedCount > 0)
        {
            _logger.LogError("--> ProductSeeder completed with {ErrorCount} errors: {Errors}", 
                result.FailedCount, string.Join(" | ", result.Errors));
        }

        _logger.LogInformation("--> ProductSeeder finished successfully: {SuccessCount} products created.", 
            result.SuccessfulCount);
    }
}