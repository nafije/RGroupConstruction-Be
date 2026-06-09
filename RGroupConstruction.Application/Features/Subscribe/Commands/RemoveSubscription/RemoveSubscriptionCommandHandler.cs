using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Subscribe.Commands.RemoveSubscription;

internal class RemoveSubscriptionCommandHandler(ISubscribeService _subscribeService) : ICommandHandler<RemoveSubscriptionCommand, bool>
{
    public async Task<Result<bool>> Handle(RemoveSubscriptionCommand request, CancellationToken cancellationToken)
        => await _subscribeService.UsubscribeAsync(request, cancellationToken);
}

