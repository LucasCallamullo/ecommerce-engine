using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Ecommerce.Shared.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log the unhandled exception message and full stack trace for debugging
        // _logger.LogError(exception, "Unhandled exception captured: {Message}", exception.Message);
        if (exception is AppException appException)
        {
            // Para errores conocidos (404, 400, etc.), logueamos solo un mensaje limpio en Warning
            _logger.LogWarning("Handled AppException [{StatusCode}]: {Message}", 
                (int)appException.StatusCode, appException.Message);
        }
        else
        {
            // Solo imprimimos el stack trace completo si es un bug no esperado (500)
            _logger.LogError(exception, "Unhandled Exception: {Message}", exception.Message);
        }

        // Pattern match the exception type to determine status code, message, and error details
        var (statusCode, message, errors) = exception switch
        {
            // Handle custom application exceptions thrown explicitly from business logic
            AppException appEx => (
                (int)appEx.StatusCode,
                appEx.Message,
                appEx.Errors),

            // Handle Entity Framework database update and foreign key constraint failures
            DbUpdateException dbEx => (
                StatusCodes.Status400BadRequest,
                "A database constraint error occurred. Please verify related entity IDs (e.g. Category, Brand).",
                new List<string> { dbEx.InnerException?.Message ?? dbEx.Message }),

            // Fallback handler for all unexpected or unhandled internal system exceptions
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected internal server error occurred.",
                new List<string> { exception.Message })
        };

        // Set the HTTP response status code matching the resolved exception type
        httpContext.Response.StatusCode = statusCode;

        // Set the response Content-Type header to JSON format
        httpContext.Response.ContentType = "application/json";

        // Extract the request path to populate the Path property in ErrorResponseDto
        var requestPath = httpContext.Request.Path.Value ?? string.Empty;

        // Instantiate the standard error DTO with status, message, errors list, and path
        var response = new ErrorResponseDto(statusCode, message, errors, requestPath);

        // Write the ErrorResponseDto object as a JSON payload directly to the HTTP response stream
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        // Return true to inform ASP.NET Core that the exception was handled successfully
        return true;
    }
}