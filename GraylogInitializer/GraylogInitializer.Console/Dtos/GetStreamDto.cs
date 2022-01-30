namespace GraylogInitializer.Console.Dtos;

public class GetStreamDto : CreateStreamDto
{
    public string Id { get; set; } = default!;
    public bool Default { get; set; }
}