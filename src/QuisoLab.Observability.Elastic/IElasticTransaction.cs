using Elastic.Apm.Api;

namespace QuisoLab.Observability.Elastic;

/// <summary>
///     Interface para gerenciamento centralizado de transações com Elastic APM.
///     Permite iniciar, encerrar, enriquecer e monitorar transações e spans em aplicações distribuídas.
/// </summary>
public interface IElasticTransaction
{
    /// <summary>
    ///     Retorna a transação ativa atual, se houver.
    /// </summary>
    ITransaction CurrentTransaction { get; }

    /// <summary>
    ///     Inicia uma nova transação com um nome definido, tipo e tracing distribuído (se houver).
    ///     Se o tracing for inválido ou nulo, a transação é criada normalmente.
    /// </summary>
    /// <param name="name">Nome da transação visível no Elastic APM.</param>
    /// <param name="distributedTracingData">Dados do tracing (traceparent), geralmente oriundo de headers.</param>
    /// <param name="type">Tipo da transação (ex: messaging, request, external).</param>
    void StartTransaction(string name, string distributedTracingData, string type = ApiConstants.TypeMessaging);

    /// <summary>
    ///     Inicializa a transação atual, reaproveitando a existente do contexto ou criando uma nova.
    /// </summary>
    /// <param name="type">Tipo da transação a ser criada, caso não exista uma ativa.</param>
    void InitCurrentTransaction(string type = ApiConstants.TypeMessaging);

    /// <summary>
    ///     Finaliza a transação atual, encerrando sua captura no Elastic APM.
    /// </summary>
    void EndTransaction();

    /// <summary>
    ///     Captura e registra uma exceção dentro do escopo da transação atual.
    /// </summary>
    /// <param name="e">Exceção lançada.</param>
    void CaptureException(Exception e);

    /// <summary>
    ///     Cria e executa um span assíncrono com labels dentro da transação atual.
    ///     Útil para medir o tempo de blocos lógicos como chamadas de API, consultas ao banco etc.
    /// </summary>
    /// <param name="name">Nome do span a ser registrado.</param>
    /// <param name="spanLabel">Labels descritivos a serem associados ao span.</param>
    /// <param name="func">Função assíncrona a ser executada dentro do span.</param>
    /// <param name="type">Tipo do span (ex: messaging, db, external).</param>
    Task CaptureSpan(string name, Dictionary<string, string> spanLabel, Func<Task> func,
        string type = ApiConstants.TypeMessaging);

    /// <summary>
    ///     Cria e executa um span síncrono com labels dentro da transação atual.
    /// </summary>
    /// <param name="name">Nome do span.</param>
    /// <param name="spanLabel">Labels associados ao span.</param>
    /// <param name="action">Ação a ser executada no escopo do span.</param>
    /// <param name="type">Tipo do span.</param>
    void CaptureSpan(string name, Dictionary<string, string> spanLabel, Action action,
        string type = ApiConstants.TypeMessaging);

    /// <summary>
    ///     Adiciona um label personalizado à transação atual no Elastic APM.
    ///     Útil para filtros, correlação de dados e análises no Kibana.
    ///     Se não houver transação ativa, uma será criada automaticamente.
    /// </summary>
    /// <param name="key">Chave do label (nome do campo).</param>
    /// <param name="value">Valor associado à chave.</param>
    /// <param name="type">Tipo da transação, utilizado caso precise iniciar uma nova.</param>
    void AddLabel(string key, string value, string type = ApiConstants.TypeMessaging);

    /// <summary>
    ///     Retorna os dados de tracing da transação atual para propagação em chamadas downstream.
    /// </summary>
    /// <returns>String serializada contendo os dados de tracing.</returns>
    string GetOutgoingDistributedTracingData();

    /// <summary>
    ///     Adiciona o payload (dicionário) completo de um objeto à transação atual no Elastic APM.
    /// </summary>
    /// <param name="labels">Dicionário de labels contendo os dados do payload.</param>
    /// <param name="type">Tipo da transação, utilizado caso precise iniciar uma nova.</param>
    void AddMessagePayloadToTransaction(Dictionary<string, string> labels, string type = ApiConstants.TypeMessaging);

    /// <summary>
    ///     Adiciona múltiplos labels de uma só vez à transação atual.
    /// </summary>
    /// <param name="labels">Dicionário de labels a serem adicionados.</param>
    /// <param name="type">Tipo da transação, utilizado caso precise iniciar uma nova.</param>
    void AddLabels(Dictionary<string, string> labels, string type = ApiConstants.TypeMessaging);

    /// <summary>
    ///     Define o resultado da transação (success, error, etc.).
    /// </summary>
    /// <param name="result">Resultado da transação.</param>
    void SetTransactionResult(string result);

    /// <summary>
    ///     Define um contexto customizado para a transação.
    /// </summary>
    /// <param name="key">Chave do contexto.</param>
    /// <param name="value">Valor do contexto.</param>
    void SetCustomContext(string key, object value);

    /// <summary>
    ///     Verifica se existe uma transação ativa.
    /// </summary>
    /// <returns>True se existe transação ativa, false caso contrário.</returns>
    bool HasActiveTransaction();

    /// <summary>
    ///     Limpa a referência da transação atual (não encerra a transação).
    /// </summary>
    void ClearCurrentTransaction();
}