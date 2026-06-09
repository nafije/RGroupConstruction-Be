using RGroupConstruction.Application.Common.Error;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Application.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace RGroupConstruction.UnitTests.Services;

public class ErrorServiceTests
{
    [Fact]
    public void GetErrorMessage_MapsInvalidCredentialsToLocalizedMessage()
    {
        var localizer = new FakeMessageLocalizer(new Dictionary<string, string>
        {
            [MessageKeys.Auth.InvalidCredentials] = "Invalid credentials"
        });
        var service = new ErrorService(NullLogger<ErrorService>.Instance, localizer);

        var message = service.GetErrorMessage(ErrorCodes.USER_INVALID_CREDENTIALS);

        Assert.Equal("Invalid credentials", message);
    }

    [Fact]
    public void GetErrorMessage_WhenCodeIsUnknown_UsesCodeAsKey()
    {
        var localizer = new FakeMessageLocalizer(new Dictionary<string, string>
        {
            ["CUSTOM_CODE"] = "Custom message"
        });
        var service = new ErrorService(NullLogger<ErrorService>.Instance, localizer);

        var message = service.GetErrorMessage("CUSTOM_CODE");

        Assert.Equal("Custom message", message);
    }

    private sealed class FakeMessageLocalizer(IReadOnlyDictionary<string, string> messages) : IMessageLocalizer
    {
        public string this[string key] => messages.TryGetValue(key, out var value) ? value : key;

        public string this[string key, params object[] args]
        {
            get
            {
                var value = this[key];
                return args.Length == 0 ? value : string.Format(value, args);
            }
        }
    }
}

