using System.Net.Http.Json;
using System.Text.Json;
using GraylogInitializer.Console.Configurations;
using GraylogInitializer.Console.Dtos;
using GraylogInitializer.Console.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IHttpClientFactory = GraylogInitializer.Console.Services.Abstraction.IHttpClientFactory;

namespace GraylogInitializer.Console.Services.Implementation;

public class StreamService : IStreamService
{
    private readonly HttpClient _httpClient;
    private readonly IDtoFactory _dtoFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StreamService> _logger;

    public StreamService(
        IHttpClientFactory httpClientFactory,
        IDtoFactory dtoFactory,
        IConfiguration configuration,
        ILogger<StreamService> logger)
    {
        _httpClient = httpClientFactory.GetHttpClient();
        _dtoFactory = dtoFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task EnsureStreams()
    {
        var defaultIndex = await GetDefaultIndex();
        var configStreams = GetStreamConfigurations();

        var existingStreams = await _httpClient.GetFromJsonAsync<GetStreamsDto>("/api/streams");
        _logger.LogInformation("Existing streams: {@ExistingStreams}", existingStreams?.Streams.Select(x => x.Title));

        var streamsToCreate = configStreams.Where(x => existingStreams!.Streams.All(y => y.Title != x.Title));
        _logger.LogInformation("Streams to create: {@StreamsToCreate}", streamsToCreate.Select(x => x.Title));

        var createdStreamIds = await CreateStreams(streamsToCreate, defaultIndex);
        await Task.Delay(5000);
        await StartCreatedStreams(createdStreamIds);

        _logger.LogInformation("Streams setup correctly");
    }
    private async Task<GetIndicesDto.IndexDto> GetDefaultIndex()
    {
        var existingIndices = await _httpClient.GetFromJsonAsync<GetIndicesDto>("/api/system/indices/index_sets");
        var getIndexDto = existingIndices!.Indices.First(x => x.Default);

        return getIndexDto;
    }

    private List<StreamConfiguration> GetStreamConfigurations()
    {
        var streamConfigurations = new List<StreamConfiguration>();
        _configuration.Bind("Streams", streamConfigurations);
        streamConfigurations = streamConfigurations.Where(x => x.Enabled).ToList();

        return streamConfigurations;
    }

    private async Task<List<string>> CreateStreams(
        IEnumerable<StreamConfiguration> streamsToCreate,
        GetIndicesDto.IndexDto defaultIndex)
    {
        var createdStreamsIds = new List<string>();

        foreach (var streamToCreate in streamsToCreate)
        {
            var streamDto = _dtoFactory.GetStreamDto(streamToCreate, defaultIndex.Id);
            var response = await _httpClient.PostAsJsonAsync("/api/streams", streamDto);
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdStream = JsonSerializer.Deserialize<CreateStreamDto.Response>(responseContent);

            createdStreamsIds.Add(createdStream!.StreamId);
        }

        return createdStreamsIds;
    }

    private async Task StartCreatedStreams(IEnumerable<string> createdStreamIds)
    {
        foreach (var createdStreamId in createdStreamIds)
        {
            await _httpClient.PostAsync($"/api/streams/{createdStreamId}/resume", null);
        }
    }
}