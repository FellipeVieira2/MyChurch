# ?? Guia de Testes - ChurchController

Guia passo a passo para testar todos os fluxos do ChurchController.

## ?? Pré-requisitos

- ? Postman instalado
- ? Collection importada
- ? Environment configurado
- ? API rodando em `https://localhost:7163`

## ?? Fluxo Completo de Teste

### 1?? Testes Públicos (Sem Autenticação)

#### 1.1. Busca Básica de Igrejas

```
Request: Search Public Churches - Basic
Method: GET
Expected: 200 OK
```

**Validações:**
- ? Lista de igrejas retornada
- ? Paginação funcionando
- ? Campos básicos presentes (id, name, city, state)

#### 1.2. Busca por Cidade e Estado

```
Request: Search by City and State
Method: GET
Expected: 200 OK
```

**Validações:**
- ? Apenas igrejas da cidade/estado filtrados
- ? Total count correto

#### 1.3. Busca com Filtros Avançados

```
Request: Search with Advanced Filters
Method: GET
Expected: 200 OK
```

**Teste diferentes combinações:**
- `minRating=4.0` - Apenas igrejas com 4+ estrelas
- `minReviews=10` - Apenas com 10+ avaliações
- `sortBy=rating` - Ordenar por nota

**Validações:**
- ? Filtros aplicados corretamente
- ? Ordenação funcionando
- ? Métricas de rating/reviews presentes

#### 1.4. Busca por Proximidade

```
Request: Search by Location (Radius)
Method: GET
Expected: 200 OK
```

**Configure:**
- `userLatitude=-23.550520` (São Paulo)
- `userLongitude=-46.633308`
- `radiusKm=5`

**Validações:**
- ? Apenas igrejas no raio retornadas
- ? Campo `distance` presente
- ? Ordenação por distância funcionando

#### 1.5. Busca por Comodidades

```
Request: Search with Amenities Filter
Method: GET
Expected: 200 OK
```

**Validações:**
- ? Apenas igrejas com amenities filtradas
- ? Múltiplos amenities funcionando (AND logic)

#### 1.6. Busca Nearby

```
Request: Get Nearby Churches
Method: GET
Expected: 200 OK
```

**Validações:**
- ? Igrejas próximas retornadas
- ? Distância calculada
- ? Limite de resultados respeitado

---

### 2?? Criação de Igreja

#### 2.1. Criar Igreja Simples (Requer Auth)

```
Request: Create Church
Method: POST
Expected: 200 OK
```

**Antes de testar:**
1. Configure `jwt_token` no environment
2. Verifique se o token é válido

**Validações:**
- ? Status 200
- ? `churchId` retornado
- ? Igreja criada no banco

**Possíveis erros:**
- ? 401 - Token inválido/expirado
- ? 400 - Validação falhou (campos obrigatórios)

#### 2.2. Criar Igreja com Admin

```
Request: Create Church with Admin
Method: POST
Expected: 200 OK
```

**Payload completo necessário:**
- Dados da igreja
- Dados do administrador
- Endereço do admin
- Documentos do admin

**Validações:**
- ? Igreja e admin criados
- ? Admin já pode fazer login
- ? Relacionamento correto

**Teste após criação:**
1. Faça login com o admin criado
2. Verifique se tem permissões Admin
3. Tente buscar a igreja criada

---

### 3?? Operações de Igreja (CRUD)

#### 3.1. Buscar Igreja por ID

```
Request: Get Church by ID
Method: GET
Expected: 200 OK
```

**Pré-requisito:**
- Atualize `church_id` no environment

**Validações:**
- ? Todos os dados retornados
- ? Endereço incluído
- ? Lat/lng se disponível

#### 3.2. Atualizar Igreja (Admin Only)

```
Request: Update Church
Method: PUT
Expected: 200 OK
```

**Validações:**
- ? Dados atualizados
- ? Apenas admin pode atualizar
- ? Response com dados novos

**Teste de permissão:**
1. Tente com token de membro comum
2. Expected: 403 Forbidden

---

### 4?? Dados Bancários (Admin Only)

#### 4.1. Atualizar Banking Info

```
Request: Update Banking Info
Method: PUT
Expected: 200 OK
```

**Payload mínimo:**
```json
{
  "bankName": "Banco do Brasil",
  "agency": "1234-5",
  "account": "98765-4",
  "accountDigit": "3",
  "accountType": "Conta Corrente"
}
```

**Validações:**
- ? Dados salvos
- ? Apenas admin pode atualizar
- ? Campos sensíveis protegidos

**Teste de segurança:**
1. Faça GET da igreja
2. Verifique se dados bancários NÃO aparecem para não-admins

---

### 5?? Dashboard Administrativo

#### 5.1. Buscar Dashboard

```
Request: Get Church Dashboard
Method: GET
Expected: 200 OK
```

**Validações:**
- ? Métricas consolidadas
- ? Total de membros
- ? Total de eventos
- ? Finanças resumidas
- ? Apenas admin acessa

**Dados esperados:**
```json
{
  "totalMembers": 150,
  "totalEvents": 25,
  "totalDonations": 50000.00,
  "activeSubscription": {...},
  "recentActivity": [...]
}
```

---

### 6?? QR Code de Onboarding

