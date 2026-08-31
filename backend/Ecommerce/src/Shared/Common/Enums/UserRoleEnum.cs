using System.ComponentModel;
using System.Reflection;

namespace Ecommerce.Shared.Common.Enums;

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

/// <summary>
/// Provides extension methods for enum types.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Retrieves the text stored in the <see cref="DescriptionAttribute"/> of an enum value.
    /// Returns the string representation of the enum if no attribute is found.
    /// </summary>
    /// <param name="value">The enum value to inspect.</param>
    /// <returns>The localized description string or the enum name as fallback.</returns>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}