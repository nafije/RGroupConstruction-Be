using System.Security.Claims;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims, CancellationToken cancellationToken = default);
    string GenerateRefreshToken(CancellationToken cancellationToken = default);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, CancellationToken cancellationToken = default);
    bool ValidateToken(string token, CancellationToken cancellationToken = default);
    DateTime? GetTokenExpiration(string token, CancellationToken cancellationToken = default);
    string GenerateUnsubscribeToken(string email, CancellationToken cancellationToken = default);
}

