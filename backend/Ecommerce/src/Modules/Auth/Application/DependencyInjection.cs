namespace Ecommerce.Auth.Application;


using Mapster;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

using Ecommerce.Auth.Application.Interfaces;
using Ecommerce.Auth.Application.Services;
using Ecommerce.Auth.Infrastructure.Services;

using Ecommerce.Shared.Auth.Interfaces;

 
public static class DependencyInjection
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services)
    {
        // 1. Application Services Registration
        services.AddScoped<IAuthService, AuthService>();

        // 2. Security & Token Infrastructure Registration
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // 2. Automatic scanning of ALL AbstractValidators in the Application assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // 3. Automatic scanning of ALL Mapster IRegister configurations in the Application assembly
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
