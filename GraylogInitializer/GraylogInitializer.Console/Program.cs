using GraylogInitializer.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;

Log.Logger = BuildLogger();

using var host = Host.CreateDefaultBuilder()
    .ConfigureServices(SetupServices)
    .UseSerilog()
    .Build();

var graylogApiService = host.Services.GetRequiredService<IGraylogApiService>();

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
    catch (HttpRequestException e)
    {
        Log.Error("Error: {error}", e.Message);

        retryCount++;
        await Task.Delay(5000);
    }
}

Logger BuildLogger()
{
    return new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateLogger();
}

void SetupServices(IServiceCollection services)
{
    services.AddSingleton(BuildConfiguration());
    services.AddScoped<IGraylogApiService, GraylogApiService>();
}

IConfiguration BuildConfiguration()
    => new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
        .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true)
        .AddEnvironmentVariables()
        .Build();
