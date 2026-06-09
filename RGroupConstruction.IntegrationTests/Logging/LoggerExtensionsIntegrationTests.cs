using RGroupConstruction.Logging;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace RGroupConstruction.IntegrationTests.Logging;

public class LoggerExtensionsIntegrationTests
{
    [Theory]
    [InlineData(LogLevel.Debug, 1, "Debug")]
    [InlineData(LogLevel.Information, 2, "Info")]
    [InlineData(LogLevel.Warning, 3, "Warn")]
    [InlineData(LogLevel.Error, 4, "Error")]
    public void LoggingExtensionsEmitEnumBackedEventIds(LogLevel level, int expectedId, string expectedName)
    {
        var logger = new CapturingLogger();
        var exception = new InvalidOperationException("boom");

        switch (level)
        {
            case LogLevel.Debug:
                logger.Debug("Debug message {Value}", 10);
                break;
            case LogLevel.Information:
                logger.Info("Info message {Value}", 20);
                break;
            case LogLevel.Warning:
                logger.Warn("Warn message {Value}", 30);
                break;
            case LogLevel.Error:
                logger.Error(exception, "Error message {Value}", 40);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(level, entry.Level);
        Assert.Equal(expectedId, entry.EventId.Id);
        Assert.Equal(expectedName, entry.EventId.Name);

        if (level == LogLevel.Error)
            Assert.Same(exception, entry.Exception);
        else
            Assert.Null(entry.Exception);
    }

    [Fact]
    public void LoggingExtensionsRenderMainMessageAndPreserveTemplatePropertiesInScope()
    {
        var logger = new CapturingLogger();

        logger.Info("Created project {ProjectId} for {ProjectName}", 42, "ABV");

        var entry = Assert.Single(logger.Entries);

        Assert.Equal("Created project 42 for \"ABV\"", entry.Message);
        Assert.DoesNotContain("{ProjectId}", entry.Message);
        Assert.DoesNotContain("{ProjectName}", entry.Message);
        Assert.Contains(entry.ScopeProperties, property => property.Key == "ProjectId" && (int)property.Value! == 42);
        Assert.Contains(entry.ScopeProperties, property => property.Key == "ProjectName" && (string)property.Value! == "ABV");
    }

    [Fact]
    public void LoggingExtensionsSendRenderedMessageAndStructuredScopePropertiesToSerilog()
    {
        var sink = new CapturingSerilogSink();
        using var serilogLogger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Sink(sink)
            .CreateLogger();
        using var provider = new SerilogLoggerProvider(serilogLogger);
        var logger = provider.CreateLogger("RoleSeeder");

        logger.Info("Role {RoleName} already exists, skipping seed", "Admin");

        var logEvent = Assert.Single(sink.Events);
        Assert.Equal("Role \"Admin\" already exists, skipping seed", logEvent.MessageTemplate.Text);
        Assert.DoesNotContain("{RoleName}", logEvent.MessageTemplate.Text);

        var roleName = Assert.IsType<ScalarValue>(logEvent.Properties["RoleName"]);
        Assert.Equal("Admin", roleName.Value);
    }

    private sealed class CapturingLogger : Microsoft.Extensions.Logging.ILogger
    {
        private Dictionary<string, object?>? _currentScopeProperties;

        public List<LogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            if (state is IEnumerable<KeyValuePair<string, object?>> properties)
                _currentScopeProperties = properties.ToDictionary(property => property.Key, property => property.Value);

            return new CapturingScope(() => _currentScopeProperties = null);
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add(new LogEntry(
                logLevel,
                eventId,
                state,
                exception,
                formatter(state, exception),
                new Dictionary<string, object?>(_currentScopeProperties ?? [])));
        }
    }

    private sealed record LogEntry(
        LogLevel Level,
        EventId EventId,
        object? State,
        Exception? Exception,
        string Message,
        IReadOnlyDictionary<string, object?> ScopeProperties);

    private sealed class CapturingScope(Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }

    private sealed class CapturingSerilogSink : ILogEventSink
    {
        public List<LogEvent> Events { get; } = [];

        public void Emit(LogEvent logEvent)
        {
            Events.Add(logEvent);
        }
    }
}

