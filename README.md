# QuisoLab.Observability.Elastic

[![Build Status](https://github.com/quiso-lab/QuisoObs/actions/workflows/build.yml/badge.svg)](https://github.com/quiso-lab/QuisoObs/actions/workflows/build.yml)
[![Publish to NuGet](https://github.com/quiso-lab/QuisoObs/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/quiso-lab/QuisoObs/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/v/QuisoLab.Observability.Elastic.svg)](https://www.nuget.org/packages/QuisoLab.Observability.Elastic/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)
[![Code of Conduct](https://img.shields.io/badge/code%20of-conduct-ff69b4.svg)](CODE_OF_CONDUCT.md)

Biblioteca .NET para integração simplificada com Elastic APM, fornecendo observabilidade completa para aplicações ASP.NET Core com captura automática de transações, spans, exceções e contexto distribuído.

## 📖 Sobre o Projeto

**QuisoLab.Observability.Elastic** é uma abstração sobre o Elastic APM .NET Agent que simplifica a instrumentação de aplicações, oferecendo:

- 🎯 **API simplificada** para gerenciamento de transações e spans
- 🔄 **Captura automática** de requisições HTTP via middleware
- 🔄 **Captura personalizada** para mensageria
- 🏷️ **Conversão automática** de objetos em labels para contexto rico
- ⚙️ **Configuração flexível** via appsettings.json ou código
- 🛡️ **Tratamento robusto** de erros e validações

Ideal para times que precisam de observabilidade profunda sem complexidade excessiva.

## ✨ Funcionalidades Principais

### 🔍 Rastreamento de Transações
- Início e fim automático de transações HTTP
- Suporte a transações manuais para processos de negócio
- Labels customizáveis para contexto de negócio
- Captura automática de exceções

### 📊 Spans e Contexto
- Criação de spans para operações específicas
- Contexto customizado para análise detalhada
- Suporte a tracing distribuído
- Extração automática de propriedades como labels

### 🔧 Configuração e Extensibilidade
- Configuração via appsettings.json ou código
- Validação de configurações em tempo de execução
- Extensões para conversão de entidades em labels
- Middleware ASP.NET Core integrado

## 📦 Instalação

### Via NuGet
```bash
dotnet add package QuisoLab.Observability.Elastic
```

### Via Projeto Local
1. Clone o repositório:
```bash
git clone https://github.com/quiso-lab/QuisoObs.git
```

2. Referencie o projeto em sua aplicação:
```xml
<ItemGroup>
  <ProjectReference Include="..\QuisoObs\src\QuisoLab.Observability.Elastic\QuisoLab.Observability.Elastic.csproj" />
</ItemGroup>
```

## 🚀 Primeiros Passos

### 1. Configuração Básica

Adicione no `Program.cs`:

```csharp
using QuisoLab.Observability.Elastic;

var builder = WebApplication.CreateBuilder(args);

// Registra os serviços do Elastic APM
builder.Services.ConfigureElasticServices(builder.Configuration);

var app = builder.Build();

// Adiciona o middleware (deve vir antes de outros middlewares)
app.UseElasticMiddleware();

app.UseRouting();
app.MapControllers();
app.Run();
```

### 2. Configuração via appsettings.json

```json
{
  "ElasticApm": {
    "ServiceName": "minha-api",
    "ServiceVersion": "1.0.0",
    "Environment": "production",
    "ServerUrl": "http://elastic-apm-server:8200",
    "SecretToken": "seu-token-secreto",
    "TransactionSampleRate": 1.0,
    "CaptureHeaders": true,
    "GlobalLabels": {
      "team": "backend",
      "application": "core-api"
    }
  }
}
```

### 3. Configuração Programática (Opcional)

```csharp
builder.Services.ConfigureElasticServices(config =>
{
    config.ServiceName = "MinhaAPI";
    config.Environment = "production";
    config.ServerUrl = "http://elastic-apm:8200";
    config.TransactionSampleRate = 0.5;
});
```

## 💻 Como Usar

### Exemplo Básico - Controller com Middleware

Quando você usa o middleware, transações HTTP são criadas automaticamente:

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IElasticTransaction _elasticTransaction;
    private readonly IOrderService _orderService;

    public OrdersController(IElasticTransaction elasticTransaction, IOrderService orderService)
    {
        _elasticTransaction = elasticTransaction;
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        // Adiciona contexto adicional à transação HTTP automática
        _elasticTransaction.AddLabel("customer_id", request.CustomerId);
        
        // Cria span para operação de negócio
        var order = await _elasticTransaction.CaptureSpan(
            "ProcessOrder",
            null,
            async () => await _orderService.ProcessOrderAsync(request)
        );

        return Ok(order);
    }
}
```

### Exemplo Avançado - Transação Manual com Spans

```csharp
public class OrderService
{
    private readonly IElasticTransaction _elasticTransaction;
    private readonly IRepository<Order> _repository;

    public OrderService(IElasticTransaction elasticTransaction, IRepository<Order> repository)
    {
        _elasticTransaction = elasticTransaction;
        _repository = repository;
    }

    public async Task<Order> ProcessOrderAsync(CreateOrderRequest request)
    {
        try
        {
            // Inicia transação manual para processos de negócio
            _elasticTransaction.StartTransaction("ProcessOrder", null, "business");
            
            // Adiciona contexto
            _elasticTransaction.AddLabels(new Dictionary<string, string>
            {
                ["user_id"] = request.UserId,
                ["order_type"] = request.Type,
                ["total_amount"] = request.TotalAmount.ToString("F2")
            });

            // Span para validação
            await _elasticTransaction.CaptureSpan("ValidateOrder", 
                request.SetLabelsWithPrefix("request"), 
                async () => await ValidateOrderAsync(request));

            // Span para persistência
            var order = await _elasticTransaction.CaptureSpan("SaveOrder",
                null,
                async () => await _repository.SaveAsync(request));

            // Adiciona resultado
            _elasticTransaction.SetTransactionResult("success");
            _elasticTransaction.AddLabels(order.SetLabels());

            return order;
        }
        catch (Exception ex)
        {
            _elasticTransaction.CaptureException(ex);
            _elasticTransaction.SetTransactionResult("error");
            throw;
        }
        finally
        {
            _elasticTransaction.EndTransaction();
        }
    }

    private async Task ValidateOrderAsync(CreateOrderRequest request)
    {
        // Lógica de validação
        if (request.TotalAmount <= 0)
            throw new ValidationException("Invalid amount");
    }
}
```

### Extensões de Entidades - Conversão Automática para Labels

```csharp
// Converte todas as propriedades automaticamente
var labels = order.SetLabels();
_elasticTransaction.AddLabels(labels);

// Com prefixo para organização
var customerLabels = customer.SetLabelsWithPrefix("customer");
// Resulta em: customer_Name, customer_Email, customer_Age, etc.

// Apenas propriedades específicas
var userLabels = user.SetLabelsForProperties("Name", "Email", "Role");
_elasticTransaction.AddLabels(userLabels);
```

## 📚 API de Métodos Disponíveis

| Método | Descrição | Exemplo |
|--------|-----------|---------|
| `StartTransaction(name, tracingData, type)` | Inicia uma nova transação | `StartTransaction("ProcessOrder", null, "business")` |
| `EndTransaction()` | Finaliza a transação atual | `EndTransaction()` |
| `AddLabel(key, value)` | Adiciona um label individual | `AddLabel("user_id", "123")` |
| `AddLabels(dictionary)` | Adiciona múltiplos labels | `AddLabels(new Dictionary<string, string> {...})` |
| `CaptureSpan(name, labels, action)` | Cria e captura um span | `CaptureSpan("DbQuery", null, async () => {...})` |
| `CaptureException(exception)` | Captura uma exceção | `CaptureException(ex)` |
| `SetTransactionResult(result)` | Define resultado (success/error) | `SetTransactionResult("success")` |
| `SetCustomContext(key, value)` | Adiciona contexto customizado | `SetCustomContext("business_data", data)` |
| `HasActiveTransaction()` | Verifica se há transação ativa | `if (HasActiveTransaction()) {...}` |

## 🏗️ Estrutura do Projeto

```
QuisoLab.Observability.Elastic/
├── Configuration/
│   └── ElasticConfiguration.cs          # Configurações do Elastic APM
├── Extensions/
│   └── EntityExtensions.cs              # Extensões para conversão de objetos
├── Middleware/
│   ├── ElasticTransactionMiddleware.cs  # Middleware de captura automática
│   └── ElasticMiddlewareExtensions.cs   # Extensões do middleware
├── ElasticTransaction.cs                # Implementação principal
├── IElasticTransaction.cs               # Interface pública
└── Startup.cs                           # Configuração de serviços
```

## 🤝 Como Contribuir

Contribuições são bem-vindas! Siga estas etapas:

### 1. Fork e Clone
```bash
git clone https://github.com/seu-usuario/QuisoObs.git
cd QuisoObs
```

### 2. Crie uma Branch
```bash
git checkout -b feature/minha-feature
```

### 3. Desenvolva e Teste
- Escreva código seguindo os padrões do projeto (C# 12, primary constructors, collection expressions)
- Adicione testes se aplicável
- Mantenha a documentação atualizada

### 4. Commit e Push
```bash
git add .
git commit -m "feat: adiciona nova funcionalidade X"
git push origin feature/minha-feature
```

### 5. Abra um Pull Request
- Descreva as mudanças detalhadamente
- Referencie issues relacionadas
## 🤝 Como Contribuir

Contribuições são bem-vindas! Siga estas etapas:

### 1. Fork e Clone
```bash
git clone https://github.com/seu-usuario/QuisoObs.git
cd QuisoObs
```

### 2. Crie uma Branch
```bash
git checkout -b feature/minha-feature
```

### 3. Desenvolva e Teste
- Escreva código seguindo os padrões do projeto (C# 12, primary constructors, collection expressions)
- Adicione testes se aplicável
- Mantenha a documentação atualizada

### 4. Commit e Push
```bash
git add .
git commit -m "feat: adiciona nova funcionalidade X"
git push origin feature/minha-feature
```

### 5. Abra um Pull Request
- Descreva as mudanças detalhadamente
- Referencie issues relacionadas
- Aguarde review do time

### 📚 Documentação Completa

Para mais detalhes sobre como contribuir, consulte:
- 📖 [CONTRIBUTING.md](CONTRIBUTING.md) - Guia completo de contribuição
- 🤝 [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) - Código de conduta da comunidade
- 🐛 [Bug Report Template](.github/ISSUE_TEMPLATE/bug_report.md)
- ✨ [Feature Request Template](.github/ISSUE_TEMPLATE/feature_request.md)

## 📄 Licença

Este projeto está licenciado sob a [Licença MIT](LICENSE).

© QuisoLab 2026

## 📞 Contato e Suporte

- 💬 **Discussions**: [GitHub Discussions](https://github.com/quiso-lab/QuisoObs/discussions)
- 🐛 **Issues**: [Reportar Bug](https://github.com/quiso-lab/QuisoObs/issues/new?template=bug_report.md)
- ✨ **Feature Requests**: [Sugerir Feature](https://github.com/quiso-lab/QuisoObs/issues/new?template=feature_request.md)
- 📦 **NuGet**: [QuisoLab.Observability.Elastic](https://www.nuget.org/packages/QuisoLab.Observability.Elastic/)
- 🏢 **Organização**: [QuisoLab](https://github.com/quiso-lab)

## 🔗 Links Úteis

- [Elastic APM .NET Agent Documentation](https://www.elastic.co/guide/en/apm/agent/dotnet/current/index.html)
- [Elastic APM Server](https://www.elastic.co/guide/en/apm/server/current/index.html)
- [C# 12 Features](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12)

---

**QuisoLab.Observability.Elastic** - Observabilidade simplificada para aplicações .NET 🚀
