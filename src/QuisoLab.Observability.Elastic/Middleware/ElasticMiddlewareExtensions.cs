using Microsoft.AspNetCore.Builder;

namespace QuisoLab.Observability.Elastic.Middleware;

/// <summary>
///     Extensões para registrar o middleware.
/// </summary>
public static class ElasticMiddlewareExtensions
{
    /// <summary>
    ///     Adiciona o middleware de transações Elastic APM ao pipeline.
    /// </summary>
    /// <param name="builder">Application builder.</param>
    /// <returns>Application builder para chaining.</returns>
    public static IApplicationBuilder UseElasticMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ElasticTransactionMiddleware>();
    }
}
