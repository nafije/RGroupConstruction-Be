using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Subscribe.Commands.CreateSubscription;

internal class CreateSubscriptionCommandHandler(ISubscribeService _subscribeService) : ICommandHandler<CreateSubscriptionCommand, SubscribeDto>
{
    public async Task<Result<SubscribeDto>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
        => await _subscribeService.SubscribeAsync(request, cancellationToken);
}
