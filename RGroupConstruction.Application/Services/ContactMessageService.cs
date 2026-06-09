using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Features.ContactMessage.Commands.CreateContactMessage;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Domain.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Services;


public class ContactMessageService(
    IUnitOfWork _unitOfWork, 
    IMapper _mapper, 
    IEmailService _emailService, 
    UserManager<User> _userManager, 
    INotificationService _notificationService,
    ILogger<ContactMessageService> _logger,
    IMessageLocalizer _localizer) : IContactMessageService
{
    public async Task<Result<bool>> CreateAsync(CreateContactMessageCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating contact message from {Email} with subject {Subject}", request.Email, request.Subject);

        var newMessage = new ContactMessage
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Subject = request.Subject,
            Message = request.Message
        };

        await _unitOfWork.Repository<ContactMessage>().AddAsync(newMessage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.Info("Created contact message {ContactMessageId}", newMessage.Id);

        // Notify admin
        await _notificationService.SendNotificationToAdminsAsync(
            _localizer[MessageKeys.Notification.Contact.NewMessage, request.FullName!, request.Subject!],
            UserNotificationType.NewMessage);
        _logger.Info("Sent admin notification for contact message {ContactMessageId}", newMessage.Id);

        var adminUsers = await _userManager.GetUsersInRoleAsync(SystemPolicies.Admin);
        var admin = adminUsers.FirstOrDefault(a => !a.IsDeleted);

        if (admin is not null)
        {
            await _emailService.SendContactMessageAdminNotificationAsync(
                fullName: newMessage.FullName!,
                email: newMessage.Email!,
                phone: newMessage.Phone ?? "N/A",
                subject: newMessage.Subject!,
                message: newMessage.Message!,
                adminEmail: admin.Email!,
                cancellationToken: cancellationToken
                );
            _logger.Info("Sent contact message email notification to admin {AdminUserId}", admin.Id);
        }
        else
            _logger.Warn("No admin user found for contact message {ContactMessageId} email notification", newMessage.Id);

        return Result<bool>.Success(_mapper.Map<bool>(true));
    }
}

