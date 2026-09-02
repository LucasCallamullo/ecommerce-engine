namespace Ecommerce.Shared.Auth.Enums;

using System.ComponentModel;

/// <summary>
/// Defines the system-wide roles for user authorization.
/// Designed to be used with the <c>[Authorize(Roles = nameof(UserRoleEnum.Admin))]</c> attribute 
/// across application boundaries to avoid magic strings.
/// </summary>
public enum UserRoleEnum
{
    /// <summary>System administrator with full platform control and access.</summary>
    [Description("Administrator with full platform access")]
    Admin = 1,

    /// <summary>Standard end-user with purchasing privileges.</summary>
    [Description("Standard e-commerce customer")]
    Customer = 2,

    /// <summary>Merchant authorized to manage product catalog and store sales.</summary>
    [Description("E-commerce merchant allowed to manage products and sales")]
    Seller = 3,

    /// <summary>Support representative with customer assistance capabilities.</summary>
    [Description("Customer support representative")]
    Support = 4
}