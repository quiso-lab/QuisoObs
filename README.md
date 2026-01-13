# QuisoLab.Observability.Elastic

Biblioteca .NET de observabilidade para gerenciamento centralizado de transações com Elastic APM, projetada para aplicações distribuídas da QuisoLab.

## 🚀 Funcionalidades

### ✅ Implementações Realizadas
- ✅ **Mensagens de log detalhadas** quando labels são null ou vazios
- ✅ **Validação robusta de parâmetros** em todos os métodos
- ✅ **Tratamento de exceções aprimorado** com captura automática
- ✅ **Middleware ASP.NET Core** para captura automática de transações HTTP
- ✅ **Sistema de configuração flexível** com validação
- ✅ **Métodos adicionais** para melhor usabilidade
- ✅ **EntityExtensions melhorado** com tratamento de erros e performance otimizada

### 🔧 Principais Melhorias

#### 1. **Logging e Debugging Aprimorado**
```csharp
// Agora quando labels é null/vazio, você verá logs detalhados:
_elasticTransaction.AddMessagePayloadToTransaction(null, "messaging");
// Resultado: Labels com warning_reason, warning_method, warning_transaction_type
// + Exceção capturada com detalhes completos do contexto
```

#### 2. **Validações Robustas**
```csharp
// Validação de parâmetros em todos os métodos
_elasticTransaction.CaptureSpan("", null, () => { }); 
// Captura exceções ArgumentException com mensagens específicas

_elasticTransaction.AddLabel("", "value");
// Valida e sanitiza todas as entradas
```

#### 3. **Middleware Automático para ASP.NET Core**
```csharp
// No Startup.cs ou Program.cs
services.ConfigureElasticServices(configuration);
app.UseElasticTransaction(); // Captura automática de todas as requisições HTTP
```

#### 4. **Configuração Flexível**
```csharp
// Via appsettings.json
services.ConfigureElasticServices(configuration, "ElasticApm");

// Via delegate
services.ConfigureElasticServices(config => {
    config.ServiceName = "MinhaAPI";
    config.Environment = "production";
    config.TransactionSampleRate = 0.1;
});

// Validação de configuração
Startup.ValidateElasticConfiguration(config, logger);
```

#### 5. **Novos Métodos Úteis**
```csharp
// Múltiplos labels de uma vez
_elasticTransaction.AddLabels(new Dictionary<string, string> {
    ["user_id"] = "123",
    ["tenant"] = "acme"
});

// Contexto customizado
_elasticTransaction.SetCustomContext("business_context", businessData);

// Resultado da transação
_elasticTransaction.SetTransactionResult("success");

// Verificações de estado
if (_elasticTransaction.HasActiveTransaction()) { }
```

#### 6. **EntityExtensions Melhorado**
```csharp
// Com prefixo
var labels = order.SetLabelsWithPrefix("order");

// Propriedades específicas
var labels = user.SetLabelsForProperties("Name", "Email", "Role");

// Tratamento robusto de erros e tipos complexos
var labels = complexObject.SetLabels(); // Funciona com listas, objetos aninhados, etc.
```

## 📦 Instalação

```bash
dotnet add package QuisoLab.Observability.Elastic
```

## 🔧 Configuração Básica

### 1. **Configuração Simples**
```csharp
// Program.cs ou Startup.cs
services.ConfigureElasticServices();
```

### 2. **Configuração com appsettings.json**
```json
{
  "ElasticApm": {
    "ServiceName": "minha-api",
    "ServiceVersion": "1.0.0",
    "Environment": "production",
    "ServerUrl": "http://elastic-apm:8200",
    "SecretToken": "seu-token",
    "TransactionSampleRate": 1.0,
    "CaptureHeaders": true,
    "GlobalLabels": {
      "team": "backend",
      "application": "core-api"
    }
  }
}
```

```csharp
services.ConfigureElasticServices(configuration);
```

### 3. **Configuração com Middleware (Recomendado para APIs)**
```csharp
// Program.cs
services.ConfigureElasticServices(configuration);

// No pipeline de middleware
app.UseElasticTransaction(); // Adicionar antes de outros middlewares
app.UseRouting();
app.UseAuthentication();
// ...
```

## 💻 Uso

### **Injeção de Dependência**
```csharp
public class OrderService
{
    private readonly IElasticTransaction _elasticTransaction;

    public OrderService(IElasticTransaction elasticTransaction)
    {
        _elasticTransaction = elasticTransaction;
    }
}
```

