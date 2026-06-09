using System.Text.Json.Serialization;

namespace RGroupConstruction.Application.Common;

public class Result
{
    public bool IsSuccessful { get; set; }

    public string? ErrorMessage => Exception?.Message;

    public string? ErrorResult { get; set; }

    public ValidationErrorCollection ValidationErrors { get; set; } = new();

    [JsonIgnore]
    public Exception? Exception { get; set; }

    public static Result<T> Success<T>(T data) => Result<T>.Success(data);
    public static Result Success() => new() { IsSuccessful = true };

    public static Result Error(string errorResult)
        => new() { IsSuccessful = false, ErrorResult = errorResult };

    public static Result Error(Exception exception)
        => new() { IsSuccessful = false, Exception = exception };

    public static Result Error(Exception exception, string errorResult)
        => new() { IsSuccessful = false, Exception = exception, ErrorResult = errorResult };

    public static Result ValidationError(ValidationErrorCollection errors)
        => new() { IsSuccessful = false, ValidationErrors = errors, ErrorResult = ResultMessages.ValidationFailed };
}

public class Result<T>
{
    public bool IsSuccessful { get; set; }

    public string? ErrorMessage => Exception?.Message;

    public string? ErrorResult { get; set; }

    public ValidationErrorCollection ValidationErrors { get; set; } = new();

    [JsonIgnore]
    public Exception? Exception { get; set; }

    public T? Data { get; set; }

    public static Result<T> Success(T data)
        => new() { IsSuccessful = true, Data = data };

    public static Result<T> Error(string errorResult)
        => new() { IsSuccessful = false, ErrorResult = errorResult };

    public static Result<T> Error(Exception exception)
        => new() { IsSuccessful = false, Exception = exception };

    public static Result<T> Error(Exception exception, string errorResult)
        => new() { IsSuccessful = false, Exception = exception, ErrorResult = errorResult };

    public static Result<T> ValidationError(ValidationErrorCollection errors)
        => new() { IsSuccessful = false, ValidationErrors = errors, ErrorResult = ResultMessages.ValidationFailed };

    public static implicit operator Result<T>(Result result)
        => new()
        {
            IsSuccessful = result.IsSuccessful,
            Exception = result.Exception,
            ErrorResult = result.ErrorResult,
            ValidationErrors = result.ValidationErrors
        };
}

