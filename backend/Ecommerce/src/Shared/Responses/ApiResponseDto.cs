namespace Ecommerce.Shared.Responses;

public class ApiResponseDto<T>
{
    public bool Success { get; set; } = true;
    public int Status { get; set; }
    public T? Data { get; set; }
    public string Path { get; set; }

    public ApiResponseDto(int statusCode, T? data, string path)
    {
        Status = statusCode;
        Data = data;
        Path = path;
    }
}