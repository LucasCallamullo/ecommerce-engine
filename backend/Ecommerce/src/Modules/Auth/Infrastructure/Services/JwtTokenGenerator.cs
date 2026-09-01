using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ecommerce.Shared.Auth.Configurations;
using Ecommerce.Shared.Auth.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Auth.Infrastructure.Services;

/// <summary>
/// Service responsible for generating JWT access tokens and cryptographically random refresh tokens
/// using configured security settings.
/// </summary>
public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public (string AccessToken, string RefreshToken, DateTime ExpiresAt) GenerateTokens(
        Guid userId, 
        string email, 
        IEnumerable<string> roles)
    {
        // 1. Initialize the token handler and retrieve the secret key from configuration
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        // 2. Calculate the exact UTC expiration timestamp based on configured expiry minutes
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        // 3. Define the base set of standard JWT identity claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 4. Map user security roles into standard ClaimTypes.Role claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // 5. Construct the token descriptor containing identity, issuer, audience, lifetime, and signing key
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        // 6. Create and serialize the JWT Access Token into string format
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // 7. Generate a secure, opaque Refresh Token
        var refreshToken = GenerateRefreshToken();

        // 8. Return the token metadata payload
        return (accessToken, refreshToken, expiresAt);
    }

    /// <summary>
    /// Generates a cryptographically secure random byte array encoded as a Base64 string.
    /// </summary>
    private static string GenerateRefreshToken()
    {
        // 1. Allocate a buffer of 64 bytes (512 bits) for high entropy
        var randomNumber = new byte[64];

        // 2. Fill the buffer with cryptographically strong random bytes using the system CSP
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        // 3. Convert byte array to a URL-safe Base64 string representation
        return Convert.ToBase64String(randomNumber);
    }
}