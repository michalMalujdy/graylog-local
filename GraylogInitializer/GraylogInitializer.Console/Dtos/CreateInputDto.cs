using System.Text.Json.Serialization;

namespace GraylogInitializer.Console.Dtos;

public class CreateInputDto
{
    public string Title { get; set; }
    public string Type { get; set; }
    public bool Global { get; set; }
    public ConfigurationDto Configuration { get; set; }
    public string Node { get; set; }

    public class ConfigurationDto
    {
        [JsonPropertyName("bind_address")]
        public string BindAddress { get; set; }

        [JsonPropertyName("decompress_size_limit")]
        public int DecompressSizeLimit { get; set; }

        [JsonPropertyName("enable_cors")]
        public bool EnableCors { get; set; }

        [JsonPropertyName("idle_writer_timeout")]
        public int IdleWriterTimeout { get; set; }

        [JsonPropertyName("max_chunk_size")]
        public int MaxChunkSize { get; set; }

        [JsonPropertyName("number_worker_threads")]
        public int NumberOfWorkingThreads { get; set; }

        [JsonPropertyName("override_source")]
        public object OverrideSource { get; set; }

        public int Port { get; set; }

        [JsonPropertyName("recv_buffer_size")]
        public int ReceiveBufferSize { get; set; }

        [JsonPropertyName("tcp_keepalive")]
        public bool TcpKeepAlive { get; set; }

        [JsonPropertyName("tls_cert_file")]
        public string TlsCertFile { get; set; }

        [JsonPropertyName("tls_client_auth")]
        public string TlsClientAuth { get; set; }

        [JsonPropertyName("tls_client_auth_cert_file")]
        public string TlsClientAuthCertFile { get; set; }

        [JsonPropertyName("tls_enable")]
        public bool TlsEnable { get; set; }

        [JsonPropertyName("tls_key_file")]
        public string TlsKeyFile { get; set; }

        [JsonPropertyName("tls_key_password")]
        public string TlsKeyPassword { get; set; }
    }
}