#### 6.1. Gerar QR Code

```
Request: Generate Onboarding QR Code
Method: POST
Expected: 200 OK
```

**Validações:**
- ? QR Code Base64 retornado
- ? Salvo no banco para reuso
- ? Não gera duplicado se já existe

**Como validar QR Code:**
1. Copie o Base64 retornado
2. Cole em um decodificador online (https://base64.guru/converter/decode/image)
3. Verifique se é um QR Code válido

---

### 7?? Geolocalização (Rate Limited)

?? **IMPORTANTE**: Limite de 20 requisições por minuto!

#### 7.1. Atualizar Localização Manual

```
Request: Update Location Manually
Method: PUT
Expected: 200 OK
```

**Payload:**
```json
{
  "latitude": -23.550520,
  "longitude": -46.633308,
  "autoGeocode": false
}
```

**Validações:**
- ? Lat/lng salvos
- ? Método retornado: "manual"
- ? Busca nearby funciona após update

#### 7.2. Geocodificar Automaticamente

```
Request: Update Location via Geocoding
Method: PUT
Expected: 200 OK
```

**Pré-requisito:**
- Igreja deve ter endereço completo cadastrado

**Validações:**
- ? Lat/lng calculados via Google API
- ? `formattedAddress` retornado
- ? Método: "google_geocoding"

**Teste de erro:**
1. Igreja sem endereço completo
2. Expected: 400 Bad Request

#### 7.3. Endpoint Dedicado de Geocoding

```
Request: Geocode Church Location
Method: POST
Expected: 200 OK
```

**Validações:**
- ? Mesmo comportamento do anterior
- ? Mais semântico (POST vs PUT)

#### 7.4. Teste de Rate Limit

**Procedimento:**
1. Execute qualquer endpoint de geolocalização 21 vezes consecutivas
2. Expected: 429 Too Many Requests na 21ª requisição
3. Aguarde 1 minuto
4. Tente novamente
5. Expected: 200 OK

---

## ?? Testes de Validação

### Campos Obrigatórios

**Teste: Criar igreja sem nome**
```json
{
  "description": "Igreja sem nome",
  "phone": "11999999999"
}
```
Expected: 400 Bad Request

**Teste: Criar igreja sem endereço**
```json
{
  "name": "Igreja Teste",
  "phone": "11999999999",
  "address": {}
}
```
Expected: 400 Bad Request

### Validações de Formato

**Teste: Telefone inválido**
```json
{
  "phone": "abc123"
}
```
Expected: 400 - Formato inválido

**Teste: CEP inválido**
```json
{
  "address": {
    "zipCode": "12345"
  }
}
```
Expected: 400 - CEP deve ter 8 dígitos

### Testes de Permissão

**Cenário: Membro comum tenta atualizar igreja**
1. Configure token de membro comum
2. Tente `PUT /api/Church/{id}`
3. Expected: 403 Forbidden

**Cenário: Sem autenticação em endpoint protegido**
1. Remova `jwt_token` do header
2. Tente qualquer endpoint protegido
3. Expected: 401 Unauthorized

---

## ?? Checklist de Testes

### Busca Pública
- [ ] Busca básica funciona
- [ ] Filtro por cidade/estado funciona
- [ ] Filtro por rating funciona
- [ ] Filtro por amenities funciona
- [ ] Busca por proximidade funciona
- [ ] Paginação funciona
- [ ] Ordenação funciona

### CRUD
- [ ] Criar igreja simples
- [ ] Criar igreja com admin
- [ ] Buscar igreja por ID
- [ ] Atualizar igreja (admin)
- [ ] Permissões de admin validadas

### Finanças
- [ ] Atualizar dados bancários
- [ ] Dashboard carrega corretamente
- [ ] Dados sensíveis protegidos

### QR Code
- [ ] QR Code gerado corretamente
- [ ] Não gera duplicado

### Geolocalização
- [ ] Update manual funciona
- [ ] Geocoding automático funciona
- [ ] Rate limit funcionando
- [ ] Busca nearby usa lat/lng

### Segurança
- [ ] JWT validado corretamente
- [ ] Roles (Admin) respeitadas
- [ ] Dados sensíveis protegidos
- [ ] Rate limiting funcionando

---

## ?? Problemas Comuns

### SSL Certificate Error

**Solução:**
1. Postman > Settings
2. Desabilite "SSL certificate verification"

### Connection Refused

**Verificar:**
- [ ] API está rodando?
- [ ] Porta 7163 está correta?
- [ ] Firewall não está bloqueando?

### 401 Unauthorized

**Verificar:**
- [ ] Token configurado no environment?
- [ ] Token não expirou?
- [ ] Header Authorization presente?

### 429 Too Many Requests

**Solução:**
- Aguarde 1 minuto antes de tentar novamente
- Reduza frequência de testes de geolocalização

---

## ?? Métricas de Sucesso

### Cobertura de Testes

- ? 100% dos endpoints testados
- ? Todos os fluxos principais validados
- ? Casos de erro testados
- ? Permissões validadas
- ? Rate limiting validado

### Performance

- ? Busca pública < 500ms
- ? CRUD operations < 1s
- ? Geocoding < 2s (depende Google API)

---

**Happy Testing! ??**
