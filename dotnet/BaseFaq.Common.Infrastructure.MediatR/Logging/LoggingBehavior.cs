using System.Diagnostics;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BaseFaq.Common.Infrastructure.MediatR.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        var requestGuid = Guid.NewGuid().ToString();

        var requestNameWithGuid = $"{requestName} [{requestGuid}]";

        _logger.LogInformation("===================================================================================");
        _logger.LogInformation($"[START] Handling {requestNameWithGuid}");

        var stopwatch = Stopwatch.StartNew();
        TResponse response;

        try
        {
            try
            {
                _logger.LogDebug("===================================================================================");
                _logger.LogDebug($"Handling {requestNameWithGuid} - Parameters: {JsonSerializer.Serialize(request)}");
                _logger.LogDebug("===================================================================================");
            }
            catch (NotSupportedException)
            {
                _logger.LogWarning($"[Serialization ERROR] {requestNameWithGuid} Could not serialize the request.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    $"[Serialization ERROR] {requestNameWithGuid} Could not serialize the request with error: {ex.Message}.",
                    ex);
            }


            response = await next(); // Execution moves to the handler

            // Log the response details
            try
            {
                _logger.LogDebug("===================================================================================");
                _logger.LogDebug($"Handled Response: {JsonSerializer.Serialize(response)}");
                _logger.LogDebug("===================================================================================");
            }
            catch (NotSupportedException)
            {
                _logger.LogWarning($"[Serialization ERROR] {requestNameWithGuid} Could not serialize the response.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    $"[Serialization ERROR] {requestNameWithGuid} Could not serialize the response with error: {ex.Message}",
                    ex);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("===================================================================================");
            _logger.LogError(ex, $"Error handling {requestNameWithGuid} - Exception: {ex.Message}");
            _logger.LogError("===================================================================================");


            throw; // Propagate the exception
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation($"[END] Handled {requestNameWithGuid} in {stopwatch.Elapsed.TotalMilliseconds} ms");
            _logger.LogInformation(
                "===================================================================================");
        }

        return response;
    }
}