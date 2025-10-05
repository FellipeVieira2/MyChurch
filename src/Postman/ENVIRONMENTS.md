# ?? Environments - MyChurch API

Configurações de ambientes para diferentes estágios de desenvolvimento.

## ?? Ambientes Disponíveis

### 1. ?? Development (Local)
**Arquivo**: `MyChurch.Development.postman_environment.json`

- **Base URL**: `https://localhost:7163`
- **Uso**: Desenvolvimento local
- **SSL**: Auto-assinado (desabilitar verificação no Postman)
- **Dados**: Dados de teste/mock

**Quando usar:**
- Desenvolvimento local
- Testes de novos recursos
- Debug de código

---

### 2. ?? Staging
**Arquivo**: `MyChurch.Staging.postman_environment.json`

- **Base URL**: `https://staging-api.mychurch.com.br`
- **Uso**: Ambiente de homologação
- **SSL**: Certificado válido
- **Dados**: Dados de staging (similares à produção)

**Quando usar:**
- Validação antes de produção
- Testes de integração
- Testes de aceitação (UAT)
- Demos para stakeholders

---

### 3. ?? Production
**Arquivo**: `MyChurch.Production.postman_environment.json`

- **Base URL**: `https://api.mychurch.com.br`
- **Uso**: Ambiente de produção
- **SSL**: Certificado válido
- **Dados**: Dados reais

**Quando usar:**
- ?? Apenas para verificações críticas
- ?? Não usar para testes destrutivos
- ?? Sempre validar antes de executar

---

## ?? Como Trocar de Ambiente

### No Postman Desktop

1. Canto superior direito
2. Dropdown de Environments
3. Selecione o ambiente desejado

### Via Newman (CLI)

```bash
# Development
newman run collection.json -e MyChurch.Development.postman_environment.json

# Staging
newman run collection.json -e MyChurch.Staging.postman_environment.json

# Production
newman run collection.json -e MyChurch.Production.postman_environment.json
```

---

## ?? Variáveis Comuns

Todas as variáveis presentes em todos os ambientes:

| Variável | Tipo | Descrição |
|----------|------|-----------|
| `base_url` | string | URL base da API |
| `jwt_token` | secret | Token JWT de autenticação |
| `church_id` | string | ID da igreja para testes |
| `admin_email` | string | Email do administrador |
| `admin_password` | secret | Senha do administrador |
| `latitude_sp` | string | Latitude de São Paulo |
| `longitude_sp` | string | Longitude de São Paulo |

---

## ?? Segurança

### Variáveis Secretas

As seguintes variáveis são marcadas como **secret**:
- `jwt_token`
- `admin_password`

?? **Importante:**
- Nunca commite tokens reais no Git
- Use valores vazios nos arquivos de environment
- Configure localmente após import

### Tokens por Ambiente

| Ambiente | Duração do Token | Renovação |
|----------|------------------|-----------|
| Development | 24 horas | Manual |
| Staging | 12 horas | Manual |
| Production | 1 hora | Automática via refresh token |

---

## ?? Configurações Específicas

### Development

```json
{
  "base_url": "https://localhost:7163",
  "ssl_verify": false,
  "timeout": 5000,
  "retry": 3
}
```

**Características:**
- SSL auto-assinado
- Timeout maior (para debug)
- Retry habilitado

### Staging

```json
{
  "base_url": "https://staging-api.mychurch.com.br",
  "ssl_verify": true,
  "timeout": 3000,
  "retry": 2
}
```

**Características:**
- Certificado SSL válido
- Timeout médio
- Dados similares à produção

### Production

```json
{
  "base_url": "https://api.mychurch.com.br",
  "ssl_verify": true,
  "timeout": 2000,
  "retry": 1
}
```

**Características:**
- ?? Dados reais
- Timeout baixo
- Retry mínimo
- Logging habilitado

---

## ?? Workflow Recomendado

### 1. Desenvolvimento

```mermaid
Development ? Testes Locais ? Commit ? CI/CD
```

1. Configure environment **Development**
2. Desenvolva e teste localmente
3. Execute collection completa
4. Commit quando todos os testes passarem

### 2. Homologação

```mermaid
Staging ? Testes Integração ? Validação ? Aprovação
```

1. Troque para environment **Staging**
2. Execute testes de integração
3. Valide com QA
4. Obtenha aprovação do PO

### 3. Produção

```mermaid
Production ? Smoke Tests ? Validação ? Monitoramento
```

1. ?? Apenas smoke tests
2. Validar endpoints críticos
3. Monitorar logs
4. Rollback se necessário

---

## ?? Testes por Ambiente

### Development
```bash
# Executar todos os testes
newman run collection.json -e Development.json

# Com relatório HTML
newman run collection.json -e Development.json \
  --reporters cli,html \
  --reporter-html-export dev-report.html
```

### Staging
```bash
# Apenas testes de integração
newman run collection.json -e Staging.json \
  --folder "Integration Tests"
  
# Testes de performance
newman run collection.json -e Staging.json \
  --iteration-count 100 \
  --delay-request 100
```

### Production
```bash
# ?? Apenas smoke tests
newman run collection.json -e Production.json \
  --folder "Smoke Tests" \
  --bail
```

---

## ?? Checklist de Troca de Ambiente

Antes de trocar de ambiente, verifique:

### Development ? Staging
- [ ] Todos os testes locais passando
- [ ] Code review aprovado
- [ ] CI/CD verde
- [ ] Branch mergeada

### Staging ? Production
- [ ] QA aprovado
- [ ] Testes de carga executados
- [ ] Documentação atualizada
- [ ] Rollback plan preparado
- [ ] Stakeholders notificados

---

## ?? Avisos Importantes

### ?? Production Environment

**NUNCA execute em produção:**
- ? Testes de carga
- ? Testes destrutivos (DELETE em massa)
- ? Criação de dados fake
- ? Testes de rate limiting

**SEMPRE:**
- ? Valide credenciais
- ? Execute apenas smoke tests
- ? Monitore logs após execução
- ? Tenha rollback plan

### ?? Credenciais

**NUNCA:**
- ? Commite tokens no Git
- ? Compartilhe senhas por email
- ? Use produção para testes

**SEMPRE:**
- ? Use secret managers
- ? Rotacione tokens regularmente
- ? Revogue tokens antigos

---

## ?? Suporte

Problemas com ambientes?

**Development:**
- Time de desenvolvimento

**Staging:**
- Time de QA

**Production:**
- DevOps/SRE team
- On-call engineer

---

## ?? Resumo

| Ambiente | URL | SSL | Dados | Testes |
|----------|-----|-----|-------|--------|
| Development | `localhost:7163` | ? Auto-assinado | Mock | Todos ? |
| Staging | `staging-api...` | ? Válido | Similar prod | Integração ? |
| Production | `api...` | ? Válido | Real | Smoke only ?? |

---

**Última atualização**: 05/10/2024  
**Versão**: 1.0.0
