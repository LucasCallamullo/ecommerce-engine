using System.Security.Claims;
using Ecommerce.Shared.Auth.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Shared.Auth.Services;

/// <summary>
/// Resolves current user context properties from the active HTTP request's ClaimsPrincipal.
/// Implements primary primary constructor pattern for dependency injection of IHttpContextAccessor.
/// </summary>
public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    /// <summary>
    /// Extracts and parses the user ID (NameIdentifier claim) from the authenticated HttpContext.
    /// Returns null if the request is unauthenticated or the claim is not a valid Guid.
    /// </summary>
    public Guid? UserId
    {
        get
        {
            var idClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idClaim, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// Resolves the email claim from the current user's token context.
    /// </summary>
    public string? Email => httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

    /// <summary>
    /// Indicates whether the incoming HTTP request is authenticated with a valid token.
    /// </summary>
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}