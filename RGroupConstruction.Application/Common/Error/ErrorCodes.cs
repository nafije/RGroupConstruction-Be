namespace RGroupConstruction.Application.Common.Error;

public static class ErrorCodes
{
    public const string NOT_FOUND = "ERR_1001";
    public const string ALREADY_EXISTS = "ERR_1002";
    public const string INVALID_OPERATION = "ERR_1003";
    public const string UNAUTHORIZED = "ERR_1004";
    public const string FORBIDDEN = "ERR_1005";
    public const string VALIDATION_FAILED = "ERR_1006";
    public const string CONCURRENT_UPDATE = "ERR_1007";
    public const string EXTERNAL_SERVICE_ERROR = "ERR_1008";
    public const string INVALID_ARGUMENT = "ERR_1009";
    public const string DATABASE_ERROR = "ERR_1010";

    // User Errors (3000-3999)
    public const string USER_NOT_FOUND = "ERR_3001";
    public const string USER_INVALID_CREDENTIALS = "ERR_3006";

    // Authentication Errors (3100-3199)
    public const string AUTH_INVALID_TOKEN = "ERR_3101";
    public const string AUTH_TOKEN_EXPIRED = "ERR_3102";
    public const string AUTH_REFRESH_TOKEN_INVALID = "ERR_3103";
    public const string AUTH_REFRESH_TOKEN_EXPIRED = "ERR_3104";
    public const string AUTH_PASSWORD_RESET_TOKEN_INVALID = "ERR_3105";
    public const string AUTH_PASSWORD_RESET_TOKEN_EXPIRED = "ERR_3106";
    public const string AUTH_EMAIL_NOT_CONFIRMED = "ERR_3107";
    public const string AUTH_EMAIL_ALREADY_CONFIRMED = "ERR_3108";
    public const string AUTH_ACCOUNT_LOCKED = "ERR_3109";
    public const string AUTH_ACCOUNT_SUSPENDED = "ERR_3110";
    public const string AUTH_INVALID_REFRESH_TOKEN = "ERR_3111";
    public const string AUTH_TOKEN_BLACKLISTED = "ERR_3112";

    
    public const string TOKEN_EXPIRED = "ERR_1040";
    public const string INVALID_TOKEN = "ERR_1041";

}

