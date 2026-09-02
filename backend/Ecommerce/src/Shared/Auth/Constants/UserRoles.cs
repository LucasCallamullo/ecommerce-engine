namespace Ecommerce.Shared.Auth.Constants;

using Ecommerce.Shared.Auth.Enums;

/// <summary>
/// Provides compile-time string constants for system roles to be used across authorization attributes and policy declarations.
/// 
/// <para>
/// <b>Design Purpose:</b><br/>
/// ASP.NET Core's <c>[Authorize(Roles = ...)]</c> attribute requires compile-time constant strings. 
/// Using <see cref="nameof"/> bound to <see cref="UserRoleEnum"/> prevents magic strings and ensures 
/// refactoring safety if enum values are renamed.
/// </para>
/// 
/// <para>
/// <b>Example Usage in Controllers:</b>
/// <code>
/// [Authorize(Roles = UserRoles.Admin)]
/// 
/// [Authorize(Roles = UserRoles.AdminOrSupport)]
/// </code>
/// </para>
/// </summary>
public static class UserRoles
{
    public const string Admin = nameof(UserRoleEnum.Admin);
    public const string Customer = nameof(UserRoleEnum.Customer);
    public const string Seller = nameof(UserRoleEnum.Seller);
    public const string Support = nameof(UserRoleEnum.Support);

    // + =========================================================================
    // + COMBINED ROLE CONSTANTS (For multi-role authorization policies)
    // + =========================================================================

    /// <summary>Allows access to either System Administrators or Support Representatives.</summary>
    public const string AdminOrSupport = $"{Admin},{Support}";

    /// <summary>Allows access to merchants or system administrators.</summary>
    public const string AdminOrSeller = $"{Admin},{Seller}";
}