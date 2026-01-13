# Política de Segurança

## 🔒 Versões Suportadas

Apenas a versão mais recente do pacote recebe atualizações de segurança.

| Versão | Suportada          |
| ------ | ------------------ |
| 1.x.x  | :white_check_mark: |
| < 1.0  | :x:                |

## 🚨 Reportando uma Vulnerabilidade

A segurança do QuisoLab.Observability.Elastic é uma prioridade. Se você descobriu uma vulnerabilidade de segurança, por favor **NÃO abra uma issue pública**.

### Processo de Reporte

1. **Email**: Envie os detalhes para os mantenedores através de uma issue privada ou discussão
2. **Informações a Incluir**:
   - Descrição detalhada da vulnerabilidade
   - Passos para reproduzir
   - Versões afetadas
   - Impacto potencial
   - Sugestões de correção (se houver)

### O que Esperar

- **Confirmação**: Resposta em até 48 horas
- **Avaliação**: Análise e validação da vulnerabilidade
- **Correção**: Desenvolvimento de patch de segurança
- **Release**: Publicação de versão corrigida
- **Divulgação**: Anúncio público após a correção estar disponível

### Processo de Divulgação

1. Vulnerabilidade reportada de forma privada
2. Correção desenvolvida e testada
3. Nova versão publicada
4. Aviso de segurança publicado (GitHub Security Advisory)
5. Divulgação pública dos detalhes após prazo razoável

## 🛡️ Práticas de Segurança

### Para Usuários

- ✅ Sempre use a versão mais recente do pacote
- ✅ Revise as release notes para atualizações de segurança
- ✅ Configure adequadamente as permissões do Elastic APM
- ✅ Não exponha secrets/tokens em logs ou código
- ✅ Use HTTPS para comunicação com Elastic APM Server

### Para Contribuidores

- ✅ Nunca commite secrets, tokens ou credenciais
- ✅ Revise código para possíveis injeções ou vulnerabilidades
- ✅ Use `SanitizeFieldNames` para evitar vazamento de dados sensíveis
- ✅ Valide e sanitize todas as entradas de usuário
- ✅ Siga as melhores práticas de codificação segura

## 🔐 Configurações Recomendadas

```json
{
  "ElasticApm": {
    "SanitizeFieldNames": true,
    "SanitizeFields": [
      "password",
      "passwd",
      "pwd",
      "secret",
      "key",
      "token",
      "authorization",
      "cookie",
      "credit_card",
      "ssn"
    ],
    "CaptureBody": false,  // Evita captura acidental de dados sensíveis
    "CaptureHeaders": true,
    "ServerUrl": "https://your-apm-server.com",  // Use HTTPS
    "SecretToken": "${APM_SECRET_TOKEN}"  // Use variáveis de ambiente
  }
}
```

## 📋 Checklist de Segurança

Antes de usar em produção:

- [ ] API Keys e tokens armazenados de forma segura (Azure Key Vault, AWS Secrets Manager, etc.)
- [ ] Comunicação com APM Server via HTTPS
- [ ] `SanitizeFieldNames` habilitado
- [ ] Lista de campos sensíveis configurada
- [ ] `CaptureBody` desabilitado ou com whitelist
- [ ] Logs não contêm informações sensíveis
- [ ] Versão mais recente do pacote instalada
- [ ] Dependencies atualizadas e sem vulnerabilidades conhecidas

## 🔍 Auditoria de Dependências

Execute regularmente:

```bash
# Verificar vulnerabilidades conhecidas
dotnet list package --vulnerable

# Atualizar dependências
dotnet add package QuisoLab.Observability.Elastic
```

## 📞 Contato

Para questões de segurança urgentes ou sensíveis, entre em contato através dos canais oficiais do projeto.

---

**Obrigado por ajudar a manter o QuisoLab.Observability.Elastic seguro!** 🔒
