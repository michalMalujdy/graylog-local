using GraylogInitializer.Console.Configurations;
using GraylogInitializer.Console.Dtos;
using GraylogInitializer.Console.Services.Abstraction;

namespace GraylogInitializer.Console.Services.Implementation;

public class DtoFactory : IDtoFactory
{
    public string GelfInputType => "org.graylog2.inputs.gelf.http.GELFHttpInput";
    public int GelfInputPort => 5050;
    public string BeatsInputType => "org.graylog.plugins.beats.Beats2Input";
    public int BeatsInputPort => 5045;

    public CreateInputDto GetGelfInputDto()
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

    public CreateInputDto GetBeatsInputDto()
        => new()
        {
            Title = "Beats",
            Global = true,
            Type = BeatsInputType,
            Configuration = new CreateInputDto.ConfigurationDto
            {
                BindAddress = "0.0.0.0",
                EnableCors = true,
                IdleWriterTimeout = 60,
                NumberOfWorkingThreads = 2,
                Port = BeatsInputPort,
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

    public CreateStreamDto GetStreamDto(StreamConfiguration streamConfiguration, string indexId)
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
}