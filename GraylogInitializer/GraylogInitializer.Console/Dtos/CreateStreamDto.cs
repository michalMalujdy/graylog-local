using System.Text.Json.Serialization;

namespace GraylogInitializer.Console.Dtos;

public class CreateStreamDto
{
    public string Title { get; set; } = default!;
    public List<RuleDto> Rules { get; set; } = new();

    [JsonPropertyName("matching_type")]
    public string MatchingType { get; set; } = default!;

    [JsonPropertyName("remove_matches_from_default_stream")]
    public bool RemoveMatchesFromDefaultStream { get; set; }

    [JsonPropertyName("index_set_id")]
    public string IndexSetId { get; set; } = default!;

    public class RuleDto
    {
        public int Type { get; set; }
        public string Field { get; set; } = default!;
        public string Value { get; set; } = default!;
        public bool Inverted { get; set; }
    }

    public class Response
    {
        [JsonPropertyName("stream_id")]
        public string StreamId { get; set; }
    }
}