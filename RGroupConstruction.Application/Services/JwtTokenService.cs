using RGroupConstruction.Application.Common.Constants;
using RGroupConstruction.Application.Common.Error;
using RGroupConstruction.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RGroupConstruction.Application.Services;

public class JwtTokenService(
    IConfiguration _configuration,
    ILogger<JwtTokenService> _logger,
    IMessageLocalizer _localizer) : IJwtTokenService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var jwtSettings = _configuration.GetSection(ConfigurationKeys.Sections.JwtSettings);
        var secretKey = jwtSettings[ConfigurationKeys.JwtSettingKeys.SecretKey] ?? throw new InvalidOperationException(_localizer[ExceptionMessages.JwtSecretKeyNotConfigured]);
        var issuer = jwtSettings[ConfigurationKeys.JwtSettingKeys.Issuer] ?? "RGroupConstruction";
        var audience = jwtSettings[ConfigurationKeys.JwtSettingKeys.Audience] ?? "RGroupConstructionUsers";
        var expiryMinutes = int.Parse(jwtSettings[ConfigurationKeys.JwtSettingKeys.ExpiryMinutes] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        _logger.Info("Generated access token for issuer {Issuer} and audience {Audience}", issuer, audience);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        _logger.Info("Generated refresh token");
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var jwtSettings = _configuration.GetSection(ConfigurationKeys.Sections.JwtSettings);
        var secretKey = jwtSettings[ConfigurationKeys.JwtSettingKeys.SecretKey] ?? throw new InvalidOperationException(_localizer[ExceptionMessages.JwtSecretKeyNotConfigured]);
        var issuer = jwtSettings[ConfigurationKeys.JwtSettingKeys.Issuer] ?? "RGroupConstruction";
        var audience = jwtSettings[ConfigurationKeys.JwtSettingKeys.Audience] ?? "RGroupConstruction";

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidIssuer = issuer,
            ValidAudience = audience,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.Warn("Expired token principal validation failed because token algorithm was invalid");
            throw new SecurityTokenException(_localizer[ExceptionMessages.InvalidToken]);
        }

        _logger.Info("Extracted principal from expired token");
        return principal;
    }

    public bool ValidateToken(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var jwtSettings = _configuration.GetSection(ConfigurationKeys.Sections.JwtSettings);
            var secretKey = jwtSettings[ConfigurationKeys.JwtSettingKeys.SecretKey] ?? throw new InvalidOperationException(_localizer[ExceptionMessages.JwtSecretKeyNotConfigured]);
            var issuer = jwtSettings[ConfigurationKeys.JwtSettingKeys.Issuer] ?? "RGroupConstruction";
            var audience = jwtSettings[ConfigurationKeys.JwtSettingKeys.Audience] ?? "RGroupConstructionUsers";

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidIssuer = issuer,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            _logger.Info("Token validation succeeded");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Token validation failed");
            return false;
        }
    }

    public string GenerateUnsubscribeToken(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var jwtSettings = _configuration.GetSection(ConfigurationKeys.Sections.JwtSettings);
        var secretKey = jwtSettings[ConfigurationKeys.JwtSettingKeys.SecretKey] ?? throw new InvalidOperationException(_localizer[ExceptionMessages.JwtSecretKeyNotConfigured]);
        var issuer = jwtSettings[ConfigurationKeys.JwtSettingKeys.Issuer] ?? "RGroupConstruction";
        var audience = jwtSettings[ConfigurationKeys.JwtSettingKeys.Audience] ?? "RGroupConstructionUsers";

        var claims = new[]
        {
            new Claim("email", email),
            new Claim("purpose", "unsubscribe")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: credentials
        );

        _logger.Info("Generated unsubscribe token for {SubscriberEmail}", email);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime? GetTokenExpiration(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            _logger.Info("Read token expiration {TokenExpiration}", jwtToken.ValidTo);
            return jwtToken.ValidTo;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to get token expiration");
            return null;
        }
    }
}


