namespace Ecommerce.Shared.Auth.Interfaces;

/// <summary>
/// Contract for accessing the current authenticated user's context extracted from the JWT bearer token.
/// Provides a decoupled mechanism for modules to query user identity without direct HttpContext dependencies.
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