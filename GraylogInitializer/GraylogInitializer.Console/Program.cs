using GraylogInitializer.Console;
using Microsoft.Extensions.Configuration;
using Serilog;

var configuration = BuildConfiguration();
var graylogApiService = new GraylogApiService(configuration);
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var retryCount = 0;
const int maxRetries = 15;

while (retryCount < maxRetries)
{
    try
    {
        Log.Debug("Starting the Graylog initialization");
        await graylogApiService.EnsureInputs();
        await graylogApiService.EnsureStreams();
        break;
    }
    catch (HttpRequestException e)
    {
        Log.Error("Error: {error}", e.Message);

        retryCount++;
        await Task.Delay(5000);
    }
}

IConfiguration BuildConfiguration()
    => new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
        .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true)
        .AddEnvironmentVariables()
        .Build();
