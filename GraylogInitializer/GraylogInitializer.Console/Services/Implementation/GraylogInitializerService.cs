using GraylogInitializer.Console.Services.Abstraction;
using Microsoft.Extensions.Logging;

namespace GraylogInitializer.Console.Services.Implementation;

public class GraylogInitializerService : IGraylogInitializerService
{
    private readonly IInputService _inputService;
    private readonly IStreamService _streamService;
    private readonly ILogger<GraylogInitializerService> _logger;

    private const int MaxRetries = 15;
    private const int DelayInMilliseconds = 5000;

    public GraylogInitializerService(
        IInputService inputService,
        IStreamService streamService,
        ILogger<GraylogInitializerService> logger)
    {
        _inputService = inputService;
        _streamService = streamService;
        _logger = logger;
    }

    public async Task InitializeGraylog()
    {
        var retryCount = 0;

        while (retryCount < MaxRetries)
        {
            try
            {
                _logger.LogInformation("Trying to initialize Graylog, try number: {TryNumber}", retryCount + 1);

                await _inputService.EnsureInputs();
                await _streamService.EnsureStreams();

                break;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error: {Error}", e.Message);

                retryCount++;
                await Task.Delay(DelayInMilliseconds);
            }
            catch (Exception e)
            {
                _logger.LogError("Unhandled error: {Error}", e);
                _logger.LogError("Shutting down");
            }
        }
    }
}