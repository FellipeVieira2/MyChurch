# ?? MyChurch API - Postman Collections

Coleções completas do Postman para testar a API do MyChurch.

## ?? Estrutura de Arquivos

```
Postman/
??? ChurchController.postman_collection.json           # Collection Church
??? ChurchPhotoController.postman_collection.json      # Collection Photos (NEW)
??? MyChurch.Development.postman_environment.json      # Environment Dev
??? MyChurch.Staging.postman_environment.json          # Environment Staging
??? MyChurch.Production.postman_environment.json       # Environment Prod
??? README.md                                          # Instruções gerais
??? PAYLOADS.md                                        # Exemplos de JSON (Church)
??? TESTING_GUIDE.md                                   # Guia de testes (Church)
??? AUTOMATION_SCRIPTS.md                              # Scripts de automação
??? ENVIRONMENTS.md                                    # Guia de ambientes
??? INDEX.md                                           # Índice visual
??? CHURCH_PHOTO_README.md                             # Documentação Photos
??? CHURCH_PHOTO_PAYLOADS.md                           # Exemplos Photos
??? CHURCH_PHOTO_TESTING_GUIDE.md                      # Guia testes Photos
```

## ?? Quick Start

### 1. Importar Collections

1. **ChurchController**: Importe `ChurchController.postman_collection.json`
2. **ChurchPhotoController**: Importe `ChurchPhotoController.postman_collection.json`
3. **Environment**: Importe `MyChurch.Development.postman_environment.json`
4. Selecione o environment "MyChurch - Development"

### 2. Configurar Token JWT

1. Faça login na API (endpoint de autenticação)
2. Copie o token JWT
3. Cole em: Environments > MyChurch - Development > `jwt_token`

### 3. Executar Requests

Pronto! Agora você pode executar qualquer endpoint das collections.

## ?? Collections Disponíveis

### 1?? ChurchController (17 endpoints)

Gerenciamento completo de igrejas:
- ?? Busca pública com filtros avançados
- ?? Geolocalização (nearby, geocoding)
- ? CRUD de igrejas
- ?? Gestão financeira
- ?? QR Code de onboarding

**Documentação:**
- [README.md](README.md) - Guia completo
- [PAYLOADS.md](PAYLOADS.md) - Exemplos de JSON
- [TESTING_GUIDE.md](TESTING_GUIDE.md) - Guia de testes

### 2?? ChurchPhotoController (15 endpoints) ?? NEW!

Galeria de fotos das igrejas:
- ?? Upload de fotos (10 categorias)
- ?? Listar e filtrar fotos
- ?? Sistema de likes
- ??? Deletar fotos (admin/autor)

**Documentação:**
- [CHURCH_PHOTO_README.md](CHURCH_PHOTO_README.md) - Guia completo
- [CHURCH_PHOTO_PAYLOADS.md](CHURCH_PHOTO_PAYLOADS.md) - Exemplos
- [CHURCH_PHOTO_TESTING_GUIDE.md](CHURCH_PHOTO_TESTING_GUIDE.md) - Testes

## ?? Estatísticas Gerais

| Métrica | Valor |
|---------|-------|
| **Collections** | 2 |
| **Total Endpoints** | 32 |
| **Endpoints Públicos** | 8 |
| **Endpoints Protegidos** | 24 |
| **Categorias de Fotos** | 10 |
| **Ambientes** | 3 (Dev/Staging/Prod) |
| **Documentação** | ~1200 linhas |

## ?? Documentação Completa

### Church Controller
- **[README.md](README.md)** - Instruções básicas e configuração
- **[PAYLOADS.md](PAYLOADS.md)** - Exemplos de JSON para requests
- **[TESTING_GUIDE.md](TESTING_GUIDE.md)** - Guia passo a passo de testes
- **[AUTOMATION_SCRIPTS.md](AUTOMATION_SCRIPTS.md)** - Scripts para automação

### Church Photo Gallery (NEW)
- **[CHURCH_PHOTO_README.md](CHURCH_PHOTO_README.md)** - Guia completo da galeria
- **[CHURCH_PHOTO_PAYLOADS.md](CHURCH_PHOTO_PAYLOADS.md)** - Exemplos de upload
- **[CHURCH_PHOTO_TESTING_GUIDE.md](CHURCH_PHOTO_TESTING_GUIDE.md)** - Testes detalhados

### Ambientes
- **[ENVIRONMENTS.md](ENVIRONMENTS.md)** - Guia de ambientes (Dev/Staging/Prod)

### Geral
- **[INDEX.md](INDEX.md)** - Índice visual com estatísticas

## ?? Collections Overview

### ?? ChurchController (17 endpoints)

#### Public - Search Churches (6)
- Busca básica
- Filtro por cidade/estado
- Filtro por rating/reviews
- Busca por proximidade
- Filtro por amenidades
- Filtro por denominação

