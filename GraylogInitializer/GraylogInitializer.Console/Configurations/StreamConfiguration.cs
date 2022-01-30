namespace GraylogInitializer.Console.Configurations;

public class StreamConfiguration
{
    public string Title { get; set; } = default!;
    public string ApplicationName { get; set; } = default!;
    public bool Enabled { get; set; }
}