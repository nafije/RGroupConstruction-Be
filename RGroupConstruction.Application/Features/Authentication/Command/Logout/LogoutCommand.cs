using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Authentication.Command.Logout;

public record LogoutCommand(string UserId) : ICommand<bool>;


