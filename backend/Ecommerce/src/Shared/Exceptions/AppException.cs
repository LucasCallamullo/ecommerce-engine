namespace Ecommerce.Shared.Exceptions;

using System.Net;

/// <summary>
/// Custom application exception used to represent controlled domain and business logic errors 
/// with an associated HTTP status code and optional field validation details.
/// </summary>
public class AppException : Exception
{
    /// <summary>
    /// Gets the HTTP status code that should be mapped and returned in the HTTP error response.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets an optional collection of detailed validation error messages or contextual details.
    /// </summary>
    public List<string>? Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    /// <param name="message">The primary error message describing the exception.</param>
    /// <param name="statusCode">The HTTP status code to associated with the exception. 
    ///     Defaults to <see cref="HttpStatusCode.BadRequest"/>.
    /// </param>
    /// <param name="errors">An optional collection of specific validation or contextual error strings.</param>
    public AppException(
        string message, 
        HttpStatusCode statusCode = HttpStatusCode.BadRequest, 
        List<string>? errors = null) 
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}