using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IAuthenticationService
{
    Task<Result<AuthenticationResponseDto>> LoginAsync(string login, string password, string? ipAddress = null, string? deviceInfo = null, CancellationToken cancellationToken = default);
    Task<Result<bool>> LogoutAsync(string userId, CancellationToken cancellationToken = default);
}

