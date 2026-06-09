using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Subscribe.Commands.CreateSubscription;
using RGroupConstruction.Application.Features.Subscribe.Commands.RemoveSubscription;
using RGroupConstruction.Application.Features.Subscribe.Queries.GetAllPagedSubscriptions;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Domain.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace RGroupConstruction.Application.Services;


public class SubscribeService(IUnitOfWork _unitOfWork, IMapper _mapper, UserManager<User> _userManager, IEmailService _emailService, IJwtTokenService _jwtTokenService, INotificationService _notificationService, ILogger<SubscribeService> _logger, IMessageLocalizer _localizer) : ISubscribeService
{
    public async Task<Result<PagedResponse<SubscribeDto>>> GetALlPagedSubscriptionsAsync(GetAllPagedSubscriptionsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged subscriptions page {PageNumber} with size {PageSize} and search {Search}", request.PageNr, request.PageSize, request.Search);

        var query = _unitOfWork.Repository<Subscribe>()
             .AsQueryable()
             .Where(c => !c.IsDeleted);

        // Search
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(c =>
                (c.Email! != null && c.Email.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var subscribtions = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<List<SubscribeDto>>(subscribtions);

        var pagedResponse = new PagedResponse<SubscribeDto>(mapped, totalCount, request.PageNr, request.PageSize);

        _logger.Info("Retrieved {SubscriptionCount} subscriptions from total {TotalCount}", mapped.Count, totalCount);

        return Result<PagedResponse<SubscribeDto>>.Success(pagedResponse);
    }

    public async Task<Result<SubscribeDto>> SubscribeAsync(CreateSubscriptionCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating subscription for {SubscriberEmail}", request.Email);

        var subscription = await _unitOfWork.Repository<Subscribe>()
            .FirstOrDefaultAsync(p => p.Email == request.Email && !p.IsDeleted, cancellationToken);

        // Already actively subscribed — block duplicate
        if (subscription is not null && !subscription.IsUnsubscribed)
        {
            _logger.Warn("Subscription already exists for {SubscriberEmail}", request.Email);
            return Result<SubscribeDto>.Error(_localizer[MessageKeys.Error.Subscribe.AlreadySubscribed]);
        }

        if (subscription is null)
        {
            // New subscriber
            subscription = new Subscribe
            {
                Email = request.Email,
            };

            await _unitOfWork.Repository<Subscribe>().AddAsync(subscription, cancellationToken);
        }
        else if (subscription.IsUnsubscribed)
        {
            // Re-subscribing after having unsubscribed
            subscription.IsUnsubscribed = false;
            await _unitOfWork.Repository<Subscribe>().UpdateAsync(subscription, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.Info("Subscription saved for {SubscriberEmail} with id {SubscriptionId}", request.Email, subscription.Id);

        // Notify admin
        await _notificationService.SendNotificationToAdminsAsync(
            _localizer[MessageKeys.Notification.Subscribe.Subscribed, request.Email!],
            UserNotificationType.NewSubscribe);
        _logger.Info("Sent admin notification for subscription {SubscriptionId}", subscription.Id);

        var adminUsers = await _userManager.GetUsersInRoleAsync(SystemPolicies.Admin);
        var admin = adminUsers.FirstOrDefault(a => !a.IsDeleted);

        if (admin is not null)
        {
            await _emailService.SendNewSubscriptionEmailToAdminAsync(admin.Email!, request.Email!, cancellationToken);
            _logger.Info("Sent subscription email notification to admin {AdminUserId}", admin.Id);
        }
        else
            _logger.Warn("No admin user found for subscription {SubscriptionId} email notification", subscription.Id);

        return Result<SubscribeDto>.Success(_mapper.Map<SubscribeDto>(subscription));
    }

    public async Task<Result<bool>> UsubscribeAsync(RemoveSubscriptionCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Processing unsubscribe request");

        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.Token!, cancellationToken);
        var email = principal?.Claims.FirstOrDefault(c =>
            c.Type == "email" ||
            c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            _logger.Warn("Unsubscribe request had an invalid token without an email claim");
            return Result<bool>.Error(_localizer[MessageKeys.Error.Subscribe.InvalidToken]);
        }

        var subscription = await _unitOfWork.Repository<Subscribe>()
            .FirstOrDefaultAsync(p => p.Email == email && !p.IsDeleted, cancellationToken);

        if (subscription is null)
        {
            _logger.Warn("Subscription was not found for unsubscribe email {SubscriberEmail}", email);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Subscribe.NotSubscribed]);
        }

        subscription.IsUnsubscribed = true;

        await _unitOfWork.Repository<Subscribe>().UpdateAsync(subscription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.Info("Unsubscribed {SubscriberEmail} with subscription id {SubscriptionId}", email, subscription.Id);

        // Notify admin
        await _notificationService.SendNotificationToAdminsAsync(
            _localizer[MessageKeys.Notification.Subscribe.Unsubscribed, email],
            UserNotificationType.Unsubscribe);
        _logger.Info("Sent admin notification for unsubscribe {SubscriptionId}", subscription.Id);

        var adminUsers = await _userManager.GetUsersInRoleAsync(SystemPolicies.Admin);
        var admin = adminUsers.FirstOrDefault(a => !a.IsDeleted);

        if (admin is not null)
        {
            await _emailService.SendUnsubscribeEmailToAdminAsync(admin.Email!, email, cancellationToken);
            _logger.Info("Sent unsubscribe email notification to admin {AdminUserId}", admin.Id);
        }
        else
            _logger.Warn("No admin user found for unsubscribe {SubscriptionId} email notification", subscription.Id);

        return Result<bool>.Success(_mapper.Map<bool>(true));
    }
}

