namespace Ecommerce.Users.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Ecommerce.Shared.Responses;
using Ecommerce.Shared.Auth.Constants;
using Ecommerce.Shared.API;

using Ecommerce.Users.Application.DTOs.Request;
using Ecommerce.Users.Application.DTOs.Response;
using Ecommerce.Users.Application.Interfaces;

/// <summary>
/// Exposes administrative and self-service HTTP endpoints for user account management.
/// </summary>
[Route("api/v1/users")]
public class UsersController(IUserService userService) : ApiControllerBase
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Updates personal profile details for the currently authenticated user.
    /// </summary>
    /// <param name="dto">The payload containing updated personal information fields.</param>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> UpdateMyProfile(
        [FromBody] UpdateUserProfile dto, 
        CancellationToken cancellationToken)
    {
        var updatedUser = await _userService.UpdateProfileAsync(dto, cancellationToken);
        return Ok(updatedUser);
    }

    /// <summary>
    /// Retrieves a specific user profile by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the target user.</param>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = UserRoles.AdminOrSeller)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(
        Guid id, 
        CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(user);
    }

    /// <summary>
    /// Retrieves a paginated and filtered collection of user accounts. Restrict access to administrative staff.
    /// </summary>
    /// <param name="filterParams">The query parameters controlling search terms and pagination offsets.</param>
    [HttpGet]
    [Authorize(Roles = UserRoles.AdminOrSeller)]
    [ProducesResponseType(typeof(PagedResultDto<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResultDto<UserResponse>>> GetAll(
        [FromQuery] UserFilterParams filterParams, 
        CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(filterParams, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Updates assigned security roles for a target user account. Restricted to system administrators.
    /// </summary>
    /// <param name="dto">The administrative payload containing the target email and new role assignments.</param>
    [HttpPut("roles")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> UpdateUserRole(
        [FromBody] UpdateUserRol dto, 
        CancellationToken cancellationToken)
    {
        var updatedUser = await _userService.UpdateUserRoleAsync(dto, cancellationToken);
        return Ok(updatedUser);
    }
}