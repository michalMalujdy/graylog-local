namespace GraylogInitializer.Console.Services.Abstraction;

public interface IStreamService
{
    Task EnsureStreams();
}