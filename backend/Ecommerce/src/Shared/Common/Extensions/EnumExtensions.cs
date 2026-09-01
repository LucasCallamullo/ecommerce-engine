namespace Ecommerce.Shared.Common.Extensions;

using System.Reflection;
using System.ComponentModel;


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