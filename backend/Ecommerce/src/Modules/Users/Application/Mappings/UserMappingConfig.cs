namespace Ecommerce.Users.Infrastructure.Mappings;

using Mapster;
using Ecommerce.Shared.Auth.Enums;
using Ecommerce.Users.Application.DTOs.Response;
using Ecommerce.Users.Domain.Entities;

/// <summary>
/// Configures Mapster object-to-object mapping rules and LINQ database projection behaviors 
/// specifically for <see cref="UserResponse"/> and Application-layer DTOs.
/// </summary>
public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // -------------------------------------------------------------------------
        // Projection & Mapping Rule: User -> UserResponse
        // -------------------------------------------------------------------------
        // Supports both database projections (ProjectToType) via EF Core LEFT JOINs 
        // and in-memory mappings (.Adapt) by falling back to casting RoleId to UserRoleEnum.
        config.NewConfig<User, UserResponse>()
            .Map(dest => dest.Roles, src => src.UserRoles.Select(ur =>
                ur.Role != null
                    ? ur.Role.Name
                    : ((UserRoleEnum)ur.RoleId).ToString()
            ));
    }
}