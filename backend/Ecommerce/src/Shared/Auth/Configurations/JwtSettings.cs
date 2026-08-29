namespace Ecommerce.Shared.Auth.Configurations;

/// <summary>Represents strongly-typed configuration settings for JWT authentication.</summary>
public class JwtSettings
{
    /// <summary>Configuration section key in appsettings.json.</summary>
    public const string SectionName = "JwtSettings";

    /// <summary>Secret key used to sign and verify JWT tokens (min 32 chars).</summary>
    public string Secret { get; init; } = string.Empty;

    /// <summary>Identifies the principal that issued the token.</summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>Identifies the intended recipients for the token.</summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>Lifespan of the generated JWT token in minutes.</summary>
    public int ExpiryMinutes { get; init; }
}