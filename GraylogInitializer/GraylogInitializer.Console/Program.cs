using GraylogInitializer.Console.Services.Abstraction;
using GraylogInitializer.Console.Services.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using IHttpClientFactory = GraylogInitializer.Console.Services.Abstraction.IHttpClientFactory;

using var host = Host.CreateDefaultBuilder()
    .ConfigureServices(SetupServices)
    .UseSerilog()
    .Build();

Log.Logger = BuildLogger();

await host.Services
    .GetRequiredService<IGraylogInitializerService>()
    .InitializeGraylog();

Logger BuildLogger()
    => new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateLogger();

void SetupServices(IServiceCollection services)
{
    services.AddSingleton(BuildConfiguration());
    services.AddScoped<IGraylogInitializerService, GraylogInitializerService>();
    services.AddScoped<IInputService, InputService>();
    services.AddScoped<IStreamService, StreamService>();
    services.AddScoped<IDtoFactory, DtoFactory>();
    services.AddScoped<IHttpClientFactory, HttpClientFactory>();
}

IConfiguration BuildConfiguration()
    => new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
        .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true)
        .AddEnvironmentVariables()
        .Build();
