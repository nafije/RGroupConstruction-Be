namespace RGroupConstruction.Application.Common.Email;

public class EmailSettings
{
    public string? SmtpServer { get; set; }
    public int SmtpPort { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FromEmail { get; set; }
    public string? FromName { get; set; }
    public bool EnableSsl { get; set; } = true;
    public string? BaseUrl { get; set; }
    public string? SupportEmail { get; set; }
    public string? ApiKey { get; set; }
}

