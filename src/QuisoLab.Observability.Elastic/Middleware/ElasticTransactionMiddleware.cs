using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace QuisoLab.Observability.Elastic.Middleware;

/// <summary>
///     Middleware para captura automática de transações APM em aplicações ASP.NET Core.
/// </summary>
public class ElasticTransactionMiddleware(
    RequestDelegate next,
    IElasticTransaction elasticTransaction,
    ILogger<ElasticTransactionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;
        var transactionName = $"{request.Method} {request.Path}";

        try
        {
            // Extrai dados de tracing distribuído do header
            var tracingData = request.Headers.TryGetValue("traceparent", out var traceParent)
                ? traceParent.FirstOrDefault()
                : null;

            // Inicia transação
            elasticTransaction.StartTransaction(transactionName, tracingData, "request");

            // Adiciona labels da requisição
            var requestLabels = new Dictionary<string, string>
            {
                ["http.method"] = request.Method,
                ["http.url"] = request.GetDisplayUrl(),
                ["http.scheme"] = request.Scheme,
                ["http.host"] = request.Host.ToString(),
                ["http.path"] = request.Path,
                ["http.query_string"] = request.QueryString.ToString(),
                ["user_agent"] = request.Headers.UserAgent.FirstOrDefault() ?? "",
                ["remote_address"] = context.Connection.RemoteIpAddress?.ToString() ?? "",
                ["request_id"] = context.TraceIdentifier
            };

            elasticTransaction.AddLabels(requestLabels);

            // Executa próximo middleware
            await next(context);

            // Adiciona informações da resposta
            var response = context.Response;
            var responseLabels = new Dictionary<string, string>
            {
                ["http.status_code"] = response.StatusCode.ToString(),
                ["http.response_time_ms"] = stopwatch.ElapsedMilliseconds.ToString()
            };

            elasticTransaction.AddLabels(responseLabels);

            // Define resultado baseado no status code
            var result = response.StatusCode >= 400 ? "error" : "success";
            elasticTransaction.SetTransactionResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ElasticTransactionMiddleware for {TransactionName}", transactionName);

            elasticTransaction.CaptureException(ex);
            elasticTransaction.SetTransactionResult("error");
            elasticTransaction.AddLabel("error.message", ex.Message);
            elasticTransaction.AddLabel("error.type", ex.GetType().Name);

            throw;
        }
        finally
        {
            stopwatch.Stop();
            elasticTransaction.EndTransaction();
        }
    }
}
