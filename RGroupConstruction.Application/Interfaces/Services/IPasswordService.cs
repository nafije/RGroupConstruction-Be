namespace RGroupConstruction.Application.Interfaces.Services;

public interface IPasswordService
{
    string HashPassword(string password, CancellationToken cancellationToken = default);
    bool VerifyPassword(string password, string hashedPassword, CancellationToken cancellationToken = default);
    bool IsPasswordStrong(string password, CancellationToken cancellationToken = default);
    string GenerateRandomPassword(int length = 12, CancellationToken cancellationToken = default);
}

