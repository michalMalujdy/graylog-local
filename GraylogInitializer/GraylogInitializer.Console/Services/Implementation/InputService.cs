using System.Net.Http.Json;
using GraylogInitializer.Console.Dtos;
using GraylogInitializer.Console.Services.Abstraction;
using Microsoft.Extensions.Logging;
using IHttpClientFactory = GraylogInitializer.Console.Services.Abstraction.IHttpClientFactory;

namespace GraylogInitializer.Console.Services.Implementation;

public class InputService : IInputService
{
    private readonly HttpClient _httpClient;
    private readonly IDtoFactory _dtoFactory;
    private readonly ILogger<InputService> _logger;

    public InputService(
        IHttpClientFactory httpClientFactory,
        ILogger<InputService> logger,
        IDtoFactory dtoFactory)
    {
        _httpClient = httpClientFactory.GetHttpClient();
        _logger = logger;
        _dtoFactory = dtoFactory;
    }

    public async Task EnsureInputs()
    {
        var existingInputs = await _httpClient.GetFromJsonAsync<GetInputsDto>("/api/system/inputs");

        if (!IsInputPresent(existingInputs, _dtoFactory.GelfInputType, _dtoFactory.GelfInputPort))
        {
            _logger.LogInformation("GELF input not present, creating one");
            var gelfCreateInputDto = _dtoFactory.GetGelfInputDto();
            await _httpClient.PostAsJsonAsync("/api/system/inputs", gelfCreateInputDto);
        }

        if (!IsInputPresent(existingInputs, _dtoFactory.BeatsInputType, _dtoFactory.BeatsInputPort))
        {
            _logger.LogInformation("Beats input not present, creating one");
            var beatsCreateInputDto = _dtoFactory.GetBeatsInputDto();
            await _httpClient.PostAsJsonAsync("/api/system/inputs", beatsCreateInputDto);
        }

        _logger.LogInformation("Inputs setup correctly");
    }

    private static bool IsInputPresent(GetInputsDto? existingInputs, string type, int port)
        => existingInputs!.Inputs
            .Any(x => x.Type == type && x.Attributes.Port == port);
}