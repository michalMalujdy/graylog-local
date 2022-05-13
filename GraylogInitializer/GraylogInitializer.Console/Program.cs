using GraylogInitializer.Console;
using GraylogInitializer.Console;
using Microsoft.Extensions.Configuration;

var configuration = BuildConfiguration();
var graylogApiService = new GraylogApiService(configuration);

var retryCount = 0;
const int maxRetries = 15;

while (retryCount < maxRetries)
{
    try
    {
        await graylogApiService.EnsureInputs();
        await graylogApiService.EnsureStreams();
        break;
    }
    catch (HttpRequestException)
    {
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
