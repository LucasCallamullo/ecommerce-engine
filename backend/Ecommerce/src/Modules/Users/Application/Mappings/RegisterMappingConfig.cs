namespace Ecommerce.Users.Application.Mappings;

using Mapster;
using Ecommerce.Users.Contracts.DTOs;
using Ecommerce.Users.Domain.Entities;
using Ecommerce.Shared.Auth.Enums;

/// <summary>
/// Configures Mapster object-to-object mapping rules and LINQ projection behaviors 
/// for the Users domain entities and Data Transfer Objects (DTOs).
/// </summary>
public class RegisterMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // -------------------------------------------------------------------------
        // 1. Projection Rule: User -> UserAuthDetailsDto
        // -------------------------------------------------------------------------
        // Translates directly into an optimized SQL SELECT (with LEFT JOINs).
        // Automatically projects nested UserRoles.Role.Name into a flat collection of strings.
        config.NewConfig<User, UserAuthDetailsDto>()
            .Map(
                dest => dest.Roles, 
                src => src.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : string.Empty)
            );

        // -------------------------------------------------------------------------
        // 2. Projection Rule: User -> UserProfileDto
        // -------------------------------------------------------------------------
        config.NewConfig<User, UserProfileDto>()
            .Map(
                dest => dest.Roles, 
                src => src.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : string.Empty)
            );

        // -------------------------------------------------------------------------
        // 2. Command Rule: CreateUserDto -> User (In-Memory Entity Instantiation)
        // -------------------------------------------------------------------------
        // Sets essential domain defaults upon initial entity creation.
        config.NewConfig<CreateUserDto, User>()
            .Map(dest => dest.Id, src => Guid.CreateVersion7())
            .Map(dest => dest.IsActive, src => true);

        // -------------------------------------------------------------------------
        // 3. Response Rule: User -> UserCreatedDto (In-Memory Response Mapping)
        // -------------------------------------------------------------------------
        // NOTE ON 'with' SYNTAX:
        // Newly created User entities in memory only have UserRoles populated with RoleId,
        // while navigation properties like ur.Role remain null before an explicit DB fetch.
        // This mapping rule gracefully falls back to casting RoleId to UserRoleEnum when 
        // ur.Role is null. This allows returning 'user.Adapt<UserCreatedDto>()' directly 
        // without requiring manual post-processing patches like 'response with { Roles = [...] }'.
        config.NewConfig<User, UserCreatedDto>()
            .Map(dest => dest.Roles, src => src.UserRoles.Select(ur => 
                ur.Role != null 
                    ? ur.Role.Name 
                    : ((UserRoleEnum)ur.RoleId).ToString()
            ));
    }
}