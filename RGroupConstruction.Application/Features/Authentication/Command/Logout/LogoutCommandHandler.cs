using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Common.Error;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Features.Authentication.Command.Logout;


public class LogoutCommandHandler(
    IAuthenticationService _authenticationService,
    ILogger<LogoutCommandHandler> _logger,
    IErrorService _errorService) : ICommandHandler<LogoutCommand, bool>
{
    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Info("Logout request for user {UserId}", request.UserId);

            if (string.IsNullOrWhiteSpace(request.UserId))
                return _errorService.CreateFailure<bool>(ErrorCodes.USER_NOT_FOUND);

            var result = await _authenticationService.LogoutAsync(request.UserId, cancellationToken);

            if (result.IsSuccessful)
                _logger.Info("User {UserId} logged out successfully", request.UserId);
            else
                _logger.Warn("Logout failed for user {UserId}: {ErrorResult}", request.UserId, result.ErrorResult);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error logging out user {UserId}", request.UserId);
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);
        }
    }
}

