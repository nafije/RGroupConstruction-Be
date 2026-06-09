using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Common.Error;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Features.Authentication.Command.Login;


public class LoginCommandHandler(
    IAuthenticationService _authenticationService,
    ILogger<LoginCommandHandler> _logger,
    IErrorService _errorService,
    IHttpContextAccessor _httpContextAccessor) : ICommandHandler<LoginCommand, AuthenticationResponseDto>
{
    public async Task<Result<AuthenticationResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Info("Login attempt for {Login}", request.Login);

            var ipAddress = request.LastIPAddress
                ?? _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var userAgent = request.UserAgent
                ?? _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

            var result = await _authenticationService.LoginAsync(request.Login, request.Password, ipAddress, userAgent, cancellationToken);

            if (result.IsSuccessful)
                _logger.Info("Login successful for {Login}", request.Login);
            else
                _logger.Warn("Login failed for {Login}: {ErrorResult}", request.Login, result.ErrorResult);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Login error for {Login}", request.Login);
            return _errorService.CreateFailure<AuthenticationResponseDto>(ErrorCodes.EXTERNAL_SERVICE_ERROR);
        }
    }
}


