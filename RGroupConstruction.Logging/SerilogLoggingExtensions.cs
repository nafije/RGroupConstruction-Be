using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System.Security.Claims;

namespace RGroupConstruction.Logging;

public static class SerilogLoggingExtensions
{
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        var serilogSection = builder.Configuration["Logging:SerilogSection"];
        if (string.IsNullOrWhiteSpace(serilogSection))
            throw new InvalidOperationException("Configuration key 'Logging:SerilogSection' is not configured.");

        var options = builder.Configuration.GetSection(serilogSection).Get<SerilogLoggingOptions>()
            ?? new SerilogLoggingOptions();

        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .MinimumLevel.Is(ParseLevel(options.MinimumLevel, LogEventLevel.Information))
                .MinimumLevel.Override("Microsoft", ParseLevel(options.MicrosoftLevel, LogEventLevel.Warning))
                .MinimumLevel.Override("Microsoft.AspNetCore", ParseLevel(options.MicrosoftLevel, LogEventLevel.Warning))
                .MinimumLevel.Override("System", ParseLevel(options.SystemLevel, LogEventLevel.Warning))
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", options.ApplicationName)
                .WriteTo.Console(outputTemplate: options.OutputTemplate);

            var connectionString = context.Configuration.GetConnectionString(options.ConnectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    $"Connection string '{options.ConnectionStringName}' is not configured for Serilog.");

            loggerConfiguration.WriteTo.MySQL(
                connectionString,
                options.TableName,
                storeTimestampInUtc: options.StoreTimestampInUtc);
        });

        return builder;
    }

    public static IApplicationBuilder UseConfiguredSerilogRequestLogging(this IApplicationBuilder app)
    {
        return app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            options.GetLevel = (httpContext, _, exception) =>
            {
                if (exception is not null || httpContext.Response.StatusCode >= 500)
                    return LogEventLevel.Error;

                return httpContext.Response.StatusCode >= 400
                    ? LogEventLevel.Warning
                    : LogEventLevel.Information;
            };

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
                diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());

                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                    diagnosticContext.Set("UserId", userId);
            };
        });
    }

    private static LogEventLevel ParseLevel(string? value, LogEventLevel fallback)
        => Enum.TryParse<LogEventLevel>(value, ignoreCase: true, out var level)
            ? level
            : fallback;
}

