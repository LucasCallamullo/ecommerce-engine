namespace Ecommerce.Shared.Responses;

/// <summary>
/// Data transfer object representing a standardized API response wrapper for successful HTTP operations.
/// </summary>
/// <typeparam name="T">The type of the payload data contained within the response.</typeparam>
public class ApiResponseDto<T>
{
    /// <summary>
    /// Gets or sets a value indicating whether the request execution was successful.
    /// Defaults to <c>true</c> for standard success responses.
    /// </summary>
    public bool Success { get; set; } = true;

    public int Status { get; set; }

    public T? Data { get; set; }

    public string Path { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponseDto{T}"/> class with specific response parameters.
    /// </summary>
    /// <param name="statusCode">The HTTP status code assigned to the response payload.</param>
    /// <param name="data">The primary data payload returned by the application logic.</param>
    /// <param name="path">The relative URL path of the incoming HTTP request.</param>
    public ApiResponseDto(int statusCode, T? data, string path)
    {
        Status = statusCode;
        Data = data;
        Path = path;
    }
}