namespace RGroupConstruction.Application.DTOs.ApiResponse;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse SuccessResponse(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public static ApiResponse FailureResponse(string error)
    {
        return new ApiResponse
        {
            Success = false,
            Errors = new List<string> { error }
        };
    }

    public static ApiResponse FailureResponse(List<string> errors)
    {
        return new ApiResponse
        {
            Success = false,
            Errors = errors
        };
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static new ApiResponse<T> FailureResponse(string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Errors = new List<string> { error }
        };
    }

    public static new ApiResponse<T> FailureResponse(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Errors = errors
        };
    }
}


