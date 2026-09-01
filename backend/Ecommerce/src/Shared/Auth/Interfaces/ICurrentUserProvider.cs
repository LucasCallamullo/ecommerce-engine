namespace Ecommerce.Shared.Auth.Interfaces;

/// <summary>
/// Contract for accessing the current authenticated user's context extracted from the JWT bearer token.
/// Provides a decoupled mechanism for modules to query user identity without direct HttpContext dependencies.
/// 
/// <para>
/// <b>Dependency Inversion Principle (DIP):</b><br/>
/// Application Layer services (Domain/Use Cases) should never depend on infrastructure-specific web abstractions
/// like <c>Microsoft.AspNetCore.Http.IHttpContextAccessor</c> or <c>HttpContext</c>.
/// </para>
/// 
/// <para>
/// <b>Example Usage in Application Layer:</b>
/// <code>
/// public class CartService(ICurrentUserProvider currentUser) : ICartService
/// {
///     public async Task AddItemAsync(Guid productId, int quantity, CancellationToken ct)
///     {
///         // 1. Verify user authentication status
///         if (!currentUser.IsAuthenticated)
///             throw new UnauthorizedAccessException("User must be logged in to manage the cart.");
/// 
///         // 2. Safely obtain current user ID directly inside Application/Domain logic
///         var userId = currentUser.UserId!.Value;
/// 
///         // 3. Execute domain business logic...
///     }
/// }
/// </code>
/// </para>
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>Gets the unique identifier of the authenticated user, or null if unauthenticated.</summary>
    Guid? UserId { get; }

    /// <summary>Gets the email address of the authenticated user, or null if unauthenticated.</summary>
    string? Email { get; }

    /// <summary>Gets a value indicating whether the current request is authenticated.</summary>
    bool IsAuthenticated { get; }
}