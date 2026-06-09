using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Subscribe.Commands.CreateSubscription;

public class CreateSubscriptionCommand : ICommand<SubscribeDto>
{
    public string? Email { get; set; }
}

