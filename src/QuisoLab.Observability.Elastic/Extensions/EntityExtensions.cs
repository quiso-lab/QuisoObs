using System.Text.Json;
using System.Reflection;

namespace QuisoLab.Observability.Elastic.Extensions;

/// <summary>
///     Extensões utilitárias para extrair informações de entidades como labels para uso no Elastic APM.
/// </summary>
public static class EntityExtensions
{
    /// <summary>
    ///     Converte todas as propriedades públicas de um objeto em um dicionário de labels (chave/valor),
    ///     para uso com Elastic APM (transações ou spans).
    /// </summary>
    /// <typeparam name="T">Tipo do objeto de entrada (classe, interface, DTO etc).</typeparam>
    /// <param name="source">Instância a ser processada.</param>
    /// <returns>Dicionário com os nomes e valores das propriedades.</returns>
    public static Dictionary<string, string> SetLabels<T>(this T source) where T : class
    {
        if (source == null)
            return new Dictionary<string, string>();

        return ExtractLabels(source);
    }

    /// <summary>
    ///     Método auxiliar interno que percorre as propriedades do objeto e extrai seus valores como strings serializadas.
    ///     Trata listas, objetos complexos e tipos primitivos com melhor performance e tratamento de erros.
    /// </summary>
    /// <param name="source">Instância a ser processada.</param>
    /// <returns>Dicionário de labels no formato chave/valor.</returns>
    private static Dictionary<string, string> ExtractLabels(object source)
    {
        var labels = new Dictionary<string, string>();

        try
        {
            var entityType = source.GetType();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                try
                {
                    // Pula propriedades indexadas (como this[index])
                    if (property.GetIndexParameters().Length > 0)
                        continue;

                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(source, null);

                    if (propertyValue == null)
                    {
                        labels[propertyName] = null;
                        continue;
                    }

                    var propertyType = propertyValue.GetType();

                    // Trata coleções IEnumerable (exceto string)
                    if (propertyValue is System.Collections.IEnumerable enumerable && propertyType != typeof(string))
                    {
                        var items = new List<string>();
                        foreach (var item in enumerable)
                        {
                            if (item == null) continue;
                            
                            // Se é tipo primitivo ou string, usa ToString(), senão serializa
                            if (IsPrimitiveOrString(item.GetType()))
                                items.Add(item.ToString());
                            else
                                items.Add(JsonSerializer.Serialize(item));
                        }
                        
                        labels[propertyName] = items.Count > 0 ? string.Join(", ", items) : string.Empty;
                    }
                    // Trata tipos primitivos, enums e string
                    else if (IsPrimitiveOrString(propertyType) || propertyType.IsEnum)
                    {
                        labels[propertyName] = propertyValue.ToString();
                    }
                    // Trata DateTime
                    else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    {
                        labels[propertyName] = propertyValue.ToString();
                    }
                    // Trata objetos complexos - serializa como JSON
                    else
                    {
                        labels[propertyName] = JsonSerializer.Serialize(propertyValue, new JsonSerializerOptions
                        {
                            WriteIndented = false,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                    }
                }
                catch (Exception ex)
                {
                    // Em caso de erro ao processar uma propriedade específica, adiciona informação do erro
                    labels[$"{property.Name}_error"] = $"Error extracting property: {ex.Message}";
                }
            }
        }
        catch (Exception ex)
        {
            // Em caso de erro geral, adiciona informação do erro
            labels["extraction_error"] = $"Error extracting object labels: {ex.Message}";
        }

        return labels;
    }

    /// <summary>
    ///     Verifica se o tipo é primitivo, string ou nullable de primitivo.
    /// </summary>
    /// <param name="type">Tipo a ser verificado.</param>
    /// <returns>True se é primitivo ou string, false caso contrário.</returns>
    private static bool IsPrimitiveOrString(Type type)
    {
        return type.IsPrimitive || 
               type == typeof(string) || 
               type == typeof(decimal) || 
               type == typeof(DateTime) ||
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && 
                Nullable.GetUnderlyingType(type)?.IsPrimitive == true);
    }

    /// <summary>
    ///     Converte um objeto em labels com um prefixo personalizado.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto.</typeparam>
    /// <param name="source">Objeto fonte.</param>
    /// <param name="prefix">Prefixo a ser adicionado nas chaves.</param>
    /// <returns>Dicionário de labels com prefixo.</returns>
    public static Dictionary<string, string> SetLabelsWithPrefix<T>(this T source, string prefix) where T : class
    {
        var labels = source.SetLabels();
        
        if (string.IsNullOrWhiteSpace(prefix))
            return labels;

        var prefixedLabels = new Dictionary<string, string>();
        var cleanPrefix = prefix.Trim().TrimEnd('_') + "_";
        
        foreach (var kvp in labels)
        {
            prefixedLabels[$"{cleanPrefix}{kvp.Key}"] = kvp.Value;
        }
        
        return prefixedLabels;
    }

    /// <summary>
    ///     Converte apenas propriedades específicas de um objeto em labels.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto.</typeparam>
    /// <param name="source">Objeto fonte.</param>
    /// <param name="propertyNames">Nomes das propriedades a serem incluídas.</param>
    /// <returns>Dicionário com apenas as propriedades especificadas.</returns>
    public static Dictionary<string, string> SetLabelsForProperties<T>(this T source, params string[] propertyNames) where T : class
    {
        if (source == null || propertyNames == null || propertyNames.Length == 0)
            return new Dictionary<string, string>();

        var allLabels = source.SetLabels();
        var filteredLabels = new Dictionary<string, string>();

        foreach (var propertyName in propertyNames)
        {
            if (allLabels.TryGetValue(propertyName, out var value))
                filteredLabels[propertyName] = value;
        }

        return filteredLabels;
    }
}