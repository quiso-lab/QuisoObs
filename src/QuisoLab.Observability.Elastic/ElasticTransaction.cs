using Elastic.Apm;
using Elastic.Apm.Api;

namespace QuisoLab.Observability.Elastic;

/// <summary>
///     Classe concreta para gerenciamento centralizado de transações com Elastic APM.
///     Permite iniciar, encerrar, enriquecer e monitorar transações e spans em aplicações distribuídas.
/// </summary>
public class ElasticTransaction : IElasticTransaction
{
    /// <summary>
    ///     Retorna a transação ativa atual, se houver.
    /// </summary>
    public ITransaction CurrentTransaction { get; private set; }

    /// <summary>
    ///     Inicia uma nova transação com um nome definido, tipo e tracing distribuído (se houver).
    ///     Se o tracing for inválido ou nulo, a transação é criada normalmente.
    /// </summary>
    /// <param name="name">Nome da transação visível no Elastic APM.</param>
    /// <param name="distributedTracingData">Dados do tracing (traceparent), geralmente oriundo de headers.</param>
    /// <param name="type">Tipo da transação (ex: messaging, request, external).</param>
    public void StartTransaction(string name, string distributedTracingData,
        string type = ApiConstants.TypeMessaging)
    {
        if (CurrentTransaction is not null) return;

        // Validação e sanitização dos parâmetros
        var transactionName = string.IsNullOrWhiteSpace(name) ? "default" : name.Trim();
        var transactionType = string.IsNullOrWhiteSpace(type) ? ApiConstants.TypeMessaging : type.Trim();

        try
        {
            var tracingData = string.IsNullOrWhiteSpace(distributedTracingData) 
                ? null 
                : DistributedTracingData.TryDeserializeFromString(distributedTracingData);
                
            CurrentTransaction = tracingData is not null
                ? Agent.Tracer.StartTransaction(transactionName, transactionType, tracingData)
                : Agent.Tracer.StartTransaction(transactionName, transactionType);
        }
        catch (Exception ex)
        {
            // Em caso de erro, criar transação básica e capturar exceção
            CurrentTransaction = Agent.Tracer.StartTransaction(transactionName, transactionType);
            CaptureException(ex);
        }
    }

    /// <summary>
    ///     Inicializa a transação atual, reaproveitando a existente do contexto ou criando uma nova.
    /// </summary>
    /// <param name="type">Tipo da transação a ser criada, caso não exista uma ativa.</param>
    public void InitCurrentTransaction(string type = ApiConstants.TypeMessaging)
    {
        CurrentTransaction ??= Agent.Tracer.CurrentTransaction ?? Agent.Tracer.StartTransaction("Default", type);
    }

    /// <summary>
    ///     Finaliza a transação atual, encerrando sua captura no Elastic APM.
    /// </summary>
    public void EndTransaction()
    {
        CurrentTransaction?.End();
    }

    /// <summary>
    ///     Captura e registra uma exceção dentro do escopo da transação atual.
    /// </summary>
    /// <param name="e">Exceção lançada.</param>
    public void CaptureException(Exception e)
    {
        CurrentTransaction?.CaptureException(e);
    }

    /// <summary>
    ///     Cria e executa um span assíncrono com labels dentro da transação atual.
    ///     Útil para medir o tempo de blocos lógicos como chamadas de API, consultas ao banco etc.
    /// </summary>
    /// <param name="name">Nome do span a ser registrado.</param>
    /// <param name="spanLabel">Labels descritivos a serem associados ao span.</param>
    /// <param name="func">Função assíncrona a ser executada dentro do span.</param>
    /// <param name="type">Tipo do span (ex: messaging, db, external).</param>
    public async Task CaptureSpan(string name, Dictionary<string, string> spanLabel, Func<Task> func,
        string type = ApiConstants.TypeMessaging)
    {
        if (CurrentTransaction is null) InitCurrentTransaction(type);
        
        // Validações de entrada
        if (string.IsNullOrWhiteSpace(name))
        {
            CaptureException(new ArgumentException("Span name cannot be null or empty", nameof(name)));
            return;
        }
        
        if (func is null)
        {
            CaptureException(new ArgumentNullException(nameof(func), "Function to execute cannot be null"));
            return;
        }

        try
        {
            await CurrentTransaction.CaptureSpan(name.Trim(), type, async span =>
            {
                // Adiciona labels se fornecidos
                if (spanLabel?.Count > 0)
                {
                    foreach (var item in spanLabel.Where(item => !string.IsNullOrWhiteSpace(item.Key)))
                    {
                        span.SetLabel(item.Key.Trim(), item.Value ?? string.Empty);
                    }
                }
                
                await func().ConfigureAwait(false);
            });
        }
        catch (Exception ex)
        {
            CaptureException(ex);
        }
    }

    /// <summary>
    ///     Cria e executa um span síncrono com labels dentro da transação atual.
    /// </summary>
    /// <param name="name">Nome do span.</param>
    /// <param name="spanLabel">Labels associados ao span.</param>
    /// <param name="action">Ação a ser executada no escopo do span.</param>
    /// <param name="type">Tipo do span.</param>
    public void CaptureSpan(string name, Dictionary<string, string> spanLabel, Action action,
        string type = ApiConstants.TypeMessaging)
    {
        if (CurrentTransaction is null) InitCurrentTransaction(type);
        
        // Validações de entrada
        if (string.IsNullOrWhiteSpace(name))
        {
            CaptureException(new ArgumentException("Span name cannot be null or empty", nameof(name)));
            return;
        }
        
        if (action is null)
        {
            CaptureException(new ArgumentNullException(nameof(action), "Action to execute cannot be null"));
            return;
        }

        try
        {
            CurrentTransaction?.CaptureSpan(name.Trim(), type, span =>
            {
                // Adiciona labels se fornecidos
                if (spanLabel?.Count > 0)
                {
                    foreach (var item in spanLabel.Where(item => !string.IsNullOrWhiteSpace(item.Key)))
                    {
                        span.SetLabel(item.Key.Trim(), item.Value ?? string.Empty);
                    }
                }
                
                action();
            });
        }
        catch (Exception ex)
        {
            CaptureException(ex);
        }
    }

