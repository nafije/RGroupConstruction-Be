using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Common.Email;
using RGroupConstruction.Application.Common.Error;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Reflection;

namespace RGroupConstruction.Application.Services;

public class EmailService(
    IOptions<EmailSettings> emailSettingsOptions,
    ILogger<EmailService> _logger,
    IErrorService _errorService,
    IJwtTokenService _jwtTokenService,
    IMessageLocalizer _localizer) : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettingsOptions.Value;

    public async Task<Result<bool>> SendContactMessageAdminNotificationAsync(string fullName, string email, string phone, string subject, string message, string adminEmail,
         CancellationToken cancellationToken = default)
    {
        var body = await LoadTemplateAsync("RGroupConstructionContactMessageAdminTemplate", cancellationToken);
        if (body is null)
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);

        body = body
            .Replace("{{FullName}}", fullName)
            .Replace("{{Email}}", email)
            .Replace("{{Phone}}", phone ?? "N/A")
            .Replace("{{Subject}}", subject)
            .Replace("{{Message}}", message);

        return await SendEmailAsync(adminEmail, string.Format(LocalizedSubject("New Contact Message: {0}", "Mesazh i ri kontakti: {0}"), subject), body, isHtml: true, cancellationToken);
    }

    public async Task<Result<bool>> SendNewSubscriptionEmailToAdminAsync(string adminEmail, string subscriberEmail,
       CancellationToken cancellationToken = default)
    {
        var body = await LoadTemplateAsync("RGroupConstructionNewSubscriptionEmailTemplate", cancellationToken);
        if (body is null)
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);

        var rawBaseUrl = _emailSettings.BaseUrl;
        if (!Uri.TryCreate(rawBaseUrl, UriKind.Absolute, out var parsedBaseUrl))
        {
            _logger.Warn("Email BaseUrl is missing or invalid in configuration.");
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);
        }

        var baseUrl = parsedBaseUrl.ToString().TrimEnd('/');
        var subscribersLink = $"{baseUrl}/admin/subscribers";

        body = body
            .Replace("{{SubscriberEmail}}", subscriberEmail)
            .Replace("{{SubscribedDate}}", DateTime.UtcNow.ToString("MMMM dd, yyyy", CultureInfo.CurrentUICulture))
             .Replace("{{Subscribers}}", subscribersLink);

        return await SendEmailAsync(adminEmail, LocalizedSubject("New Newsletter Subscription", "Abonim i ri ne newsletter"), body, isHtml: true, cancellationToken);
    }

    public async Task<Result<bool>> SendUnsubscribeEmailToAdminAsync(string adminEmail, string subscriberEmail,
        CancellationToken cancellationToken = default)
    {
        var body = await LoadTemplateAsync("RGroupConstructionUnsubscribeEmailTemplate", cancellationToken);
        if (body is null)
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);

        var rawBaseUrl = _emailSettings.BaseUrl;
        if (!Uri.TryCreate(rawBaseUrl, UriKind.Absolute, out var parsedBaseUrl))
        {
            _logger.Warn("Email BaseUrl is missing or invalid in configuration.");
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);
        }

        var baseUrl = parsedBaseUrl.ToString().TrimEnd('/');
        var subscribersLink = $"{baseUrl}/admin/subscribers";

        body = body
            .Replace("{{SubscriberEmail}}", subscriberEmail)
            .Replace("{{UnsubscribedDate}}", DateTime.UtcNow.ToString("MMMM dd, yyyy", CultureInfo.CurrentUICulture))
            .Replace("{{Subscribers}}", subscribersLink);

        return await SendEmailAsync(adminEmail, LocalizedSubject("Newsletter Unsubscription", "Cregjistrim nga newsletter"), body, isHtml: true, cancellationToken);
    }

    public async Task<Result<bool>> SendNewProjectEmailToUserAsync(
    string projectName, string projectType, string city, string description, string userEmail, string imageUrl, Guid projectId,
    CancellationToken cancellationToken = default)
    {
        var body = await LoadTemplateAsync("RGroupConstructionNewProjectTemplate", cancellationToken);
        if (body is null)
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);


        var rawBaseUrl = _emailSettings.BaseUrl;
        if (!Uri.TryCreate(rawBaseUrl, UriKind.Absolute, out var parsedBaseUrl))
        {
            _logger.Warn("Email BaseUrl is missing or invalid in configuration.");
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);
        }

        var baseUrl = parsedBaseUrl.ToString().TrimEnd('/');
        var applyLink = $"{baseUrl}/projects/{projectId}";

        var token = _jwtTokenService.GenerateUnsubscribeToken(userEmail);
        var unsubscribeLink = $"{baseUrl}/unsubscribe?token={token}";


        body = body
            .Replace("{{ProjectName}}", projectName)
            .Replace("{{ProjectType}}", projectType)
            .Replace("{{City}}", city)
            .Replace("{{Description}}", description ?? "N/A")
            .Replace("{{ProjectUrl}}", applyLink)
            .Replace("{{ImageUrl}}", $"https://RGroupConstruction-4.onrender.com/{imageUrl}")
            .Replace("{{UnsubscribeLink}}", unsubscribeLink);

        var subject = LocalizedSubject(
            $"New Project Available: {projectName}",
            $"Projekt i ri i disponueshëm: {projectName}");

        return await SendEmailAsync(userEmail, subject, body, isHtml: true, cancellationToken);
    }

    public async Task<Result<bool>> SendProjectUpdateEmailToUserAsync(
        string projectName, string oldStatus, string newStatus, string city, string userEmail, string imageUrl, Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var body = await LoadTemplateAsync("RGroupConstructionProjectStatusUpdateTemplate", cancellationToken);
        if (body is null)
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);

        var rawBaseUrl = _emailSettings.BaseUrl;
        if (!Uri.TryCreate(rawBaseUrl, UriKind.Absolute, out var parsedBaseUrl))
        {
            _logger.Warn("Email BaseUrl is missing or invalid in configuration.");
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);
        }

        var baseUrl = parsedBaseUrl.ToString().TrimEnd('/');
        var applyLink = $"{baseUrl}/projects/{projectId}";

        var token = _jwtTokenService.GenerateUnsubscribeToken(userEmail);
        var unsubscribeLink = $"{baseUrl}/unsubscribe?token={token}";

        body = body
            .Replace("{{ProjectName}}", projectName)
            .Replace("{{City}}", city)
            .Replace("{{OldStatus}}", oldStatus)
            .Replace("{{NewStatus}}", newStatus)
            .Replace("{{UpdatedDate}}", DateTime.UtcNow.ToString("MMMM dd, yyyy", CultureInfo.CurrentUICulture))
            .Replace("{{ProjectUrl}}", applyLink)
            .Replace("{{ImageUrl}}", $"https://RGroupConstruction-4.onrender.com/{imageUrl}")
            .Replace("{{UnsubscribeLink}}", unsubscribeLink);

        var subject = LocalizedSubject(
            $"Project Status Update: {projectName}",
            $"Përditësim i statusit të projektit: {projectName}");

        return await SendEmailAsync(userEmail, subject, body, isHtml: true, cancellationToken);
    }

    public async Task<Result<bool>> SendNewJobEmailToUserAsync(
    string jobTitle, string department, string location, string employmentType, string userEmail, Guid jobId,
    CancellationToken cancellationToken = default)
    {
        var body = await LoadTemplateAsync("RGroupConstructionNewJobTemplate", cancellationToken);
        if (body is null)
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);

        var rawBaseUrl = _emailSettings.BaseUrl;
        if (!Uri.TryCreate(rawBaseUrl, UriKind.Absolute, out var parsedBaseUrl))
        {
            _logger.Warn("Email BaseUrl is missing or invalid in configuration.");
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);
        }

        var baseUrl = parsedBaseUrl.ToString().TrimEnd('/');
        var applyLink = $"{baseUrl}/careers/{jobId}";

        var token = _jwtTokenService.GenerateUnsubscribeToken(userEmail);
        var unsubscribeLink = $"{baseUrl}/unsubscribe?token={token}";

        body = body
            .Replace("{{JobTitle}}", jobTitle)
            .Replace("{{Department}}", department)
            .Replace("{{Location}}", location ?? "N/A")
            .Replace("{{EmploymentType}}", employmentType)
            .Replace("{{ApplyLink}}", applyLink)
            .Replace("{{UnsubscribeLink}}", unsubscribeLink);

        var subject = LocalizedSubject(
            $"New Job Opening: {jobTitle}",
            $"Pozicion i ri pune: {jobTitle}");

        return await SendEmailAsync(userEmail, subject, body, isHtml: true, cancellationToken);
    }

    public async Task<Result<bool>> SendJobApplicationToAdminAsync(
    string adminEmail, string applicantFullName, string applicantEmail, string applicantPhone, string jobTitle, string message,
    CancellationToken cancellationToken = default)
    {
        var body = await LoadTemplateAsync("RGroupConstructionJobApplicationTemplate", cancellationToken);
        if (body is null)
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);

        body = body
            .Replace("{{ApplicantFullName}}", applicantFullName)
            .Replace("{{ApplicantEmail}}", applicantEmail)
            .Replace("{{ApplicantPhone}}", applicantPhone ?? "N/A")
            .Replace("{{JobTitle}}", jobTitle)
            .Replace("{{Message}}", message ?? "N/A")
            .Replace("{{AppliedDate}}", DateTime.UtcNow.ToString("MMMM dd, yyyy", CultureInfo.CurrentUICulture));

        var subject = LocalizedSubject(
            $"New Job Application: {jobTitle}",
            $"Aplikim i ri për punë: {jobTitle}");

        return await SendEmailAsync(adminEmail, subject, body, isHtml: true, cancellationToken);
    }


    private string LocalizedSubject(string en, string sq)
    {
        return GetTemplateCulture() switch
        {
            "en" => en,
            _ => sq
        };
    }

    private async Task<string?> LoadTemplateAsync(string templateBaseName, CancellationToken cancellationToken)
    {
        _logger.Info("Loading email template {TemplateBaseName}", templateBaseName);

        var assembly = Assembly.GetExecutingAssembly();
        var culture = GetTemplateCulture();
        var resourceName = FindTemplateResource(assembly, templateBaseName, culture)
            ?? FindTemplateResource(assembly, templateBaseName, SupportedLanguages.DefaultCode);

        if (resourceName is null)
        {
            _logger.Warn("Email template {TemplateBaseName} was not found in embedded resources", templateBaseName);
            return null;
        }

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        var body = await reader.ReadToEndAsync(cancellationToken);

        return ApplyCommonTemplateValues(body);
    }
    private static string? FindTemplateResource(Assembly assembly, string templateBaseName, string culture)
    {
        return assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($"{templateBaseName}_{culture}.html", StringComparison.OrdinalIgnoreCase));
    }
    private string GetTemplateCulture()
    {
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        return SupportedLanguages.IsSupported(culture) ? culture : SupportedLanguages.DefaultCode;
    }
    private string ApplyCommonTemplateValues(string body)
    {
        return body
            .Replace("{{Year}}", DateTime.UtcNow.Year.ToString(CultureInfo.InvariantCulture))
            .Replace("{{SupportEmail}}", GetSupportEmail());
    }
    private string GetSupportEmail()
    {
        if (!string.IsNullOrWhiteSpace(_emailSettings.SupportEmail))
            return _emailSettings.SupportEmail!;

        if (!string.IsNullOrWhiteSpace(_emailSettings.FromEmail))
            return _emailSettings.FromEmail!;

        return string.Empty;
    }

    private async Task<Result<bool>> SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Info("Sending email to {RecipientEmail} with subject {EmailSubject}", to, subject);

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_emailSettings.ApiKey}");

            var payload = new
            {
                from = $"{_emailSettings.FromName} <{_emailSettings.FromEmail}>",
                to = new[] { to },
                subject,
                html = body
            };

            var response = await http.PostAsJsonAsync(
                "https://api.resend.com/emails",
                payload,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.Warn("Resend API error: {Error}", error);
                return Result<bool>.Error(_localizer[MessageKeys.Error.Email.SendFailed, error]);
            }

            _logger.Info("Sent email to {RecipientEmail}", to);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to send email to {RecipientEmail}", to, subject);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Email.SendFailed, ex.Message]);
        }
    }

}

