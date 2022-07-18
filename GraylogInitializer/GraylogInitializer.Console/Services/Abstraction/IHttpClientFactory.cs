namespace GraylogInitializer.Console.Services.Abstraction;

public interface IHttpClientFactory
{
    HttpClient GetHttpClient();
}