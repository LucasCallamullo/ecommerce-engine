
using System.Reflection;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Shared.Database;
using Ecommerce.Shared.Exceptions;
using Ecommerce.Shared.Responses;
using Ecommerce.Shared.Middlewares;
using Ecommerce.Shared.Auth.Extensions;
using Ecommerce.Shared.API;
using Ecommerce.Shared.Filters;

using Ecommerce.Auth.Application;
using Ecommerce.Users.Application;
using Ecommerce.Users.Infrastructure;

using Ecommerce.Products.Application;
using Ecommerce.Products.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// MODULE REGISTRATIONS (Add new module references here)
// =============================================================================

// 1. API Assemblies: Controllers discovery per module via AssemblyReference markers
Assembly[] apiAssemblies =
[
    typeof(Ecommerce.Users.API.AssemblyReference).Assembly,
    typeof(Ecommerce.Auth.API.AssemblyReference).Assembly,
    typeof(Ecommerce.Products.API.AssemblyReference).Assembly,
    // Future API module assemblies
];

// 2. Application Services: Dependency Injection extensions per module
builder.Services.AddUsersModule();
builder.Services.AddAuthModule();
builder.Services.AddProductModule();
// Future application module extensions

// 3. Infrastructure Assemblies: EF Core configurations (IEntityTypeConfiguration) per module
Assembly[] moduleAssemblies =
[
    typeof(Ecommerce.Users.Infrastructure.AssemblyReference).Assembly,
    typeof(Ecommerce.Auth.Infrastructure.AssemblyReference).Assembly,
    typeof(Ecommerce.Products.Infrastructure.AssemblyReference).Assembly,
    // Future infrastructure module assemblies
];

//! Add services from Infrastructure/DependencyInjection.cs only for database seeding
builder.Services.AddUsersInfrastructure();
builder.Services.AddProductsInfrastructure();

// Register custom CORS policy
builder.Services.AddCustomCors(builder.Configuration);

// =============================================================================
// FRAMEWORK & INFRASTRUCTURE CONFIGURATION
// =============================================================================

// Registers JWT Authentication and Authorization infrastructure
// * Register ICurrentUserProvider stuff
builder.Services.AddJwtAuthentication(builder.Configuration);

// ! Registers the custom GlobalExceptionHandler into the DI container
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Registers RFC 7807 Problem Details support in DI
builder.Services.AddProblemDetails();

// Registers MVC controllers and dynamically attaches module API assemblies via Application Parts
// Single unified MVC Controllers configuration (Application Parts + Global Filters)
var mvcBuilder = builder.Services.AddControllers(options =>
{
    // * Executes FluentValidation rules automatically before action execution
    options.Filters.Add<FluentValidationFilter>();

    // * Standardizes successful HTTP response payloads across all controllers
    options.Filters.Add<ApiResponseFilter>();
});

// Dynamically attaches module API assemblies to discover controllers
foreach (var assembly in apiAssemblies)
{
    mvcBuilder.AddApplicationPart(assembly);
}

// Registers OpenAPI metadata generator in DI container
builder.Services.AddOpenApi();

// Registers module infrastructure assemblies so AppDbContext can load Fluent API mappings dynamically
builder.Services.AddSingleton<IEnumerable<Assembly>>(moduleAssemblies);

// ! Registers AppDbContext in DI container with SQLite using connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention()); // Converts PascalCase to snake_case automatically

// Builds the underlying ASP.NET Core web host application instance
var app = builder.Build();

// =============================================================================
// HTTP REQUEST PIPELINE MIDDLEWARES (ORDER MATTERS!)
// =============================================================================

// 1. Enforces global exception handling middleware at the top of the HTTP pipeline
app.UseExceptionHandler();
// app.UseHttpsRedirection();

// 2. Register logging as early as possible to measure request execution time
app.UseMiddleware<RequestLoggingMiddleware>();

// 3. Enable CORS policy before Authentication and Routing execution
// MUST BE PLACED AFTER UseRouting AND BEFORE UseAuthentication / UseAuthorization
app.UseCors("DefaultCors");

// 4. Enables JWT identity extraction (Must be placed before UseAuthorization)
app.UseAuthentication();

// 5. Enables policy/role enforcement on endpoints
app.UseAuthorization();

// 6. Handling 404 responses for non-existent endpoints
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
    {
        context.HttpContext.Response.ContentType = "application/json";
        var path = context.HttpContext.Request.Path.Value ?? string.Empty;
        var errorDto = new ErrorResponseDto(404, "The requested resource or endpoint was not found.", null, path);
        
        await context.HttpContext.Response.WriteAsJsonAsync(errorDto);
    }
});

// =============================================================================
//! DATABASE MIGRATIONS & DATA SEEDING AT STARTUP
// =============================================================================
// Automatically applies pending EF Core migrations on application startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Step 1: Apply any pending database schema migrations.
    await dbContext.Database.MigrateAsync();

    // Step 2: Run all registered module seeders exclusively in local development environments.
    // if (app.Environment.IsDevelopment() || !app.Environment.IsDevelopment())
    // {
    // Resolves all IDbSeeder implementations registered by individual modules.
    var seeders = scope.ServiceProvider.GetServices<IDbSeeder>();

    foreach (var seeder in seeders)
    {
        await seeder.SeedAsync(dbContext);
    }
    // }
}

// Enables the OpenAPI JSON endpoint only when running in local 'Development' mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Maps controller action endpoints (e.g., /api/products) discovered via Application Parts
app.MapControllers();

// Maps a minimal API HTTP GET endpoint at the root path ("/")
// app.MapGet("/", () => "API Its OK");

// Starts the web server and listens for incoming HTTP requests
app.Run();