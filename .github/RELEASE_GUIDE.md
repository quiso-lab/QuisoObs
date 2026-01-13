# 🚀 Guia Rápido - Publicação de Release

## Fluxo Simplificado

```bash
# 1. Atualizar da main
git checkout main
git pull

# 2. Criar branch de release
git checkout -b release/1.0.0

# 3. Atualizar versão no .csproj
# Edite: src/QuisoLab.Observability.Elastic/QuisoLab.Observability.Elastic.csproj
# Altere: <Version>1.0.0</Version>

# 4. Commit e push
git add .
git commit -m "chore: bump version to 1.0.0"
git push origin release/1.0.0

# 5. Aguardar publicação automática (verificar em Actions)

# 6. Merge de volta para main
git checkout main
git merge release/1.0.0
git push origin main

# 7. Limpar branch de release
git branch -d release/1.0.0
git push origin --delete release/1.0.0
```

## Branches e Pipelines

| Branch | Build & Test | Publica NuGet |
|--------|--------------|---------------|
| `main` | ✅ Sim | ❌ Não |
| `release/*` | ✅ Sim | ✅ **Sim** |
| Outras | ❌ Não | ❌ Não |

## Versionamento SemVer

- **MAJOR** (1.0.0 → 2.0.0): Breaking changes
- **MINOR** (1.0.0 → 1.1.0): Novas funcionalidades
- **PATCH** (1.0.0 → 1.0.1): Bug fixes

## Verificações

- ✅ GitHub Actions: https://github.com/quiso-lab/QuisoObs/actions
- ✅ NuGet.org: https://www.nuget.org/packages/QuisoLab.Observability.Elastic
- ✅ Releases: https://github.com/quiso-lab/QuisoObs/releases

## Dicas

💡 **Branch de release é temporária** - crie, publique, faça merge e delete  
💡 **Versão deve estar no .csproj** - é de lá que o pipeline lê  
💡 **Aguarde alguns minutos** - o pacote leva tempo para aparecer no NuGet  
💡 **Use tags** - após merge, crie uma tag: `git tag v1.0.0 && git push --tags`
