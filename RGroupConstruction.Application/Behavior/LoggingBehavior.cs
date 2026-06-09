using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace RGroupConstruction.Application.Behavior;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> _logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.Info("Handling {RequestName}", requestName);

        var sw = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);
            sw.Stop();

            _logger.Info("Handled {RequestName} in {ElapsedMilliseconds}ms", requestName, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();

            _logger.Error(ex, "Error handling {RequestName} after {ElapsedMilliseconds}ms", requestName, sw.ElapsedMilliseconds);

            throw;
        }
    }
}

