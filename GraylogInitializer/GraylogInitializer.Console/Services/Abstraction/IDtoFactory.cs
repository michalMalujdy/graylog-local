using GraylogInitializer.Console.Configurations;
using GraylogInitializer.Console.Dtos;

namespace GraylogInitializer.Console.Services.Abstraction;

public interface IDtoFactory
{
    public string GelfInputType { get; }
    public int GelfInputPort { get; }
    public string BeatsInputType { get; }
    public int BeatsInputPort { get; }

    CreateInputDto GetGelfInputDto();
    CreateInputDto GetBeatsInputDto();
    CreateStreamDto GetStreamDto(StreamConfiguration streamConfiguration, string indexId);
}