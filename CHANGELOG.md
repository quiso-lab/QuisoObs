# Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planejado
- Suporte a métricas customizadas
- Configuração de sampling strategies
- Suporte a tracing distribuído avançado

## [1.0.0] - 2026-01-13

### 🎉 Lançamento Inicial

Primeira versão pública da biblioteca QuisoLab.Observability.Elastic.

### ✨ Adicionado

#### Core Features
- **IElasticTransaction**: Interface principal para gerenciamento de transações
- **ElasticTransaction**: Implementação completa da interface
- **ElasticTransactionMiddleware**: Middleware ASP.NET Core para captura automática de transações HTTP
- **ElasticConfiguration**: Sistema de configuração flexível

#### Extensions
- **EntityExtensions**: Conversão automática de objetos para labels
  - `SetLabels()`: Converte todas as propriedades
  - `SetLabelsWithPrefix()`: Adiciona prefixo aos labels
  - `SetLabelsForProperties()`: Filtra propriedades específicas
- Suporte para tipos primitivos, coleções, objetos complexos e enums
- Tratamento robusto de erros com fallback

#### Métodos Principais
- `StartTransaction()`: Inicia transação manual
- `EndTransaction()`: Finaliza transação
- `AddLabel()` / `AddLabels()`: Adiciona contexto
- `CaptureSpan()`: Cria spans para operações
- `CaptureException()`: Registra exceções
- `SetTransactionResult()`: Define resultado da transação
- `SetCustomContext()`: Contexto customizado
- `HasActiveTransaction()`: Verifica estado

#### Configuração
- Suporte a appsettings.json
- Configuração programática via delegate
- Validação de configuração em runtime
- Sanitização de campos sensíveis
- Global labels configuráveis

#### CI/CD
- GitHub Actions workflow para build e testes
- Pipeline de publicação automática no NuGet
- Criação automática de releases no GitHub

#### Documentação
- README.md completo com exemplos
- CONTRIBUTING.md com guidelines
- CODE_OF_CONDUCT.md
- SECURITY.md com políticas de segurança
- Issue templates (Bug Report, Feature Request, Question)
- Pull Request template
- Guias de configuração e release

### 🔧 Técnico

#### Requisitos
- .NET 8.0+
- Elastic APM .NET Agent
- ASP.NET Core (para middleware)

#### Características
- **Primary Constructors** (C# 12)
- **Collection Expressions** para sintaxe moderna
- Reflection otimizada com `BindingFlags`
- Serialização JSON configurável
- Tratamento de exceções robusto
- Validação de parâmetros consistente

### 📦 Pacote NuGet

#### Metadados
- **PackageId**: QuisoLab.Observability.Elastic
- **Versão**: 1.0.0
- **Licença**: MIT
- **Tags**: elastic, apm, observability, monitoring, tracing, aspnetcore
- **Ícone**: Incluído no pacote
- **README**: Incluído automaticamente
- **Símbolos**: SnupKg para debugging

### 🙏 Contribuidores

- [@inocencio.cardoso](https://github.com/inocencio.cardoso) - Desenvolvimento inicial

---

## Categorias de Mudanças

- `Added` - Novas funcionalidades
- `Changed` - Mudanças em funcionalidades existentes
- `Deprecated` - Funcionalidades que serão removidas
- `Removed` - Funcionalidades removidas
- `Fixed` - Correções de bugs
- `Security` - Correções de vulnerabilidades

---

[Unreleased]: https://github.com/quiso-lab/QuisoObs/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/quiso-lab/QuisoObs/releases/tag/v1.0.0
