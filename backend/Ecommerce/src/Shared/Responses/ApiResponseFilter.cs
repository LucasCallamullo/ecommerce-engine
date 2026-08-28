using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.Shared.Responses;

// Intercepts successful API responses globally and wraps them in a consistent ApiResponseDto format.
// Operates as middleware-like infrastructure for outgoing controller results.
public class ApiResponseFilter : IAsyncResultFilter
{
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

    // Checks whether the object is already wrapped or represents a structured error response.
    private bool IsAlreadyWrapped(object value)
    {
        var type = value.GetType();
        
        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ApiResponseDto<>)) 
               || type.Name.Contains("ErrorResponse");
    }
}