namespace GraylogInitializer.Console.Dtos;

public class GetStreamsDto
{
    public int Total { get; set; }
    public List<GetStreamDto> Streams { get; set; } = new();
}