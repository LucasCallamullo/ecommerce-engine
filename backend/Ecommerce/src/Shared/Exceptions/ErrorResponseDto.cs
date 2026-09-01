namespace Ecommerce.Shared.Exceptions;

/// <summary>
/// Data transfer object representing the standardized API error payload returned on unhandled or domain exceptions.
/// </summary>
public class ErrorResponseDto
{
    public bool Success { get; set; } = false;

    public int Status { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public List<string>? Errors { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Initializes a new instance of <see cref="ErrorResponseDto"/>. 
    /// Required for JSON deserialization frameworks.
    /// </summary>
    public ErrorResponseDto() { }

    /// <summary>
    /// Initializes a new instance of <see cref="ErrorResponseDto"/> with custom exception parameters.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="message">The main error message summary.</param>
    /// <param name="errors">Optional collection of field-specific validation errors.</param>
    /// <param name="path">The HTTP request route path.</param>
    public ErrorResponseDto(int statusCode, string message, List<string>? errors = null, string path = "")
    {
        Status = statusCode;
        Message = message;
        Errors = errors;
        Path = path;
    }
}