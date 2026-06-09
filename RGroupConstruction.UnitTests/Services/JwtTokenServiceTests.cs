using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace RGroupConstruction.UnitTests.Services;

public class JwtTokenServiceTests
{
    [Fact]
    public void GetTokenExpiration_WhenTokenIsInvalid_ReturnsNull()
    {
        var service = new JwtTokenService(
            BuildConfiguration(),
            NullLogger<JwtTokenService>.Instance,
            new FakeMessageLocalizer());

        var expiration = service.GetTokenExpiration("not-a-jwt", CancellationToken.None);

        Assert.Null(expiration);
    }

    private static IConfiguration BuildConfiguration()
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "Yfk2FYTH8oT6SG9nxGqsCLXC6qf5C3FdJ0v/12QsOBo=",
                ["JwtSettings:Issuer"] = "RGroupConstruction",
                ["JwtSettings:Audience"] = "RGroupConstructionUsers",
                ["JwtSettings:ExpiryMinutes"] = "60"
            })
            .Build();

    private sealed class FakeMessageLocalizer : IMessageLocalizer
    {
        public string this[string key] => key;
        public string this[string key, params object[] args] => args.Length == 0 ? key : string.Format(key, args);
    }
}

