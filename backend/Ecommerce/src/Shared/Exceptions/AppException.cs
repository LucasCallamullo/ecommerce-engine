// Import System.Net for HttpStatusCode enum values
using System.Net;

namespace Ecommerce.Shared.Exceptions;

// Custom application exception class used for controlled business logic errors
public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }

    // Holds an optional list of detailed validation or contextual error messages
    public List<string>? Errors { get; }

    public AppException(
        string message, 
        HttpStatusCode statusCode = HttpStatusCode.BadRequest, 
        List<string>? errors = null) 
        : base(message) // Pass main message to the base System.Exception class
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}