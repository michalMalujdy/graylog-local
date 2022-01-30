using GraylogInitializer.Console;
using Microsoft.Extensions.Configuration;

var configuration = BuildConfiguration();
var graylogApiService = new GraylogApiService(configuration);

await graylogApiService.EnsureInputs();
await graylogApiService.EnsureStreams();

IConfiguration BuildConfiguration()
    => new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
        .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true)
        .AddEnvironmentVariables()
        .Build();
