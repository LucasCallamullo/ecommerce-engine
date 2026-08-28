using System.Reflection;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Products.Application;
using Ecommerce.Shared.Database;
using Ecommerce.Shared.Exceptions;
using Ecommerce.Shared.Responses;
using Ecommerce.Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// MODULE REGISTRATIONS (Add new module references here)
// =============================================================================

// 1. API Assemblies: Controllers discovery per module via AssemblyReference markers
Assembly[] apiAssemblies =
[
    typeof(Ecommerce.Products.API.AssemblyReference).Assembly,
    // Future API module assemblies (e.g., typeof(Ecommerce.Order.API.AssemblyReference).Assembly)
];

// 2. Application Services: Dependency Injection extensions per module
builder.Services.AddProductModule();
// Future application module extensions (e.g., builder.Services.AddOrderModule();)

// 3. Infrastructure Assemblies: EF Core configurations (IEntityTypeConfiguration) per module
Assembly[] moduleAssemblies =
[
    typeof(Ecommerce.Products.Infrastructure.AssemblyReference).Assembly,
    // Future infrastructure module assemblies (e.g., typeof(Ecommerce.Order.Infrastructure.AssemblyReference).Assembly)
];

// =============================================================================
// FRAMEWORK & INFRASTRUCTURE CONFIGURATION
// =============================================================================

// Registers the custom GlobalExceptionHandler into the DI container
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Registers RFC 7807 Problem Details support in DI
builder.Services.AddProblemDetails();

// Registers MVC controllers and dynamically attaches module API assemblies via Application Parts
var mvcBuilder = builder.Services.AddControllers();
foreach (var assembly in apiAssemblies)
{
    mvcBuilder.AddApplicationPart(assembly);
}

// Fadd filter responses
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});

// Registers OpenAPI metadata generator in DI container
builder.Services.AddOpenApi();

// Registers module infrastructure assemblies so AppDbContext can load Fluent API mappings dynamically
builder.Services.AddSingleton<IEnumerable<Assembly>>(moduleAssemblies);

// Registers AppDbContext in DI container with SQLite using connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention()); // Converts PascalCase to snake_case automatically

// Builds the underlying ASP.NET Core web host application instance
var app = builder.Build();

// =============================================================================
// HTTP REQUEST PIPELINE MIDDLEWARES (ORDER MATTERS!)
// =============================================================================

// Register the middleware as high up as possible to measure total time
app.UseMiddleware<RequestLoggingMiddleware>();

// Enforces global exception handling middleware at the top of the HTTP pipeline
app.UseExceptionHandler();
// app.UseHttpsRedirection();
// app.UseAuthorization();

// Handling 404 responses for non-existent endpoints
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

// Automatically applies pending EF Core migrations on application startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Enables the OpenAPI JSON endpoint only when running in local 'Development' mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Maps controller action endpoints (e.g., /api/products) discovered via Application Parts
app.MapControllers();

// Maps a minimal API HTTP GET endpoint at the root path ("/")
app.MapGet("/", () => "API Its OK");

// Starts the web server and listens for incoming HTTP requests
app.Run();