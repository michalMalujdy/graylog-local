using System.Net.Http.Json;
using System.Net.Http.Json;
using System.Text.Json;
using GraylogInitializer.Console.Configurations;
using GraylogInitializer.Console.Dtos;
using GraylogInitializer.Console.Dtos.GetInputs;
using Microsoft.Extensions.Configuration;

namespace GraylogInitializer.Console;

public class GraylogApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private const string GelfInputType = "org.graylog2.inputs.gelf.http.GELFHttpInput";
    private const int GelfInputPort = 5050;

    public GraylogApiService(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = SetupHttpClient();
    }

    public async Task EnsureInputs()
    {
        var existingInputs = await _httpClient.GetFromJsonAsync<GetInputsDto>("/api/system/inputs");

        if (!IsInputPresent(existingInputs, GelfInputType, GelfInputPort))
        {
            var gelfCreateInputDto = GetCreateGelfInputDto();
            await _httpClient.PostAsJsonAsync("/api/system/inputs", gelfCreateInputDto);
        }
    }

    public async Task EnsureStreams()
    {
        var defaultIndex = await GetDefaultIndex();
        var configStreams = GetStreamConfigurations();

        var existingStreams = await _httpClient.GetFromJsonAsync<GetStreamsDto>("/api/streams");
        var streamsToCreate = configStreams.Where(x => existingStreams!.Streams.All(y => y.Title != x.Title));

        var createdStreamIds = await CreateStreams(streamsToCreate, defaultIndex);
        await Task.Delay(5000);
        await StartCreatedStreams(createdStreamIds);
    }

    private HttpClient SetupHttpClient()
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

    private static bool IsInputPresent(GetInputsDto? existingInputs, string type, int port)
        => existingInputs!.Inputs
            .Any(x => x.Type == type && x.Attributes.Port == port);

    private static CreateInputDto GetCreateGelfInputDto()
        => new()
        {
            Title = "GELF HTTP",
            Global = true,
            Type = GelfInputType,
            Configuration = new CreateInputDto.ConfigurationDto
            {
                BindAddress = "0.0.0.0",
                DecompressSizeLimit = 8388608,
                EnableCors = true,
                IdleWriterTimeout = 60,
                MaxChunkSize = 65536,
                NumberOfWorkingThreads = 2,
                Port = GelfInputPort,
                ReceiveBufferSize = 1048576,
                TcpKeepAlive = true,
                TlsEnable = false,
                TlsCertFile = "",
                TlsClientAuth = "disabled",
                TlsClientAuthCertFile = "",
                TlsKeyFile = "",
                TlsKeyPassword = ""
            }
        };

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

    private async Task<List<string>> CreateStreams(IEnumerable<StreamConfiguration> streamsToCreate, GetIndicesDto.IndexDto defaultIndex)
    {
        var createdStreamsIds = new List<string>();

        foreach (var streamToCreate in streamsToCreate)
        {
            var streamDto = GetCreateStreamDto(streamToCreate, defaultIndex.Id);

            var response = await _httpClient.PostAsJsonAsync("/api/streams", streamDto);
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdStream = JsonSerializer.Deserialize<CreateStreamDto.Response>(responseContent);

            createdStreamsIds.Add(createdStream!.StreamId);
        }

        return createdStreamsIds;
    }

    private CreateStreamDto GetCreateStreamDto(StreamConfiguration streamConfiguration, string indexId)
        => new()
        {
            Title = streamConfiguration.Title,
            MatchingType = "OR",
            RemoveMatchesFromDefaultStream = true,
            IndexSetId = indexId,
            Rules = new List<CreateStreamDto.RuleDto>
            {
                new()
                {
                    Type = 1,
                    Value = streamConfiguration.ApplicationName,
                    Field = "applicationName",
                    Inverted = false
                },
                new()
                {
                    Type = 1,
                    Value = streamConfiguration.ApplicationName,
                    Field = "fields_applicationName",
                    Inverted = false
                }
            }
        };

    private async Task StartCreatedStreams(IEnumerable<string> createdStreamIds)
    {
        foreach (var createdStreamId in createdStreamIds)
        {
            await _httpClient.PostAsync($"/api/streams/{createdStreamId}/resume", null);
        }
    }
}