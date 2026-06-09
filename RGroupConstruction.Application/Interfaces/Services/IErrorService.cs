using RGroupConstruction.Application.Common;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IErrorService
{
    Result<T> CreateFailure<T>(string errorCode, string? language = null, params object[] parameters);
    Result<T> CreateFailure<T>(string errorCode, Exception? exception, string? language = null, params object[] parameters);
    Result CreateFailure(string errorCode, string? language = null, params object[] parameters);
    Result CreateFailure(string errorCode, Exception? exception, string? language = null, params object[] parameters);
    string GetErrorMessage(string errorCode, string? language = null, params object[] parameters);
    string GetCurrentUserLanguage();
}

