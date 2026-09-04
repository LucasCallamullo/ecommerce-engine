namespace Ecommerce.Shared.Filters;

using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Ecommerce.Shared.Exceptions;

/// <summary>
/// Global MVC Action Filter that automatically intercepts incoming HTTP request DTOs,
/// resolves their corresponding FluentValidation validators from DI, and executes validation rules.
/// </summary>
public class FluentValidationFilter : IAsyncActionFilter
{
    /// <summary>
    /// Intercepts the action execution pipeline to validate incoming action arguments before reaching the controller.
    /// </summary>
    /// <param name="context">The filter context containing action executing information and arguments.</param>
    /// <param name="next">The delegate representing the next action execution step in the HTTP pipeline.</param>
    /// <exception cref="AppException">Thrown when validation rules fail, encapsulating error messages and a 400 Bad Request status.</exception>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 1. Iterate through all incoming arguments passed to the controller action
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;

            // 2. Resolve the underlying runtime type of the argument
            var argumentType = argument.GetType();

            // 3. Construct the generic validator service type (e.g., IValidator<UpdateUserProfile>)
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            // 4. Attempt to resolve a registered validator instance from the Dependency Injection container
            if (context.HttpContext.RequestServices.GetService(validatorType) is IValidator validator)
            {
                // 5. Wrap the target payload object inside a FluentValidation execution context
                var validationContext = new ValidationContext<object>(argument);

                // 6. Asynchronously execute all defined validation rules passing the HTTP cancellation token
                var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

                // 7. If validation fails, extract and format all error messages
                if (!validationResult.IsValid)
                {
                    // 8. Flatten validation failure details into a list of "PropertyName: ErrorMessage" strings
                    var errorMessages = validationResult.Errors
                        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                        .ToList();

                    // 9. Interrupt execution by throwing the application domain exception to be handled globally
                    throw new AppException(
                        "Validation failed for one or more fields.", 
                        HttpStatusCode.BadRequest, 
                        errorMessages
                    );
                }
            }
        }

        // 10. Proceed to controller action execution if all payloads pass validation or lack registered validators
        await next();
    }
}