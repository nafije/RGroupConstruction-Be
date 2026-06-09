using RGroupConstruction.Application.Common;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IEmailService
{
    Task<Result<bool>> SendNewSubscriptionEmailToAdminAsync(string adminEmail, string subscriberEmail,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> SendUnsubscribeEmailToAdminAsync(string adminEmail, string subscriberEmail,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> SendContactMessageAdminNotificationAsync(string fullName, string email, string phone, string subject, string message, string adminEmail,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> SendNewProjectEmailToUserAsync(string projectName, string projectType, string city, string description, string userEmail, string imageUrl, Guid projectId,
       CancellationToken cancellationToken = default);
    Task<Result<bool>> SendProjectUpdateEmailToUserAsync(string projectName, string oldStatus, string newStatus, string city, string userEmail, string imageUrl, Guid projectId,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> SendNewJobEmailToUserAsync(string jobTitle, string department, string location, string employmentType, string userEmail, Guid jobId,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> SendJobApplicationToAdminAsync(string adminEmail, string applicantFullName, string applicantEmail, string applicantPhone, string jobTitle, string message,
        CancellationToken cancellationToken = default);
}

