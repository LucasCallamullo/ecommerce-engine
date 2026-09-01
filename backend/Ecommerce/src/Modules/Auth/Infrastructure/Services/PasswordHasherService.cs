namespace Ecommerce.Auth.Infrastructure.Services;

using Ecommerce.Shared.Auth.Interfaces;

/// <summary>
/// Provides password hashing and verification services utilizing the BCrypt algorithm.
/// 
/// Implements <see cref="IPasswordHasher"/> to decouple the application layer from 
/// the concrete hashing library (<c>BCrypt.Net-Next</c>), allowing easy migration 
/// to alternative algorithms (e.g., Argon2) without impacting domain logic.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password using BCrypt's salted key derivation function.
    /// </summary>
    /// <param name="password">The plain-text password string to be hashed.</param>
    /// <returns>The generated cryptographic password hash containing the salt and work factor.</returns>
    public string HashPassword(string password) 
        => BCrypt.Net.BCrypt.HashPassword(password);

    /// <summary>
    /// Verifies a plain-text password against a stored BCrypt password hash.
    /// </summary>
    /// <param name="password">The input plain-text password to verify.</param>
    /// <param name="passwordHash">The stored BCrypt hash string retrieved from database storage.</param>
    /// <returns><c>true</c> if the password matches the hash; otherwise, <c>false</c>.</returns>
    public bool VerifyPassword(string password, string passwordHash) 
        => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}