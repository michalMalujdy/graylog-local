namespace GraylogInitializer.Console.Services;

public interface IGraylogApiService
{
    Task EnsureInputs();
    Task EnsureStreams();
}