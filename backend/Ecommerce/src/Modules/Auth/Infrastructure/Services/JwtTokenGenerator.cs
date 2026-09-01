namespace Ecommerce.Auth.Infrastructure.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Ecommerce.Shared.Auth.Configurations;
using Ecommerce.Shared.Auth.Interfaces;

/// <summary>
/// Service responsible for generating JWT access tokens and cryptographically signed refresh tokens
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
        // 1. Initialize the token handler and retrieve the signing key secret
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        // 2. Calculate the exact UTC expiration timestamp for the access token
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

        // 5. Construct the token descriptor for the short-lived Access Token
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

        // 7. Generate a signed JWT Refresh Token bound to the user ID
        var refreshToken = GenerateRefreshToken(userId);

        // 8. Return token metadata payload
        return (accessToken, refreshToken, expiresAt);
    }

    public Guid ValidateRefreshToken(string refreshToken)
    {
        // 1. Return empty Guid immediately if the provided token string is empty or whitespace
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Guid.Empty;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        try
        {
            // 2. Configure strict cryptographic and payload validation parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // 3. Validate signature, issuer, audience, and expiration against the token payload
            var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out var validatedToken);

            // 4. Ensure the validated token object is a JWT signed with HMAC-SHA256
            if (validatedToken is not JwtSecurityToken jwtToken || 
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Guid.Empty;
            }

            // 5. Verify the explicit 'token_type' claim to prevent using Access Tokens as Refresh Tokens
            var tokenType = principal.FindFirst("token_type")?.Value;
            if (tokenType != "refresh")
                return Guid.Empty;

            // 6. Extract the user subject ID claim from the validated principal
            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
                           ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 7. Parse and return the extracted user Guid, or empty Guid on failure
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
        catch
        {
            // 8. Intercept validation failures (tampered, expired, or malformed tokens) safely
            return Guid.Empty;
        }
    }

    /// <summary>
    /// Generates a signed JWT Refresh Token containing identity claims with an extended lifetime.
    /// </summary>
    private string GenerateRefreshToken(Guid userId)
    {
        // 1. Initialize the token handler and signing key
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        // 2. Define specific claims dedicated exclusively to token refresh semantics
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("token_type", "refresh"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 3. Construct descriptor with an extended lifetime (e.g., 7 days)
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        // 4. Create and serialize the signed Refresh Token
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}