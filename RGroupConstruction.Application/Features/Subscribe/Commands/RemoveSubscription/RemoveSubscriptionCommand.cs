using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Subscribe.Commands.RemoveSubscription;

public class RemoveSubscriptionCommand : ICommand<bool>
{
    public string? Token { get; set; }
}