#### Geolocation (1)
- Busca nearby (lat/lng + raio)

#### Church CRUD (4)
- Criar igreja simples
- Criar igreja com admin
- Buscar por ID
- Atualizar dados

#### Banking & Financial (2)
- Atualizar dados bancários
- Dashboard administrativo

#### QR Code (1)
- Gerar QR Code onboarding

#### Location Management (3)
- Update manual (lat/lng)
- Geocoding automático
- Endpoint dedicado geocoding

### ?? ChurchPhotoController (15 endpoints) NEW!

#### Upload Photos (6)
- Upload - Exterior (Fachada)
- Upload - Interior (Templo)
- Upload - Worship (Culto)
- Upload - Events (Eventos)
- Upload - Child Ministry
- Upload - Facilities

#### List & Filter (6)
- Todas as fotos
- Filtro por categoria - Exterior
- Filtro por categoria - Worship
- Apenas fotos em destaque
- Paginação grande (50)
- Página 2

#### Like System (2)
- Curtir foto
- Descurtir foto

#### Delete (1)
- Deletar foto (admin/autor)

## ?? Variáveis do Environment

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `base_url` | URL da API | `https://localhost:7163` |
| `jwt_token` | Token de autenticação | `eyJhbGciOiJIUzI1...` |
| `church_id` | ID da igreja para testes | `1` |
| `photo_id` | ID da foto (NEW) | `1` |
| `admin_email` | Email do admin | `admin@igreja.com` |
| `admin_password` | Senha do admin | `SenhaForte@123` |
| `latitude_sp` | Latitude SP | `-23.550520` |
| `longitude_sp` | Longitude SP | `-46.633308` |

## ?? Autenticação

### Endpoints Públicos ?
- `GET /api/Church/public/search`
- `GET /api/Church/public/nearby`
- `GET /api/ChurchPhoto/church/{id}`

### Endpoints Protegidos ??
Todos os outros (requerem JWT Token)

### Endpoints Admin Only ??
- Church: Update, Banking, Dashboard, Location
- Photos: Delete (admin OU autor)

## ?? Rate Limiting

?? **Geolocalização**: Máximo de **20 requisições por minuto**
- `PUT /api/Church/{id}/location`
- `POST /api/Church/{id}/geocode`

## ?? Executar via Newman (CLI)

### Instalar Newman

```bash
npm install -g newman
```

### Executar Collection - Church

```bash
newman run ChurchController.postman_collection.json \
  -e MyChurch.Development.postman_environment.json
```

### Executar Collection - Photos

```bash
newman run ChurchPhotoController.postman_collection.json \
  -e MyChurch.Development.postman_environment.json
```

### Executar Ambas com Relatório

```bash
# Church
newman run ChurchController.postman_collection.json \
  -e MyChurch.Development.postman_environment.json \
  --reporters cli,html \
  --reporter-html-export church-report.html

# Photos
newman run ChurchPhotoController.postman_collection.json \
  -e MyChurch.Development.postman_environment.json \
  --reporters cli,html \
  --reporter-html-export photos-report.html
```

## ?? Exemplos Rápidos

### ChurchController

```http
# Buscar Igrejas Próximas
GET {{base_url}}/api/Church/public/nearby
  ?lat=-23.550520
  &lng=-46.633308
  &radiusKm=5

# Criar Igreja com Admin
POST {{base_url}}/api/Church/withadmin
Content-Type: application/json
{
  "name": "Igreja Teste",
  "adminEmail": "admin@teste.com",
  ...
}
```

### ChurchPhotoController (NEW)

```http
# Upload Foto da Fachada
POST {{base_url}}/api/ChurchPhoto
Authorization: Bearer {{jwt_token}}
Content-Type: application/json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,...",
  "caption": "Fachada principal",
  "category": 1
}

# Listar Fotos de Culto
GET {{base_url}}/api/ChurchPhoto/church/1?category=3

# Curtir Foto
POST {{base_url}}/api/ChurchPhoto/5/like?like=true
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

### ? Foto não aparece após upload
- Verifique se upload retornou 200
- Confirme `isApproved: true`
- Use `church_id` correto

## ?? Contribuindo

Para reportar bugs ou sugerir melhorias:

1. Abra uma issue no repositório
2. Descreva o problema/sugestão
3. Inclua screenshots se possível

## ?? Roadmap

### Próximas Collections
- [ ] ChurchScheduleController (Horários de cultos)
- [ ] ChurchReviewController (Avaliações)
- [ ] MemberController
- [ ] EventController

## ?? Última Atualização

**05/10/2024** - Adicionada Collection ChurchPhotoController (15 endpoints)

---

**Desenvolvido para MyChurch API** ??

**Total de Endpoints**: 32  
**Collections**: 2  
**Documentação**: ~1200 linhas  
**Status**: ? Production Ready
