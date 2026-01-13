# Contribuindo para QuisoLab.Observability.Elastic

Obrigado por considerar contribuir para o projeto! 🎉

Este documento fornece diretrizes para contribuições ao projeto.

## 🌟 Como Contribuir

### Reportar Bugs

Se encontrou um bug:

1. **Verifique** se já não existe uma issue relatando o mesmo problema
2. **Crie uma nova issue** usando o template de Bug Report
3. **Descreva** o problema detalhadamente:
   - Passos para reproduzir
   - Comportamento esperado vs atual
   - Versão do pacote e .NET
   - Stack trace (se aplicável)

### Sugerir Melhorias

Tem uma ideia para melhorar o projeto?

1. **Crie uma issue** usando o template de Feature Request
2. **Explique** o problema que a feature resolve
3. **Descreva** a solução proposta
4. **Considere** alternativas e impactos

### Pull Requests

#### Antes de Começar

1. **Fork** o repositório
2. **Clone** seu fork localmente
3. **Crie uma branch** a partir da `main`:
   ```bash
   git checkout -b feature/minha-feature
   # ou
   git checkout -b fix/meu-bugfix
   ```

#### Durante o Desenvolvimento

1. **Siga os padrões** de código do projeto (C# 12+, primary constructors, etc.)
2. **Escreva testes** se aplicável
3. **Mantenha commits** pequenos e descritivos
4. **Use Conventional Commits**:
   - `feat:` nova funcionalidade
   - `fix:` correção de bug
   - `docs:` mudanças em documentação
   - `refactor:` refatoração de código
   - `test:` adição ou correção de testes
   - `chore:` mudanças em build, CI, etc.

#### Padrões de Código

- ✅ Use **Primary Constructors** quando apropriado
- ✅ Prefira **Collection Expressions** (`[]`) 
- ✅ Use `ArgumentNullException.ThrowIfNull` para validações
- ✅ Adicione **XML documentation comments** para APIs públicas
- ✅ Mantenha métodos **pequenos e focados**
- ✅ Siga as **convenções de nomenclatura** C#
- ✅ Execute `dotnet format` antes de commitar

#### Exemplo de Commit

```bash
git commit -m "feat: adiciona suporte para custom sampling strategies

- Implementa interface ISamplingStrategy
- Adiciona AdaptiveSamplingStrategy
- Atualiza documentação com exemplos
- Adiciona testes unitários

Closes #42"
```

#### Submetendo o PR

1. **Push** para seu fork:
   ```bash
   git push origin feature/minha-feature
   ```

2. **Abra um Pull Request** para a branch `main`

3. **Preencha o template** de PR com:
   - Descrição das mudanças
   - Tipo de mudança (bugfix, feature, breaking change, etc.)
   - Checklist de verificação
   - Issues relacionadas

4. **Aguarde review** - mantenha-se disponível para discussões

### Revisão de Código

Ao revisar PRs de outros:

- ✅ Seja **construtivo** e **respeitoso**
- ✅ Foque no **código**, não na pessoa
- ✅ Explique o **porquê** das sugestões
- ✅ Aprove quando estiver satisfeito
- ✅ Use **Approve**, **Request Changes** ou **Comment** apropriadamente

## 📋 Checklist para PRs

Antes de submeter, verifique:

- [ ] Código segue os padrões do projeto
- [ ] Commits seguem Conventional Commits
- [ ] Adicionou/atualizou testes (se aplicável)
- [ ] Adicionou/atualizou documentação
- [ ] Build e testes passam localmente
- [ ] Executou `dotnet format`
- [ ] PR tem uma descrição clara
- [ ] Issues relacionadas estão referenciadas

## 🏗️ Configuração do Ambiente

### Pré-requisitos

- .NET 8.0 SDK ou superior
- Git
- Editor de código (VS Code, Visual Studio, Rider, etc.)

### Setup

```bash
# Clone seu fork
git clone https://github.com/SEU-USUARIO/QuisoObs.git
cd QuisoObs

# Adicione o repositório original como upstream
git remote add upstream https://github.com/quiso-lab/QuisoObs.git

# Restaure dependências
dotnet restore

# Build
dotnet build

# Execute testes
dotnet test
```

### Mantendo seu Fork Atualizado

```bash
git fetch upstream
git checkout main
git merge upstream/main
git push origin main
```

## 🧪 Executando Testes

```bash
# Todos os testes
dotnet test

# Com coverage
dotnet test --collect:"XPlat Code Coverage"

# Testes específicos
dotnet test --filter FullyQualifiedName~EntityExtensions
```

## 📝 Documentação

Ao adicionar/modificar código:

1. **XML Comments** para APIs públicas:
   ```csharp
   /// <summary>
   ///     Descrição breve do método.
   /// </summary>
   /// <param name="parameter">Descrição do parâmetro.</param>
   /// <returns>Descrição do retorno.</returns>
   public string MyMethod(string parameter)
   ```

2. **README.md** se adicionar features significativas
3. **Exemplos** em comentários ou arquivo separado

## 🚀 Processo de Release

Releases são gerenciadas pelos maintainers:

1. Versão atualizada no `.csproj`
2. Branch `release/x.x.x` criada
3. Pipeline CI/CD publica automaticamente
4. GitHub Release criada automaticamente

## 🤝 Código de Conduta

Este projeto adere ao [Código de Conduta](CODE_OF_CONDUCT.md). Ao participar, você concorda em seguir suas diretrizes.

## 💬 Dúvidas?

- Abra uma **Discussion** no GitHub
- Crie uma **issue** com a tag `question`
- Entre em contato com os maintainers

## 🙏 Reconhecimento

Todas as contribuições serão reconhecidas:
- Mencionadas no CHANGELOG
- Listadas como contribuidores do projeto
- Gratidão eterna da comunidade! ❤️

---

**Obrigado por contribuir! Juntos tornamos este projeto melhor! 🚀**
