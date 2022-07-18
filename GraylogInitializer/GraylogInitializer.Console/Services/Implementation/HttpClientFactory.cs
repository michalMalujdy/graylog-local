using GraylogInitializer.Console.Configurations;
using Microsoft.Extensions.Configuration;
using IHttpClientFactory = GraylogInitializer.Console.Services.Abstraction.IHttpClientFactory;

namespace GraylogInitializer.Console.Services.Implementation;

public class HttpClientFactory : IHttpClientFactory
{
    private readonly IConfiguration _configuration;

    public HttpClientFactory(IConfiguration configuration)
        => _configuration = configuration;

    public HttpClient GetHttpClient()
    {
        var graylogApiConfiguration = GetGraylogApiConfiguration();

        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(graylogApiConfiguration.Url);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {graylogApiConfiguration.BasicAuthorization}");
        httpClient.DefaultRequestHeaders.Add("X-Requested-By", "XMLHttpRequest");
        httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

        return httpClient;
    }

    private GraylogApiConfiguration GetGraylogApiConfiguration()
    {
        var graylogApiConfiguration = new GraylogApiConfiguration();
        _configuration.Bind("GraylogApi", graylogApiConfiguration);

        return graylogApiConfiguration;
    }
}