namespace Ecommerce.Products.Infrastructure;

/// <summary>
/// Static marker class used to provide type-safe reflection references 
/// to the <c>Ecommerce.Products.Infrastructure</c> assembly.
/// </summary>
/// <remarks>
/// This reference is primarily used during application startup (e.g., in <c>Program.cs</c> 
/// or <c>AppDbContextFactory</c>) to dynamically scan and register Entity Framework Core entity configurations
/// without creating tight coupling to specific domain entity classes.
/// </remarks>
public static class AssemblyReference
{
}