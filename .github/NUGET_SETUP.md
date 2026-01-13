# Configuração do Pipeline de Publicação NuGet

Este documento explica como configurar e usar o pipeline de publicação automática no NuGet.

## 📋 Pré-requisitos

### 1. Criar API Key no NuGet.org

1. Acesse [https://www.nuget.org/account/apikeys](https://www.nuget.org/account/apikeys)
2. Clique em **"Create"**
3. Configure:
   - **Key Name**: `QuisoLab.Observability.Elastic`
   - **Package Owner**: Selecione seu usuário/organização
   - **Scopes**: 
     - ✅ **Push new packages and package versions** (obrigatório)
     - ✅ **Unlist or relist package versions** (recomendado - permite ocultar versões problemáticas)
   - **Packages**: 
     - Glob Pattern: `QuisoLab.Observability.Elastic`
   - **Expiration**: Escolha um período (recomendado: 365 dias)
4. Clique em **"Create"**
5. **COPIE A API KEY** (ela só será mostrada uma vez!)

### 2. Adicionar Secret no GitHub

1. Acesse o repositório: https://github.com/quiso-lab/QuisoObs
2. Vá em **Settings** → **Secrets and variables** → **Actions**
3. Clique em **"New repository secret"**
4. Configure:
   - **Name**: `NUGET_API_KEY`
   - **Secret**: Cole a API Key copiada do NuGet.org
5. Clique em **"Add secret"**

## 🚀 Como Funciona

### Estratégia de Branches

O projeto usa uma estratégia de versionamento com branches:

- **`main`**: Desenvolvimento contínuo - executa build e testes automaticamente
- **`release/*`**: Branches de release - executa build, testes E publicação no NuGet

### Trigger Automático - Build e Teste

Executado automaticamente quando:
- Há **push na `main`** ou em branches `release/*`
- Há **Pull Request** para `main` ou `release/*`
- Arquivos em `src/**` ou `*.sln` são modificados

### Trigger Automático - Publicação no NuGet

Executado automaticamente **APENAS** quando:
- Há **push em branches `release/*`** (exemplo: `release/1.0.0`, `release/1.1.0`)
- Arquivos em `src/**` ou `*.sln` são modificados

### Trigger Manual

Você pode executar manualmente a publicação:
1. Vá em **Actions** → **Publish to NuGet**
2. Clique em **"Run workflow"**
3. Selecione uma branch `release/*`
4. Clique em **"Run workflow"**

## 🔄 Fluxo de Publicação

### Desenvolvimento Normal (branch `main`)
1. **Push** para `main`
2. ✅ **Build** automático
3. ✅ **Testes** automáticos
4. ❌ Publicação no NuGet **NÃO** acontece

### Criação de Release
1. **Crie uma branch de release**:
   ```bash
   git checkout -b release/1.0.0
   ```

2. **Atualize a versão** no `.csproj`:
   ```xml
   <Version>1.0.0</Version>
   ```

3. **Commit e Push**:
   ```bash
   git add .
   git commit -m "chore: bump version to 1.0.0"
   git push origin release/1.0.0
   ```

4. **Pipeline de Publicação é Executado**:
   - ✅ Checkout do código
   - ✅ Setup do .NET 8.0
   - ✅ Restore das dependências
   - ✅ Build em modo Release
   - ✅ Testes (se existirem)
   - ✅ Extração da versão do `.csproj`
   - ✅ Pack do pacote NuGet
   - ✅ **Push para NuGet.org**
   - ✅ **Criação de GitHub Release**

5. **(Opcional) Merge para main**:
   ```bash
   git checkout main
   git merge release/1.0.0
   git push origin main
   ```

## 📦 Versionamento

A versão do pacote é controlada no arquivo `.csproj`:

```xml
<Version>1.0.0</Version>
```

### Como Publicar uma Nova Versão

1. **Crie uma branch de release** a partir da `main`:
   ```bash
   git checkout main
   git pull
   git checkout -b release/1.1.0
   ```

2. **Atualize a versão** no arquivo `.csproj`:
   ```bash
   # Em: src/QuisoLab.Observability.Elastic/QuisoLab.Observability.Elastic.csproj
   <Version>1.1.0</Version>  <!-- Altere aqui -->
   ```

3. **Commit e Push** para a branch de release:
   ```bash
   git add .
   git commit -m "chore: bump version to 1.1.0"
   git push origin release/1.1.0
   ```

4. O pipeline de **publicação será executado automaticamente**!

5. **(Recomendado) Merge de volta para main**:
   ```bash
   git checkout main
   git merge release/1.1.0
   git push origin main
   git branch -d release/1.1.0  # Limpa branch local
   ```

### Convenção de Versionamento (SemVer)

- **MAJOR** (1.0.0 → 2.0.0): Breaking changes
- **MINOR** (1.0.0 → 1.1.0): Novas funcionalidades (compatíveis)
- **PATCH** (1.0.0 → 1.0.1): Bug fixes

## 📊 Monitoramento

### Verificar Execução do Pipeline

1. Vá em **Actions** no GitHub
2. Clique no workflow **"Publish to NuGet"**
3. Veja os logs de cada step

### Verificar Publicação no NuGet

- Acesse: https://www.nuget.org/packages/QuisoLab.Observability.Elastic
- A nova versão deve aparecer em alguns minutos

### Verificar GitHub Release

- Vá em **Releases** no GitHub
- Uma nova release será criada automaticamente com a versão

## ⚠️ Troubleshooting

### Erro: "Package already exists"

**Causa**: Você está tentando publicar uma versão que já existe no NuGet.

**Solução**: Incremente a versão no `.csproj` antes de fazer push.

### Erro: "Invalid API Key"

**Causa**: A API Key configurada está incorreta ou expirou.

**Solução**:
1. Crie uma nova API Key no NuGet.org
2. Atualize o secret `NUGET_API_KEY` no GitHub

### Erro: "Unauthorized"

**Causa**: A API Key não tem permissão para publicar o pacote.

**Solução**: Verifique se a API Key tem o escopo "Push" habilitado.

### Pipeline não executa publicação

**Causa**: O push foi feito na branch `main` ou em outra branch que não é `release/*`.

**Solução**: 
1. Crie uma branch de release: `git checkout -b release/1.0.0`
2. Faça push para ela: `git push origin release/1.0.0`
3. Ou execute manualmente selecionando a branch `release/*`

## 🔒 Segurança

- ✅ A API Key está armazenada como **Secret** (criptografada)
- ✅ Nunca faça commit da API Key no código
- ✅ A API Key tem escopo **limitado** ao pacote específico
- ✅ Configure **expiração** para renovação periódica

## 📝 Personalização

### Alterar Condições de Trigger

Edite `.github/workflows/publish-nuget.yml`:

```yaml
on:
  push:
    branches:
      - main
      - release/*  # Adicionar outras branches
    tags:
      - 'v*'  # Trigger por tags também
```

### Adicionar Validações Extras

Adicione steps antes do Pack:

```yaml
- name: Code Quality Check
  run: dotnet format --verify-no-changes

- name: Security Scan
  run: dotnet list package --vulnerable
```

### Publicar em Feed Privado

Altere a URL do source:

```yaml
- name: Push to Private Feed
  run: dotnet nuget push ./artifacts/*.nupkg 
       --api-key ${{ secrets.PRIVATE_FEED_KEY }} 
       --source https://pkgs.dev.azure.com/quiso-lab/_packaging/feed/nuget/v3/index.json
```

## 📚 Recursos Adicionais

- [NuGet.org Documentation](https://learn.microsoft.com/en-us/nuget/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Semantic Versioning](https://semver.org/)
- [Package README Best Practices](https://learn.microsoft.com/en-us/nuget/nuget-org/package-readme-on-nuget-org)

---

**Pronto!** Agora toda vez que você fizer push na `main` com alterações de código, o pacote será publicado automaticamente no NuGet! 🚀
