// Namespace matching the folder structure in Shared module
namespace Ecommerce.Shared.Exceptions;

// Data Transfer Object representing the standard HTTP error payload returned to clients
public class ErrorResponseDto
{
    public bool Success { get; set; } = false;

    public int Status { get; set; }

    // Human-readable summary message describing the error
    public string Message { get; set; } = string.Empty;

    // The HTTP request route path where the error occurred
    public string Path { get; set; } = string.Empty;

    // Optional list of specific validation errors or exception details
    public List<string>? Errors { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Parameterless constructor required for JSON deserialization frameworks
    public ErrorResponseDto() { }

    public ErrorResponseDto(int statusCode, string message, List<string>? errors = null, string path = "")
    {
        Status = statusCode;
        Message = message;
        Errors = errors;
        Path = path;
    }
}