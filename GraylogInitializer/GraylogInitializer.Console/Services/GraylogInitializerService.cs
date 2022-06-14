using Microsoft.Extensions.Logging;

namespace GraylogInitializer.Console.Services;

public class GraylogInitializerService : IGraylogInitializerService
{
    private readonly IGraylogApiService _graylogApiService;
    private readonly ILogger<GraylogInitializerService> _logger;

    private const int MaxRetries = 15;
    private const int DelayInMilliseconds = 5000;

    public GraylogInitializerService(
        IGraylogApiService graylogApiService,
        ILogger<GraylogInitializerService> logger)
    {
        _graylogApiService = graylogApiService;
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

                await _graylogApiService.EnsureInputs();
                await _graylogApiService.EnsureStreams();

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