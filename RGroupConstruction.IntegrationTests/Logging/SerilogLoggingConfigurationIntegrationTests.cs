using RGroupConstruction.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace RGroupConstruction.IntegrationTests.Logging;

public class SerilogLoggingConfigurationIntegrationTests
{
    [Theory]
    [InlineData("RGroupConstruction.API/appsettings.json")]
    [InlineData("RGroupConstruction.API/appsettings.Development.json")]
    [InlineData("RGroupConstruction.Migrations/appsettings.json")]
    [InlineData("RGroupConstruction.Migrations/appsettings.Development.json")]
    public void AppsettingsFilesDeclareConfiguredSerilogSection(string relativePath)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(FindRepositoryRoot())
            .AddJsonFile(relativePath, optional: false)
            .Build();

        var sectionName = configuration["Logging:SerilogSection"];

        Assert.False(string.IsNullOrWhiteSpace(sectionName));
        Assert.True(configuration.GetSection(sectionName).Exists());
        Assert.False(string.IsNullOrWhiteSpace(configuration[$"{sectionName}:MinimumLevel"]));
        Assert.False(string.IsNullOrWhiteSpace(configuration[$"{sectionName}:ConnectionStringName"]));
        Assert.False(string.IsNullOrWhiteSpace(configuration[$"{sectionName}:TableName"]));
    }

    [Fact]
    public void AddSerilogLoggingThrowsWhenSerilogSectionNameIsMissing()
    {
        var builder = CreateBuilder(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Default"] = "Server=localhost;Database=RGroupConstruction;Uid=test;Pwd=test"
        });

        var exception = Assert.Throws<InvalidOperationException>(() => builder.AddSerilogLogging());

        Assert.Equal("Configuration key 'Logging:SerilogSection' is not configured.", exception.Message);
    }

    [Fact]
    public void AddSerilogLoggingReadsSerilogSectionNameFromConfiguration()
    {
        var builder = CreateBuilder(new Dictionary<string, string?>
        {
            ["Logging:SerilogSection"] = "CustomSerilog",
            ["CustomSerilog:ApplicationName"] = "RGroupConstruction.Tests",
            ["CustomSerilog:MinimumLevel"] = "Debug",
            ["CustomSerilog:MicrosoftLevel"] = "Error",
            ["CustomSerilog:SystemLevel"] = "Warning",
            ["CustomSerilog:ConnectionStringName"] = "Default",
            ["CustomSerilog:TableName"] = "IntegrationLogs",
            ["CustomSerilog:StoreTimestampInUtc"] = "true",
            ["CustomSerilog:OutputTemplate"] = "{Message:lj}{NewLine}{Exception}",
            ["ConnectionStrings:Default"] = "Server=localhost;Database=RGroupConstruction;Uid=test;Pwd=test"
        });

        builder.AddSerilogLogging();
    }

    private static WebApplicationBuilder CreateBuilder(Dictionary<string, string?> values)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Testing"
        });

        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(values);

        return builder;
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "RGroupConstruction.slnx")))
        {
            directory = directory.Parent;
        }

        if (directory is null)
            throw new DirectoryNotFoundException("Could not find repository root from test output directory.");

        return directory.FullName;
    }
}

