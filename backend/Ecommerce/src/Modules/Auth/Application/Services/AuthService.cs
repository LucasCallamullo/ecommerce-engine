namespace Ecommerce.Auth.Application.Services;

using Mapster;
using System.Net;

using Ecommerce.Auth.Application.DTOs.Request;
using Ecommerce.Auth.Application.DTOs.Response;
using Ecommerce.Auth.Application.Interfaces;

// Other Modules
using Ecommerce.Users.Contracts.DTOs;
using Ecommerce.Users.Contracts.Interfaces;

using Ecommerce.Shared.Auth.Interfaces;
using Ecommerce.Shared.Exceptions;

/// <summary>
/// Handles core authentication workflows including user registration, login verification,
/// password validation, and JWT token issuance.
/// </summary>
public class AuthService(
    IUserContract userContract,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Step 1: Retrieve security profile and user credentials via the Users module contract
        var user = await userContract.GetAuthDetailsByEmailAsync(request.Email, cancellationToken);

        if (user is null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new AppException("Invalid email or password.", HttpStatusCode.Unauthorized);

        if (!user.IsActive)
            throw new AppException("User account is inactive. Please contact support.", HttpStatusCode.Forbidden);

        // Step 2: Generate access and refresh tokens for the authenticated identity
        var (accessToken, refreshToken, expiresAt) = jwtTokenGenerator.GenerateTokens(user.Id, user.Email, user.Roles);

        // Step 3: Map identity profile (including Cellphone/Dni) and security tokens into the response payload
        return new AuthResponse(
            user.Adapt<UserAuthResponse>(),
            accessToken,
            refreshToken,
            expiresAt
        );
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Step 1: Securely hash the raw user password within the Auth application layer
        var passwordHash = passwordHasher.HashPassword(request.Password);

        // Step 2: Delegate account creation and default role assignment to the Users module
        var createUserDto = new CreateUserDto(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            request.Cellphone,
            request.Dni
        );

        // Contract handles duplicate email check and database persistence
        var newUser = await userContract.CreateUserAsync(createUserDto, cancellationToken);

        // Step 3: Issue authentication tokens for the newly provisioned user account
        var (accessToken, refreshToken, expiresAt) = jwtTokenGenerator.GenerateTokens(newUser.Id, newUser.Email, newUser.Roles);

        // Step 4: Map identity snapshot alongside JWT bearer credentials
        return new AuthResponse(
            newUser.Adapt<UserAuthResponse>(),
            accessToken,
            refreshToken,
            expiresAt
        );
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        // Step 1: Validate and decode the incoming refresh token to extract user identity
        var userId = jwtTokenGenerator.ValidateRefreshToken(request.RefreshToken);

        if (userId == Guid.Empty)
            throw new AppException("Invalid or expired refresh token.", HttpStatusCode.Unauthorized);

        // Step 2: Fetch the current user security details to verify status and active roles
        var user = await userContract.GetAuthDetailsByIdAsync(userId, cancellationToken);

        if (user is null || !user.IsActive)
            throw new AppException("User account is inactive or no longer exists.", HttpStatusCode.Unauthorized);

        // Step 3: Rotate tokens - issue a fresh Access Token and a new Refresh Token
        var (newAccessToken, newRefreshToken, expiresAt) = jwtTokenGenerator.GenerateTokens(user.Id, user.Email, user.Roles);

        // Optional: Invalidate old refresh token if persisting tokens in DB / Redis cache

        // Step 4: Return updated token pair alongside profile snapshot
        return new AuthResponse(
            user.Adapt<UserAuthResponse>(),
            newAccessToken,
            newRefreshToken,
            expiresAt
        );
    }

    public async Task<UserProfileResponse> GetUserProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Step 1: Parse string representation into Guid
        if (!Guid.TryParse(userId, out var parsedUserId))
            throw new AppException("Invalid user identifier format.", HttpStatusCode.BadRequest);

        // Step 2: Retrieve detailed profile entity through cross-module contract
        var userProfile = await userContract.GetUserProfileByIdAsync(parsedUserId, cancellationToken);

        if (userProfile is null)
            throw new AppException("User profile not found.", HttpStatusCode.NotFound);

        // Step 3: Map user details to the full profile response DTO
        return userProfile.Adapt<UserProfileResponse>();
    }

    public async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        // Revoke the refresh token from persistent store/cache if stored server-side.
        // If using stateless JWT refresh tokens, validation fails automatically upon expiration.
        await Task.CompletedTask;
    }
}