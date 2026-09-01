namespace Ecommerce.Shared.Auth.Interfaces;

/// <summary>
/// Provides cryptographic password hashing and verification services.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password using a secure algorithm (e.g., BCrypt).
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>The computed salted hash string.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plain-text password against a stored cryptographic hash.
    /// </summary>
    /// <param name="password">The plain-text password input.</param>
    /// <param name="passwordHash">The previously hashed password from storage.</param>
    /// <returns><c>true</c> if the password matches the hash; otherwise, <c>false</c>.</returns>
    bool VerifyPassword(string password, string passwordHash);
}