using RGroupConstruction.Application.Resources;

namespace RGroupConstruction.Application.Common.Error;

public static class ExceptionMessages
{
    // Configuration
    public const string JwtSecretKeyNotConfigured = MessageKeys.Exception.JwtSecretKeyNotConfigured;
    public const string HangfireConnectionRequired = MessageKeys.Exception.HangfireConnectionRequired;
    public const string ConnectionStringNotFound = MessageKeys.Exception.ConnectionStringNotFound;
    public const string ConnectionStringNotFoundPostgres = MessageKeys.Exception.ConnectionStringNotFoundPostgres;

    // Authentication / JWT
    public const string InvalidToken = MessageKeys.Exception.InvalidToken;
    public const string UserIdNotFound = MessageKeys.Exception.UserIdNotFound;
}

