using System.ComponentModel.DataAnnotations;

namespace QuisoLab.Observability.Elastic.Configuration;

/// <summary>
///     Configurações para o Elastic APM Manager.
/// </summary>
public class ElasticConfiguration
{
    /// <summary>
    ///     Nome do serviço no Elastic APM.
    /// </summary>
    [Required]
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    ///     Versão do serviço.
    /// </summary>
    public string ServiceVersion { get; set; } = "1.0.0";

    /// <summary>
    ///     Ambiente (development, staging, production).
    /// </summary>
    public string Environment { get; set; } = "development";

    /// <summary>
    ///     URL do servidor Elastic APM.
    /// </summary>
    public string ServerUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Token de autenticação.
    /// </summary>
    public string SecretToken { get; set; } = string.Empty;

    /// <summary>
    ///     Chave da API.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    ///     Taxa de amostragem (0.0 a 1.0).
    /// </summary>
    [Range(0.0, 1.0)]
    public double TransactionSampleRate { get; set; } = 1.0;

    /// <summary>
    ///     Máximo de spans por transação.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int TransactionMaxSpans { get; set; } = 500;

    /// <summary>
    ///     Capturar headers de requisição.
    /// </summary>
    public bool CaptureHeaders { get; set; } = true;

    /// <summary>
    ///     Capturar body de requisição.
    /// </summary>
    public bool CaptureBody { get; set; } = false;

    /// <summary>
    ///     Timeout para flush de dados (em segundos).
    /// </summary>
    [Range(1, 300)]
    public int FlushInterval { get; set; } = 10;

    /// <summary>
    ///     Nível de log do Elastic APM.
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;

    /// <summary>
    ///     Se deve registrar logs do próprio APM.
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    ///     Metadados globais para todas as transações.
    /// </summary>
    public Dictionary<string, string> GlobalLabels { get; set; } = [];

    /// <summary>
    ///     Lista de URLs a serem ignoradas pelo APM.
    /// </summary>
    public List<string> IgnoreUrls { get; set; } = [];

    /// <summary>
    ///     Se deve capturar SQL statements.
    /// </summary>
    public bool CaptureSqlStatements { get; set; } = true;

    /// <summary>
    ///     Se deve sanitizar campos sensíveis.
    /// </summary>
    public bool SanitizeFieldNames { get; set; } = true;

    /// <summary>
    ///     Lista de campos a serem sanitizados.
    /// </summary>
    public List<string> SanitizeFields { get; set; } =
    [
        "password", "passwd", "pwd", "secret", "key", "token", "authorization", "cookie"
    ];
}
