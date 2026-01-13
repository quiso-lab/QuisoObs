using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using QuisoLab.Observability.Elastic.Configuration;
using Microsoft.Extensions.Logging;

namespace QuisoLab.Observability.Elastic;

/// <summary>
///     Classe de extensão responsável por registrar os serviços do Elastic Manager.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Startup
{
    /// <summary>
    ///     Registra todos os serviços necessários para habilitar o Elastic APM na aplicação.
    ///     Inclui a configuração do agente Elastic via <c>AddAllElasticApm</c>
    ///     e a injeção da abstração <see cref="IElasticTransaction" /> com a implementação <see cref="ElasticTransaction" />.
    /// </summary>
    /// <param name="services">Instância de <see cref="IServiceCollection" /> da aplicação.</param>
    public static IServiceCollection ConfigureElasticServices(this IServiceCollection services)
    {
        services.AddAllElasticApm();
        services.AddScoped<IElasticTransaction, ElasticTransaction>();
        return services;
    }

    /// <summary>
    ///     Registra os serviços do Elastic APM com configuração customizada.
    /// </summary>
    /// <param name="services">Instância de <see cref="IServiceCollection" /> da aplicação.</param>
    /// <param name="configuration">Configuração da aplicação.</param>
    /// <param name="sectionName">Nome da seção de configuração (padrão: "ElasticApm").</param>
    public static IServiceCollection ConfigureElasticServices(
        this IServiceCollection services, 
        IConfiguration configuration, 
        string sectionName = "ElasticApm")
    {
        // Registra a configuração
        services.Configure<ElasticConfiguration>(configuration.GetSection(sectionName));
        
        // Registra os serviços
        services.AddAllElasticApm();
        services.AddScoped<IElasticTransaction, ElasticTransaction>();
        
        return services;
    }

    /// <summary>
    ///     Registra os serviços do Elastic APM com configuração via delegate.
    /// </summary>
    /// <param name="services">Instância de <see cref="IServiceCollection" /> da aplicação.</param>
    /// <param name="configureOptions">Delegate para configuração.</param>
    public static IServiceCollection ConfigureElasticServices(
        this IServiceCollection services,
        Action<ElasticConfiguration> configureOptions)
    {
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        services.Configure(configureOptions);
        services.AddAllElasticApm();
        services.AddScoped<IElasticTransaction, ElasticTransaction>();
        
        return services;
    }

    /// <summary>
    ///     Registra apenas os serviços básicos do Elastic Transaction sem configuração automática do APM.
    ///     Útil quando o APM já está configurado externamente.
    /// </summary>
    /// <param name="services">Instância de <see cref="IServiceCollection" /> da aplicação.</param>
    public static IServiceCollection AddElasticTransaction(this IServiceCollection services)
    {
        services.AddScoped<IElasticTransaction, ElasticTransaction>();
        return services;
    }

    /// <summary>
    ///     Valida se a configuração do Elastic APM está correta.
    /// </summary>
    /// <param name="configuration">Configuração a ser validada.</param>
    /// <param name="logger">Logger para registrar problemas de validação.</param>
    /// <returns>True se a configuração é válida, false caso contrário.</returns>
    public static bool ValidateElasticConfiguration(ElasticConfiguration configuration, ILogger logger = null)
    {
        var isValid = true;

        if (string.IsNullOrWhiteSpace(configuration.ServiceName))
        {
            logger?.LogError("ElasticConfiguration.ServiceName is required but not provided");
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(configuration.ServerUrl) && 
            string.IsNullOrWhiteSpace(configuration.SecretToken) && 
            string.IsNullOrWhiteSpace(configuration.ApiKey))
        {
            logger?.LogWarning("ElasticConfiguration does not have ServerUrl, SecretToken, or ApiKey configured. APM may not work properly.");
        }

        if (configuration.TransactionSampleRate < 0 || configuration.TransactionSampleRate > 1)
        {
            logger?.LogError("ElasticConfiguration.TransactionSampleRate must be between 0.0 and 1.0, but was {SampleRate}", configuration.TransactionSampleRate);
            isValid = false;
        }

        if (configuration.TransactionMaxSpans <= 0)
        {
            logger?.LogError("ElasticConfiguration.TransactionMaxSpans must be greater than 0, but was {MaxSpans}", configuration.TransactionMaxSpans);
            isValid = false;
        }

        if (configuration.FlushInterval <= 0 || configuration.FlushInterval > 300)
        {
            logger?.LogError("ElasticConfiguration.FlushInterval must be between 1 and 300 seconds, but was {FlushInterval}", configuration.FlushInterval);
            isValid = false;
        }

        return isValid;
    }
}