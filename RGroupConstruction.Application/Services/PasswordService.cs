using RGroupConstruction.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace RGroupConstruction.Application.Services;


public class PasswordService(ILogger<PasswordService> _logger) : IPasswordService
{
    public string HashPassword(string password, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[passwordBytes.Length + salt.Length];
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(saltedPassword);

            var result = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);

            _logger.Info("Password hash generated successfully");
            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to hash password");
            throw;
        }
    }

    public bool VerifyPassword(string password, string hashedPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var storedBytes = Convert.FromBase64String(hashedPassword);

            var salt = new byte[32];
            Buffer.BlockCopy(storedBytes, 0, salt, 0, 32);

            var storedHash = new byte[storedBytes.Length - 32];
            Buffer.BlockCopy(storedBytes, 32, storedHash, 0, storedHash.Length);

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[passwordBytes.Length + salt.Length];
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            using var sha256 = SHA256.Create();
            var computedHash = sha256.ComputeHash(saltedPassword);

            var isValid = computedHash.SequenceEqual(storedHash);
            if (!isValid)
                _logger.Warn("Password verification failed");

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to verify password");
            return false;
        }
    }

    public bool IsPasswordStrong(string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(password) || password.Length < 8)
        {
            _logger.Warn("Password strength check failed: minimum length was not met");
            return false;
        }

        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            _logger.Warn("Password strength check failed: missing lowercase character");
            return false;
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            _logger.Warn("Password strength check failed: missing uppercase character");
            return false;
        }

        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            _logger.Warn("Password strength check failed: missing digit");
            return false;
        }

        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
        {
            _logger.Warn("Password strength check failed: missing special character");
            return false;
        }

        _logger.Info("Password strength check passed");
        return true;
    }

    public string GenerateRandomPassword(int length = 12, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string special = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        var allChars = lowercase + uppercase + digits + special;
        var random = new Random();
        var password = new StringBuilder();

        password.Append(lowercase[random.Next(lowercase.Length)]);
        password.Append(uppercase[random.Next(uppercase.Length)]);
        password.Append(digits[random.Next(digits.Length)]);
        password.Append(special[random.Next(special.Length)]);

        for (int i = 4; i < length; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }

        var passwordArray = password.ToString().ToCharArray();
        for (int i = passwordArray.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (passwordArray[i], passwordArray[j]) = (passwordArray[j], passwordArray[i]);
        }

        _logger.Info("Generated random password with length {PasswordLength}", length);
        return new string(passwordArray);
    }
}