### **Exemplo Completo com Melhorias**
```csharp
public async Task<Order> ProcessOrderAsync(CreateOrderRequest request)
{
    try
    {
        // Inicia transação manual (se não usando middleware)
        _elasticTransaction.StartTransaction("ProcessOrder", null, "business");
        
        // Labels do contexto
        var contextLabels = new Dictionary<string, string>
        {
            ["user_id"] = request.UserId,
            ["order_type"] = request.Type,
            ["total_amount"] = request.TotalAmount.ToString("F2")
        };
        _elasticTransaction.AddLabels(contextLabels);

        // Span para validação
        await _elasticTransaction.CaptureSpan("ValidateOrder", 
            request.SetLabelsWithPrefix("request"), 
            async () => await ValidateOrderAsync(request));

        // Span para processamento
        var order = await _elasticTransaction.CaptureSpan("CreateOrder",
            new Dictionary<string, string> { ["step"] = "creation" },
            async () => await CreateOrderInDatabaseAsync(request));

        // Labels do resultado
        _elasticTransaction.AddLabels(order.SetLabelsWithPrefix("order"));
        _elasticTransaction.SetTransactionResult("success");
        _elasticTransaction.SetCustomContext("order_result", new { 
            OrderId = order.Id, 
            Status = order.Status 
        });

        return order;
    }
    catch (Exception ex)
    {
        _elasticTransaction.CaptureException(ex);
        _elasticTransaction.SetTransactionResult("error");
        _elasticTransaction.AddLabel("error_category", GetErrorCategory(ex));
        throw;
    }
    finally
    {
        _elasticTransaction.EndTransaction();
    }
}
```

## 🛠️ Melhorias de Robustez Implementadas

### **1. Tratamento de Erros**
- ✅ Validação de parâmetros nulos/vazios em todos os métodos
- ✅ Sanitização automática de strings (trim, null safety)
- ✅ Captura automática de exceções internas
- ✅ Logs detalhados para debugging
- ✅ Fallback para transações padrão em caso de erro

### **2. Performance**
- ✅ Verificação otimizada de tipos primitivos
- ✅ Uso de BindingFlags para melhor performance de reflection
- ✅ Serialização JSON configurada para performance
- ✅ Reutilização de transações existentes quando possível

### **3. Usabilidade**
- ✅ Métodos com validação de entrada consistente
- ✅ Labels de warning automáticos para debugging
- ✅ Métodos helper para casos comuns
- ✅ Configuração flexível com múltiplas opções

### **4. Observabilidade**
- ✅ Labels automáticos para HTTP requests (método, URL, status, etc.)
- ✅ Contexto de erro detalhado
- ✅ Tracing distribuído automático
- ✅ Metadados de performance (tempo de resposta, etc.)

## 🔍 Debugging e Troubleshooting

### **Logs de Warning Automáticos**
Quando você chamar `AddMessagePayloadToTransaction` com labels null/vazios:

```
Labels dictionary is null - Transaction: ProcessOrder, Type: messaging
```

Os seguintes labels de warning serão adicionados automaticamente:
- `warning_reason`: "Labels dictionary is null" ou "Labels dictionary is empty"
- `warning_method`: "AddMessagePayloadToTransaction"
- `warning_transaction_type`: Tipo da transação

### **Validação de Configuração**
```csharp
var config = configuration.GetSection("ElasticApm").Get<ElasticConfiguration>();
if (!Startup.ValidateElasticConfiguration(config, logger))
{
    // Configuração inválida - verifique os logs
}
```

## 📋 Próximas Melhorias Sugeridas

1. **Métricas Customizadas**: Adicionar suporte a métricas além de transações
2. **Rate Limiting**: Implementar rate limiting inteligente para high-volume
3. **Batching**: Agrupamento de labels para reduzir overhead
4. **Health Checks**: Verificação automática de conectividade com Elastic
5. **Circuit Breaker**: Proteção contra falhas do Elastic APM
6. **Async Context**: Melhor suporte para contexto assíncrono
7. **Correlation IDs**: Geração automática de IDs de correlação
8. **Sampling Strategies**: Estratégias de amostragem mais sofisticadas

## 📚 Documentação Adicional

- [Elastic APM .NET Agent](https://www.elastic.co/guide/en/apm/agent/dotnet/current/index.html)
- [Configuração Avançada](./docs/advanced-configuration.md)
- [Exemplos de Uso](./docs/examples.md)
- [Troubleshooting](./docs/troubleshooting.md)

## 🤝 Contribuição

Para contribuir com melhorias:

1. Fork o repositório
2. Crie uma branch para sua feature
3. Implemente os testes
4. Submeta um Pull Request

## 📄 Licença

© QuisoLab 2026 - Uso interno da organização.
