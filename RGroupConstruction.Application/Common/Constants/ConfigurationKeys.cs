namespace RGroupConstruction.Application.Common.Constants;

public class ConfigurationKeys
{
    public static class ConnectionStrings
    {
        public const string Default = "Default";
    }

    public static class Sections
    {
        public const string JwtSettings = "JwtSettings";
        public const string Email = "EmailSettings";
        public const string Logging = "Logging";
        public const string Localization = "Localization";
    }

    public static class JwtSettingKeys
    {
        public const string SecretKey = "SecretKey";
        public const string Issuer = "Issuer";
        public const string Audience = "Audience";
        public const string ExpiryMinutes = "ExpiryMinutes";
        public const string RefreshTokenExpiryDays = "RefreshTokenExpiryDays";
    }
}