    /// <summary>
    ///     Adiciona um label personalizado à transação atual no Elastic APM.
    ///     Útil para filtros, correlação de dados e análises no Kibana.
    ///     Se não houver transação ativa, uma será criada automaticamente.
    /// </summary>
    /// <param name="key">Chave do label (nome do campo).</param>
    /// <param name="value">Valor associado à chave.</param>
    /// <param name="type">Tipo da transação, utilizado caso precise iniciar uma nova.</param>
    public void AddLabel(string key, string value, string type = ApiConstants.TypeMessaging)
    {
        if (CurrentTransaction is null) InitCurrentTransaction(type);
        
        // Validação de entrada
        if (string.IsNullOrWhiteSpace(key))
        {
            CaptureException(new ArgumentException("Label key cannot be null or empty", nameof(key)));
            return;
        }
        
        try
        {
            CurrentTransaction?.SetLabel(key.Trim(), value ?? string.Empty);
        }
        catch (Exception ex)
        {
            CaptureException(ex);
        }
    }

    /// <summary>
    ///     Retorna os dados de tracing da transação atual para propagação em chamadas downstream.
    /// </summary>
    /// <returns>String serializada contendo os dados de tracing.</returns>
    public string GetOutgoingDistributedTracingData()
    {
        try
        {
            return CurrentTransaction?.OutgoingDistributedTracingData?.SerializeToString();
        }
        catch (Exception ex)
        {
            CaptureException(ex);
            return null;
        }
    }

    /// <summary>
    ///     Adiciona o payload (dicionário) completo de um objeto à transação atual no Elastic APM.
    /// </summary>
    /// <param name="labels">Dicionário de labels contendo os dados do payload.</param>
    /// <param name="type">Tipo da transação, utilizado caso precise iniciar uma nova.</param>
    public void AddMessagePayloadToTransaction(Dictionary<string, string> labels,
        string type = ApiConstants.TypeMessaging)
    {
        if (CurrentTransaction is null) InitCurrentTransaction(type);

        try
        {
            if (labels is null || labels.Count == 0)
            {
                var message = labels is null ? "Labels dictionary is null" : "Labels dictionary is empty";
                var detailedMessage = $"AddMessagePayloadToTransaction: {message} - Transaction: {CurrentTransaction?.Name ?? "Unknown"}, Type: {type}";
                
                // Adiciona label de warning com detalhes
                CurrentTransaction?.SetLabel("warning_reason", message);
                CurrentTransaction?.SetLabel("warning_method", "AddMessagePayloadToTransaction");
                CurrentTransaction?.SetLabel("warning_transaction_type", type);

                // Captura como exceção informativa para facilitar debugging
                var warningException = new InvalidOperationException(detailedMessage);
                CaptureException(warningException);

                return;
            }
        }
        catch (Exception ex)
        {
            CaptureException(ex);
            return;
        }

        try
        {
            foreach (var kv in labels.Where(kv => !string.IsNullOrWhiteSpace(kv.Key)))
            {
                CurrentTransaction?.SetLabel(kv.Key, kv.Value ?? string.Empty);
            }
        }
        catch (Exception ex)
        {
            CaptureException(ex);
        }
    }

    /// <summary>
    ///     Adiciona múltiplos labels de uma só vez à transação atual.
    /// </summary>
    /// <param name="labels">Dicionário de labels a serem adicionados.</param>
    /// <param name="type">Tipo da transação, utilizado caso precise iniciar uma nova.</param>
    public void AddLabels(Dictionary<string, string> labels, string type = ApiConstants.TypeMessaging)
    {
        if (CurrentTransaction is null) InitCurrentTransaction(type);

        if (!(labels?.Count > 0)) return;
        
        foreach (var kvp in labels)
        {
            AddLabel(kvp.Key, kvp.Value, type);
        }
    }

    /// <summary>
    ///     Define o resultado da transação (success, error, etc.).
    /// </summary>
    /// <param name="result">Resultado da transação.</param>
    public void SetTransactionResult(string result)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(result) && CurrentTransaction is not null)
                CurrentTransaction.Result = result.Trim();
        }
        catch (Exception ex)
        {
            CaptureException(ex);
        }
    }

    /// <summary>
    ///     Define um contexto customizado para a transação.
    /// </summary>
    /// <param name="key">Chave do contexto.</param>
    /// <param name="value">Valor do contexto.</param>
    public void SetCustomContext(string key, object value)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(key) && value != null && CurrentTransaction != null)
            {
                CurrentTransaction.Context.Custom[key.Trim()] = (string)value;
            }
        }
        catch (Exception ex)
        {
            CaptureException(ex);
        }
    }

    /// <summary>
    ///     Verifica se existe uma transação ativa.
    /// </summary>
    /// <returns>True se existe transação ativa, false caso contrário.</returns>
    public bool HasActiveTransaction()
    {
        return CurrentTransaction != null;
    }

    /// <summary>
    ///     Limpa a referência da transação atual (não encerra a transação).
    /// </summary>
    public void ClearCurrentTransaction()
    {
        CurrentTransaction = null;
    }
}
