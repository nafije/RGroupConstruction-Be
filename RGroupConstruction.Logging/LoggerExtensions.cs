using Microsoft.Extensions.Logging;
using Serilog.Events;
using Serilog.Parsing;
using System.Globalization;

namespace RGroupConstruction.Logging;

public static class LoggerExtensions
{
    private static readonly EventId DebugEvent = ToEventId(LogEvent.Debug);
    private static readonly EventId InfoEvent = ToEventId(LogEvent.Info);
    private static readonly EventId WarnEvent = ToEventId(LogEvent.Warn);
    private static readonly EventId ErrorEvent = ToEventId(LogEvent.Error);
    private static readonly MessageTemplateParser MessageTemplateParser = new();

    public static void Debug(this ILogger logger, string messageTemplate, params object?[] args)
        => Write(logger, LogLevel.Debug, DebugEvent, null, messageTemplate, args);

    public static void Info(this ILogger logger, string messageTemplate, params object?[] args)
        => Write(logger, LogLevel.Information, InfoEvent, null, messageTemplate, args);

    public static void Warn(this ILogger logger, string messageTemplate, params object?[] args)
        => Write(logger, LogLevel.Warning, WarnEvent, null, messageTemplate, args);

    public static void Error(this ILogger logger, string messageTemplate, params object?[] args)
        => Write(logger, LogLevel.Error, ErrorEvent, null, messageTemplate, args);

    public static void Error(this ILogger logger, Exception exception, string messageTemplate, params object?[] args)
        => Write(logger, LogLevel.Error, ErrorEvent, exception, messageTemplate, args);

    public static void Error(this ILogger logger, string messageTemplate, Exception exception)
        => Error(logger, exception, messageTemplate);

    private static void Write(
        ILogger logger,
        LogLevel logLevel,
        EventId eventId,
        Exception? exception,
        string messageTemplate,
        params object?[] args)
    {
        if (!logger.IsEnabled(logLevel))
            return;

        var parsedTemplate = MessageTemplateParser.Parse(messageTemplate);
        var properties = GetTemplateProperties(parsedTemplate, args);
        var renderedMessage = RenderMessage(parsedTemplate, properties);

        using var scope = properties.Count > 0
            ? logger.BeginScope(properties)
            : null;

        Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, logLevel, eventId, exception, renderedMessage);
    }

    private static EventId ToEventId(LogEvent logEvent)
        => new((int)logEvent, logEvent.ToString());

    private static Dictionary<string, object?> GetTemplateProperties(MessageTemplate messageTemplate, object?[] args)
    {
        var properties = new Dictionary<string, object?>();
        var argumentIndex = 0;

        foreach (var token in messageTemplate.Tokens.OfType<PropertyToken>())
        {
            if (argumentIndex >= args.Length)
                break;

            properties.TryAdd(token.PropertyName, args[argumentIndex]);
            argumentIndex++;
        }

        return properties;
    }

    private static string RenderMessage(MessageTemplate messageTemplate, Dictionary<string, object?> properties)
    {
        if (properties.Count == 0)
            return messageTemplate.Text;

        var values = properties.ToDictionary(
            property => property.Key,
            property => (LogEventPropertyValue)new ScalarValue(property.Value));

        return messageTemplate.Render(values, CultureInfo.InvariantCulture);
    }

    private enum LogEvent
    {
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4
    }
}

