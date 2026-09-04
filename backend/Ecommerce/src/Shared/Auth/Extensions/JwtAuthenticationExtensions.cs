namespace Ecommerce.Shared.Auth.Extensions;

using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Ecommerce.Shared.Auth.Configurations;
using Ecommerce.Shared.Auth.Interfaces;
using Ecommerce.Shared.Auth.Services;
using Ecommerce.Shared.Exceptions;

/// <summary>
/// Provides extension methods for registering JWT authentication and authorization 
/// infrastructure into the Dependency Injection container.
/// </summary>
public static class JwtAuthenticationExtensions
{
    /// <summary>
    /// Configures JWT Bearer authentication, binds JwtSettings, registers current user context services,
    /// and handles 401/403 responses using standardized ErrorResponseDto payloads.
    /// </summary>
    /// <param name="services">The service collection to add dependencies to.</param>
    /// <param name="configuration">The application configuration root to read JwtSettings from.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance for method chaining.</returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Bind JwtSettings from appsettings.json for inline validation parameters
        var jwtSettings = new JwtSettings();
        configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
        
        // Register JwtSettings into the IOptions<JwtSettings> pipeline
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Register HttpContextAccessor and custom current user context resolver
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        // Configure JWT Bearer default authentication schemes
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Define rules for validating incoming JWT tokens
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };

            // Intercept authentication and authorization failures to match ErrorResponseDto schema
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    // Suppress default challenge response execution
                    context.HandleResponse();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = new ErrorResponseDto(
                        StatusCodes.Status401Unauthorized,
                        "Access denied. A valid JWT token is required.",
                        ["Missing, expired, or invalid authorization token."],
                        context.Request.Path.Value ?? string.Empty
                    );

                    await context.Response.WriteAsJsonAsync(response);
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var response = new ErrorResponseDto(
                        StatusCodes.Status403Forbidden,
                        "Forbidden. You do not have permission to access this resource.",
                        ["Insufficient user roles or claims for this endpoint."],
                        context.Request.Path.Value ?? string.Empty
                    );

                    await context.Response.WriteAsJsonAsync(response);
                }
            };
        });

        // Enable policy-based authorization framework
        // services.AddAuthorization();

        // Enable policy-based authorization framework with global FallbackPolicy
        // Configure the global safety net: require JWT on ANY endpoint by default
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}