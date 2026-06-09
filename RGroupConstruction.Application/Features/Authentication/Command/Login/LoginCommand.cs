using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Authentication.Command.Login;

public record LoginCommand(
    string Login,
    string Password,
    int? DeviceType = null,
    string? UserAgent = null,
    string? OperatingSystem = null,
    string? Browser = null,
    string? LastIPAddress = null) : ICommand<AuthenticationResponseDto>;
