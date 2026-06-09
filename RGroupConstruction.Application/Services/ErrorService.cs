using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Common.Error;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Services;

public class ErrorService(
    ILogger<ErrorService> _logger,
    IMessageLocalizer _localizer) : IErrorService
{
    public Result<T> CreateFailure<T>(string errorCode, string? language = null, params object[] parameters)
    {
        return CreateFailure<T>(errorCode, exception: null, language, parameters);
    }

    public Result<T> CreateFailure<T>(string errorCode, Exception? exception, string? language = null, params object[] parameters)
    {
        var message = GetErrorMessage(errorCode, language, parameters);

        if (exception != null)
            _logger.Error(exception, "Error occurred: {ErrorCode} - {Message}", errorCode, message);
        else
            _logger.Warn("Error occurred: {ErrorCode} - {Message}", errorCode, message);

        return Result<T>.Error(message);
    }

    public Result CreateFailure(string errorCode, string? language = null, params object[] parameters)
    {
        return CreateFailure(errorCode, exception: null, language, parameters);
    }

    public Result CreateFailure(string errorCode, Exception? exception, string? language = null, params object[] parameters)
    {
        var message = GetErrorMessage(errorCode, language, parameters);

        if (exception != null)
            _logger.Error(exception, "Error occurred: {ErrorCode} - {Message}", errorCode, message);
        else
            _logger.Warn("Error occurred: {ErrorCode} - {Message}", errorCode, message);

        return Result.Error(message);
    }

    public string GetErrorMessage(string errorCode, string? language = null, params object[] parameters)
    {
        var resxKey = MapErrorCodeToMessageKey(errorCode);
        var message = _localizer[resxKey];

        if (parameters.Length > 0)
        {
            try
            {
                message = string.Format(message, parameters);
            }
            catch (FormatException ex)
            {
                _logger.Error(ex, "Failed to format error message for code {ErrorCode}", errorCode);
            }
        }

        return message;
    }

    public string GetCurrentUserLanguage()
    {
        return System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    }

    private static string MapErrorCodeToMessageKey(string errorCode)
        => errorCode switch
        {
            ErrorCodes.EXTERNAL_SERVICE_ERROR => MessageKeys.Auth.ExternalServiceError,
            ErrorCodes.USER_NOT_FOUND => MessageKeys.Auth.UserNotFound,
            ErrorCodes.USER_INVALID_CREDENTIALS => MessageKeys.Auth.InvalidCredentials,
            ErrorCodes.AUTH_INVALID_TOKEN or ErrorCodes.INVALID_TOKEN => MessageKeys.Auth.InvalidToken,
            ErrorCodes.AUTH_TOKEN_EXPIRED or ErrorCodes.TOKEN_EXPIRED => MessageKeys.Auth.TokenExpired,
            ErrorCodes.AUTH_REFRESH_TOKEN_INVALID or ErrorCodes.AUTH_INVALID_REFRESH_TOKEN => MessageKeys.Auth.RefreshTokenInvalid,
            ErrorCodes.AUTH_REFRESH_TOKEN_EXPIRED => MessageKeys.Auth.RefreshTokenExpired,
            _ => errorCode
        };
}

