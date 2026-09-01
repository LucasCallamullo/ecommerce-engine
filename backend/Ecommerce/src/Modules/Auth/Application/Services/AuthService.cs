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
}