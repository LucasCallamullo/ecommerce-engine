using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using Mapster;

using Ecommerce.Users.Application.Services.Contracts;
using Ecommerce.Users.Application.Services.Internals;
using Ecommerce.Users.Contracts.Interfaces;
using Ecommerce.Users.Application.Interfaces;

namespace Ecommerce.Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        // 1. Module Contracts & Application Services Registration
        services.AddScoped<IUserContract, UserContract>();
        services.AddScoped<IUserService, UserService>();

        // 2. Automatic scanning of ALL AbstractValidators in the Application assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // 3. Automatic scanning of ALL Mapster IRegister configurations in the Application assembly
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        return services;
    }
}
