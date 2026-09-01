using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.Shared.Responses;

/// <summary>
/// Intercepts successful API controller results globally and encapsulates outgoing object payloads 
/// into a standardized <see cref="ApiResponseDto{T}"/> wrapper.
/// </summary>
public class ApiResponseFilter : IAsyncResultFilter
{
    /// <summary>
    /// Asynchronously surrounds an outgoing result with a uniform response metadata structure prior to serialization.
    /// </summary>
    /// <param name="context">The context for the action result executing context.</param>
    /// <param name="next">The delegate executed asynchronously to invoke the next result filter or action result.</param>
    /// <returns>A task that represents the completion of the filter execution pipeline.</returns>
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // Only intercept results containing an object payload (e.g., OkObjectResult, CreatedAtActionResult)
        if (context.Result is ObjectResult objectResult)
        {
            var value = objectResult.Value;

            // Skip wrapping if the payload is already enclosed in ApiResponseDto or an ErrorResponse DTO
            if (value is not null && IsAlreadyWrapped(value))
            {
                await next();
                return;
            }

            var statusCode = objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;
            var path = context.HttpContext.Request.Path;

            // Wrap the original payload in the standard response container
            var wrappedResponse = new ApiResponseDto<object?>(statusCode, value, path);
            objectResult.Value = wrappedResponse;
        }

        await next();
    }

    /// <summary>
    /// Determines whether the output payload is already enclosed within an <see cref="ApiResponseDto{T}"/> 
    /// or represents a structured error response payload.
    /// </summary>
    /// <param name="value">The object instance to inspect for existing wrapper signatures.</param>
    /// <returns><c>true</c> if the object is already wrapped or is an error response; otherwise, <c>false</c>.</returns>
    private static bool IsAlreadyWrapped(object value)
    {
        var type = value.GetType();
        
        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ApiResponseDto<>)) 
               || type.Name.Contains("ErrorResponse");
    }
}