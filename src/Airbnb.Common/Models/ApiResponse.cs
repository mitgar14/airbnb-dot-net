namespace Airbnb.Common.Models;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string message, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Message = message,
            Data = data,
            Success = true,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, int statusCode = 500)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Message = message,
            Data = default,
            Success = false,
            Timestamp = DateTime.UtcNow
        };
    }
}
