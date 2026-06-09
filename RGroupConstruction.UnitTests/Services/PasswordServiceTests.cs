using RGroupConstruction.Application.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace RGroupConstruction.UnitTests.Services;

public class PasswordServiceTests
{
    private readonly PasswordService _service = new(NullLogger<PasswordService>.Instance);

    [Fact]
    public void HashPassword_AndVerifyPassword_RoundTrip()
    {
        var hash = _service.HashPassword("Str0ng!Password");

        Assert.True(_service.VerifyPassword("Str0ng!Password", hash));
        Assert.False(_service.VerifyPassword("WrongPassword1!", hash));
    }

    [Fact]
    public void IsPasswordStrong_ValidatesRequiredCharacterGroups()
    {
        Assert.True(_service.IsPasswordStrong("Str0ng!Password"));
        Assert.False(_service.IsPasswordStrong("weak"));
    }

    [Fact]
    public void GenerateRandomPassword_UsesRequestedLength()
    {
        var password = _service.GenerateRandomPassword(16);

        Assert.Equal(16, password.Length);
        Assert.True(_service.IsPasswordStrong(password));
    }

    [Fact]
    public void HashPassword_WhenCancelled_Throws()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() => _service.HashPassword("password", cts.Token));
    }
}

