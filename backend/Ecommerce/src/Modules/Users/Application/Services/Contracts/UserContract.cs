namespace Ecommerce.Users.Application.Services.Contracts;


using Mapster;
using System.Net;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Shared.Database;
using Ecommerce.Shared.Exceptions;
using Ecommerce.Shared.Auth.Enums;

using Ecommerce.Users.Contracts.DTOs;
using Ecommerce.Users.Contracts.Interfaces;
using Ecommerce.Users.Domain.Entities;

/// <summary>
/// Implementation of <see cref="IUserContract"/> providing a clean, decoupled facade 
/// for external modules (e.g., Auth) to query and provision user records directly via EF Core.
/// </summary>
public class UserContract(AppDbContext dbContext) : IUserContract
{
    // * ===================================================
    // *             Register/Crate Method
    // * ===================================================

    public async Task<UserCreatedDto> CreateUserAsync(
        CreateUserDto request, 
        CancellationToken cancellationToken = default)
    {
        // 1. Optimized query to verify email uniqueness
        var emailExists = await dbContext.Set<User>()
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        // 2. Throw HTTP 409 Conflict if the resource already exists
        if (emailExists)
            throw new AppException($"Email '{request.Email}' is already registered.", HttpStatusCode.Conflict);

        // 3. Map DTO to User entity using Mapster (Id and IsActive set via config/explicitly)
        var user = request.Adapt<User>();

        // 4. Enforce domain rule: every newly registered user starts as a "Customer"
        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = (int)UserRoleEnum.Customer
        });

        // 5. EF Core persists User and UserRole within a single transactional unit of work
        dbContext.Set<User>().Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        // 6. Map created Entity directly to response DTO (Mapster handles RoleId resolution)
        return user.Adapt<UserCreatedDto>();
    }

    // * ===================================================
    // *             GET METHODS
    // * ===================================================

    public async Task<UserAuthDetailsDto?> GetAuthDetailsByEmailAsync(
        string email, 
        CancellationToken cancellationToken = default)
    {
        // 1. Direct and lightweight query projecting directly to DTO via Mapster (SQL SELECT optimized)
        return await dbContext.Set<User>()
            .AsNoTracking()
            .Where(u => u.Email == email && !u.IsDeleted)
            .ProjectToType<UserAuthDetailsDto>()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserAuthDetailsDto?> GetAuthDetailsByIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        // 1. Optimized query filtering active user by Id and projecting directly to auth DTO
        return await dbContext.Set<User>()
            .AsNoTracking()
            .Where(u => u.Id == userId && !u.IsDeleted)
            .ProjectToType<UserAuthDetailsDto>()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserProfileDto?> GetUserProfileByIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        // 1. Direct projection from database entity to detailed profile DTO skipping change tracking
        return await dbContext.Set<User>()
            .AsNoTracking()
            .Where(u => u.Id == userId && !u.IsDeleted)
            .ProjectToType<UserProfileDto>()
            .FirstOrDefaultAsync(cancellationToken);
    }
}