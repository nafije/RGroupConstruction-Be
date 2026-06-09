using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Subscribe.Commands.CreateSubscription;
using RGroupConstruction.Application.Features.Subscribe.Commands.RemoveSubscription;
using RGroupConstruction.Application.Features.Subscribe.Queries.GetAllPagedSubscriptions;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface ISubscribeService
{
    Task<Result<SubscribeDto>> SubscribeAsync(CreateSubscriptionCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UsubscribeAsync(RemoveSubscriptionCommand request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<SubscribeDto>>> GetALlPagedSubscriptionsAsync(GetAllPagedSubscriptionsQuery request, CancellationToken cancellationToken = default);
}

