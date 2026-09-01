using Ecommerce.Auth.Application.DTOs.Request;
using Ecommerce.Auth.Application.DTOs.Response;
using Ecommerce.Auth.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Auth.Api.Controllers;

/// <summary>
/// Exposes HTTP endpoints for user authentication and account registration.
/// </summary>
[ApiController] 
[Route("api/v1/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Authenticates a user with credentials and issues access and refresh tokens.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        // 1. Delegate credential validation and token generation to the domain service
        var response = await authService.LoginAsync(request, ct);

        // 2. Return HTTP 200 OK with access and refresh tokens
        return Ok(response);
    }

    /// <summary>
    /// Registers a new user account and returns the initial authentication tokens.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        // 1. Execute account creation and token generation logic
        var response = await authService.RegisterAsync(request, ct);

        // 2. Return HTTP 201 Created pointing to the Login action route
        return CreatedAtAction(nameof(Login), response);
    }
}