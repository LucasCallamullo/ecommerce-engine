namespace Ecommerce.Shared.Auth.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Ecommerce.Shared.Auth.Interfaces;

/// <summary>
/// Resolves current user context properties from the active HTTP request's ClaimsPrincipal.
/// 
/// <para>
/// <b>Django Analogy:</b> Equivalent to Django's <c>request.user</c> object populated by authentication middleware.
/// It reads identity claims injected by .NET JWT Bearer authentication from <see cref="HttpContext.User"/>,
/// allowing Application Layer services to query current user context without depending directly on HTTP infrastructure.
/// </para>
/// </summary>
public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    /// <summary>
    /// Extracts and parses the user ID (NameIdentifier/sub claim) from the authenticated HttpContext.
    /// Returns null if the request is unauthenticated or the claim is not a valid Guid.
    /// <para><i>Equivalent to: <c>request.user.id</c> in Django.</i></para>
    /// </summary>
    public Guid? UserId
    {
        get
        {
            var idClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

            return Guid.TryParse(idClaim, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// Resolves the email claim from the current user's token context.
    /// <para><i>Equivalent to: <c>request.user.email</c> in Django.</i></para>
    /// </summary>
    public string? Email => httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

    /// <summary>
    /// Indicates whether the incoming HTTP request is authenticated with a valid token.
    /// <para><i>Equivalent to: <c>request.user.is_authenticated</c> in Django.</i></para>
    /// </summary>
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}