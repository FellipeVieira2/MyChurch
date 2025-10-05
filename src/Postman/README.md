# ?? MyChurch API - Postman Collections

Coleções completas do Postman para testar a API do MyChurch.

## ?? Estrutura de Arquivos

```
Postman/
??? ChurchController.postman_collection.json   # Collection principal
??? MyChurch.Development.postman_environment.json  # Environment
??? README.md                                  # Instruções de uso
??? PAYLOADS.md                               # Exemplos de JSON
??? TESTING_GUIDE.md                          # Guia de testes passo a passo
??? AUTOMATION_SCRIPTS.md                     # Scripts de automação
```

## ?? Quick Start

### 1. Importar no Postman

1. **Collection**: Importe `ChurchController.postman_collection.json`
2. **Environment**: Importe `MyChurch.Development.postman_environment.json`
3. Selecione o environment "MyChurch - Development"

### 2. Configurar Token JWT

1. Faça login na API (endpoint de autenticação)
2. Copie o token JWT
3. Cole em: Environments > MyChurch - Development > `jwt_token`

### 3. Executar Requests

Pronto! Agora você pode executar qualquer endpoint da collection.

## ?? Documentação Completa

- **[README.md](README.md)** - Instruções básicas e configuração
- **[PAYLOADS.md](PAYLOADS.md)** - Exemplos de JSON para requests
- **[TESTING_GUIDE.md](TESTING_GUIDE.md)** - Guia passo a passo de testes
- **[AUTOMATION_SCRIPTS.md](AUTOMATION_SCRIPTS.md)** - Scripts para automação

## ?? Collection Overview

### ?? Public - Search Churches (6 endpoints)
Busca pública de igrejas com filtros avançados (não requer auth)

### ?? Geolocation (1 endpoint)
Busca por proximidade (latitude/longitude + raio)

### ? Church CRUD (4 endpoints)
Criar, buscar e atualizar igrejas (requer auth)

### ?? Banking & Financial (2 endpoints)
Gestão financeira e dashboard (Admin only)

### ?? QR Code & Onboarding (1 endpoint)
Geração de QR Code para onboarding

### ??? Location Management (3 endpoints)
Geolocalização manual e automática (Rate Limited)

**Total: 17 endpoints**

## ?? Variáveis do Environment

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `base_url` | URL da API | `https://localhost:7163` |
| `jwt_token` | Token de autenticação | `eyJhbGciOiJIUzI1...` |
| `church_id` | ID da igreja para testes | `1` |
| `admin_email` | Email do admin | `admin@igreja.com` |
| `admin_password` | Senha do admin | `SenhaForte@123` |
| `latitude_sp` | Latitude SP | `-23.550520` |
| `longitude_sp` | Longitude SP | `-46.633308` |

## ?? Autenticação

### Endpoints Públicos ?
- `GET /api/Church/public/search`
- `GET /api/Church/public/nearby`

### Endpoints Protegidos ??
Todos os outros (requerem JWT Token)

### Endpoints Admin Only ??
- `PUT /api/Church/{id}`
- `PUT /api/Church/banking-info`
- `GET /api/Church/dashboard`
- `PUT /api/Church/{id}/location`
- `POST /api/Church/{id}/geocode`

## ?? Rate Limiting

?? **Endpoints de Geolocalização**: Máximo de **20 requisições por minuto**

Se ultrapassar:
- Response: `429 Too Many Requests`
- Aguarde 60 segundos antes de tentar novamente

## ?? Executar via Newman (CLI)

### Instalar Newman

```bash
npm install -g newman
```

### Executar Collection

```bash
newman run ChurchController.postman_collection.json \
  -e MyChurch.Development.postman_environment.json
```

### Gerar Relatório HTML

```bash
newman run ChurchController.postman_collection.json \
  -e MyChurch.Development.postman_environment.json \
  --reporters cli,html \
  --reporter-html-export report.html
```

## ?? Exemplos Rápidos

### Buscar Igrejas Próximas

```http
GET {{base_url}}/api/Church/public/nearby
  ?lat=-23.550520
  &lng=-46.633308
  &radiusKm=5
```

### Criar Igreja com Admin

```http
POST {{base_url}}/api/Church/withadmin
Content-Type: application/json

{
  "name": "Igreja Teste",
  "adminEmail": "admin@teste.com",
  ...
}
```

### Geocodificar Igreja

```http
POST {{base_url}}/api/Church/1/geocode
Authorization: Bearer {{jwt_token}}
```

## ?? Troubleshooting

### ? 401 Unauthorized
- Verifique se `jwt_token` está configurado
- Token pode ter expirado

### ? 403 Forbidden
- Endpoint requer role Admin
- Verifique suas permissões

### ? 429 Too Many Requests
- Rate limit atingido
- Aguarde 1 minuto

### ? SSL Certificate Error
- Postman > Settings > Desabilite "SSL certificate verification"

## ?? Contribuindo

Para reportar bugs ou sugerir melhorias:

1. Abra uma issue no repositório
2. Descreva o problema/sugestão
3. Inclua screenshots se possível

## ?? Última Atualização

**05/10/2024** - Collection completa com 17 endpoints

---

**Desenvolvido para MyChurch API** ??
