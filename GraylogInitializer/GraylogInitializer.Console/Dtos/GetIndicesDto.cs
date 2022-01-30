using System.Text.Json.Serialization;

namespace GraylogInitializer.Console.Dtos;

public class GetIndicesDto
{
    [JsonPropertyName("index_sets")]
    public List<IndexDto> Indices { get; set; }

    public class IndexDto
    {
        public string Id { get; set; } = default!;
        public bool Default { get; set; }
    }
}