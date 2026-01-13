# GitHub Actions Workflows

Este repositório possui workflows automatizados para CI/CD.

## 📋 Workflows Disponíveis

### 1. 🏗️ Build and Test (`build.yml`)

**Quando executa:**
- Em todo Pull Request para `main`
- Em todo push na `main`
- Quando arquivos em `src/**` são modificados

**O que faz:**
- ✅ Restaura dependências
- ✅ Compila o projeto
- ✅ Executa testes (se existirem)
- ✅ Verifica formatação de código
- ✅ Testa criação do pacote NuGet

**Propósito:** Garantir que o código está compilando e funcionando antes de merge.

### 2. 📦 Publish to NuGet (`publish-nuget.yml`)

**Quando executa:**
- Em push na `main` (após merge de PR)
- Manualmente via "workflow_dispatch"

**O que faz:**
- ✅ Compila em modo Release
- ✅ Cria pacote NuGet
- ✅ Publica no NuGet.org
- ✅ Cria GitHub Release com a versão

**Propósito:** Publicar automaticamente novas versões do pacote.

## 🚀 Fluxo de Trabalho Recomendado

### Para Desenvolvimento

```bash
# 1. Crie uma branch
git checkout -b feature/nova-funcionalidade

# 2. Desenvolva e commit
git add .
git commit -m "feat: adiciona nova funcionalidade"

# 3. Push da branch
git push origin feature/nova-funcionalidade

# 4. Abra um Pull Request
# O workflow "Build and Test" será executado automaticamente
```

### Para Publicação de Nova Versão

```bash
# 1. Atualize a versão no .csproj
# Em: src/QuisoLab.Observability.Elastic/QuisoLab.Observability.Elastic.csproj
# Altere: <Version>1.0.0</Version> para <Version>1.1.0</Version>

# 2. Commit a mudança de versão
git add src/QuisoLab.Observability.Elastic/QuisoLab.Observability.Elastic.csproj
git commit -m "chore: bump version to 1.1.0"

# 3. Push para main (ou merge PR)
git push origin main

# 4. O workflow "Publish to NuGet" executa automaticamente!
```

## ⚙️ Configuração Necessária

### Secrets Necessários

| Secret | Onde Obter | Como Configurar |
|--------|------------|-----------------|
| `NUGET_API_KEY` | [NuGet.org](https://www.nuget.org/account/apikeys) | Settings → Secrets → Actions |

### Configuração do NUGET_API_KEY

Veja o guia completo em [NUGET_SETUP.md](./NUGET_SETUP.md)

## 📊 Monitoramento

### Ver Status dos Workflows

1. Acesse a aba **Actions** no GitHub
2. Selecione o workflow desejado
3. Veja os logs de execução

### Badges de Status

Adicione ao README.md:

```markdown
![Build Status](https://github.com/quiso-lab/QuisoObs/actions/workflows/build.yml/badge.svg)
![NuGet](https://github.com/quiso-lab/QuisoObs/actions/workflows/publish-nuget.yml/badge.svg)
```

## 🔧 Personalização

### Alterar Versão do .NET

Em ambos os workflows, altere:

```yaml
env:
  DOTNET_VERSION: '9.0.x'  # Alterar versão aqui
```

### Adicionar Mais Ambientes

Teste em múltiplos sistemas operacionais:

```yaml
jobs:
  build:
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
    runs-on: ${{ matrix.os }}
```

### Adicionar Validações de Segurança

```yaml
- name: Security Scan
  run: dotnet list package --vulnerable --include-transitive
  
- name: Dependency Check
  run: dotnet list package --outdated
```

## 📝 Convenções

### Mensagens de Commit

Siga [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` - Nova funcionalidade
- `fix:` - Correção de bug
- `docs:` - Mudanças na documentação
- `chore:` - Mudanças em build, CI, etc.
- `refactor:` - Refatoração de código
- `test:` - Adição ou correção de testes

### Branches

- `main` - Branch principal (protegida)
- `feature/*` - Novas funcionalidades
- `fix/*` - Correções de bugs
- `docs/*` - Documentação

## 🛡️ Proteção de Branch

Recomendado configurar proteção na branch `main`:

1. Settings → Branches → Add rule
2. Branch name pattern: `main`
3. Habilitar:
   - ✅ Require a pull request before merging
   - ✅ Require status checks to pass before merging
     - Status: `build`
   - ✅ Require branches to be up to date before merging

## ❓ Troubleshooting

### Build falha no PR

1. Execute localmente: `dotnet build`
2. Verifique os logs do workflow
3. Corrija os erros
4. Faça push das correções

### Publicação falha

1. Verifique se a versão foi incrementada
2. Verifique se `NUGET_API_KEY` está configurada
3. Verifique se a API Key não expirou
4. Veja logs detalhados no workflow

## 📚 Links Úteis

- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [.NET CI/CD](https://learn.microsoft.com/en-us/dotnet/devops/)
- [NuGet Package Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/publish-a-package)

---

Para mais detalhes sobre publicação no NuGet, consulte [NUGET_SETUP.md](./NUGET_SETUP.md).
