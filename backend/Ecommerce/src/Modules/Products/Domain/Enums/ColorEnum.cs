namespace Ecommerce.Products.Domain.Enums;

using System;
using System.Collections.Generic;
using Ecommerce.Shared.Common.Extensions;

/// <summary>
/// Represents normalized master catalog base colors used for faceted search filtering and UI categorization.
/// </summary>
public enum ColorEnum
{
    Black,
    White,
    Grey,
    Red,
    Blue,
    Green,
    Yellow,
    Orange,
    Purple,
    Violet,
    Pink,
    Brown,
    Beige,
    Gold,
    Silver
}


/// <summary>
/// Provides extension methods for parsing, mapping, and resolving fallback visual attributes for <see cref="BaseColor"/>.
/// </summary>
public static class ColorExtensions
{
    private static readonly Dictionary<string, ColorEnum> ColorNameMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // Neutros / Básicos
        { "negro", ColorEnum.Black },
        { "black", ColorEnum.Black },

        { "blanco", ColorEnum.White },
        { "white", ColorEnum.White },

        { "gris", ColorEnum.Grey },
        { "gray", ColorEnum.Grey },
        { "grey", ColorEnum.Grey },

        // Primarios y Secundarios
        { "rojo", ColorEnum.Red },
        { "red", ColorEnum.Red },

        { "azul", ColorEnum.Blue },
        { "blue", ColorEnum.Blue },

        { "verde", ColorEnum.Green },
        { "green", ColorEnum.Green },

        { "amarillo", ColorEnum.Yellow },
        { "yellow", ColorEnum.Yellow },

        { "naranja", ColorEnum.Orange },
        { "orange", ColorEnum.Orange },

        { "morado", ColorEnum.Purple },
        { "purple", ColorEnum.Purple },

        { "violeta", ColorEnum.Violet },

        { "rosado", ColorEnum.Pink },
        { "rosa", ColorEnum.Pink },
        { "pink", ColorEnum.Pink },

        { "marron", ColorEnum.Brown },
        { "brown", ColorEnum.Brown },
        
        { "beige", ColorEnum.Beige },

        // Metallics / Special
        { "dorado", ColorEnum.Gold },
        { "gold", ColorEnum.Gold },
        
        { "plateado", ColorEnum.Silver },
        { "silver", ColorEnum.Silver }
    };

    /// <summary>
    /// Attempts to parse a raw string input (Spanish or English) into a normalized <see cref="BaseColor"/> enum.
    /// </summary>
    /// <param name="colorInput">The raw color string from user input or Excel import.</param>
    /// <returns>The matching <see cref="BaseColor"/> enum, or <c>null</c> if unrecognized.</returns>
    public static ColorEnum? ToBaseColor(this string? colorInput)
    {
        if (string.IsNullOrWhiteSpace(colorInput))
            return null;

        // parse acents
        var key = StringExtensions.ToSlug(colorInput.Trim());
        return ColorNameMap.TryGetValue(key, out var baseColor) ? baseColor : null;
    }

    /// <summary>
    /// Resolves the default HEX color swatch code for a normalized <see cref="BaseColor"/>.
    /// </summary>
    public static string ToDefaultHex(this ColorEnum color) => color switch
    {
        ColorEnum.Black  => "#000000",
        ColorEnum.White  => "#FFFFFF",
        ColorEnum.Grey   => "#808080",
        ColorEnum.Red    => "#FF0000",
        ColorEnum.Blue   => "#0000FF",
        ColorEnum.Green  => "#008000",
        ColorEnum.Yellow => "#FFFF00",
        ColorEnum.Orange => "#FFA500",
        ColorEnum.Purple => "#800080",
        ColorEnum.Violet => "#8A2BE2",
        ColorEnum.Pink   => "#FFC0CB",
        ColorEnum.Brown  => "#A52A2A",
        ColorEnum.Beige  => "#F5F5DC",
        ColorEnum.Gold   => "#FFD700",
        ColorEnum.Silver => "#C0C0C0",
        _                => "#CCCCCC"
    };

    /// <summary>
    /// Resolves a HEX color code from a raw string, attempting enum mapping first or returning null if invalid.
    /// </summary>
    public static string? ResolveHexColor(string? colorName)
    {
        var baseColor = colorName.ToBaseColor();
        return baseColor?.ToDefaultHex();
    }
}