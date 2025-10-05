# ?? Exemplos de Payloads JSON

Exemplos completos de payloads para facilitar os testes.

## ?? Create Church (Simple)

```json
{
  "name": "Igreja Nova Esperança",
  "description": "Igreja evangélica com foco em jovens e famílias",
  "phone": "11987654321",
  "planId": 1,
  "document": "12345678000190",
  "logo": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/...",
  "address": {
    "street": "Rua das Flores",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01234-567",
    "country": "Brasil",
    "neighborhood": "Centro",
    "number": "100"
  }
}
```

## ??? Create Church with Admin

```json
{
  "name": "Igreja Pentecostal Renovada",
  "description": "Igreja focada em famílias e ministérios",
  "phone": "11999887766",
  "logo": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "document": "98765432000111",
  "address": {
    "street": "Av. Paulista",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01310-100",
    "country": "Brasil",
    "neighborhood": "Bela Vista",
    "number": "1500"
  },
  "adminName": "Pastor João Silva",
  "adminEmail": "pastor@igrejapentecostal.com.br",
  "adminPhone": "11987654321",
  "adminBirthDate": "1980-05-15",
  "adminIsBaptized": true,
  "adminBaptizedDate": "1995-12-25",
  "adminIsTither": true,
  "adminPassword": "SenhaForte@123!",
  "adminBirthCity": "São Paulo",
  "adminBirthState": "SP",
  "ministry": "Louvor e Adoração",
  "memberSince": "2010-01-01",
  "maritalStatus": "Married",
  "notes": "Pastor fundador da igreja, com 30 anos de experiência ministerial",
  "adminAddress": {
    "street": "Rua das Palmeiras",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01234-567",
    "country": "Brasil",
    "neighborhood": "Jardins",
    "number": "200"
  },
  "adminDocuments": [
    {
      "type": 1,
      "number": "12345678900"
    },
    {
      "type": 2,
      "number": "123456789"
    }
  ]
}
```

### Document Types
```
1 = CPF
2 = RG
3 = CNH
4 = Passaporte
5 = Título de Eleitor
```

### Marital Status Options
```
"Single" = Solteiro(a)
"Married" = Casado(a)
"Divorced" = Divorciado(a)
"Widowed" = Viúvo(a)
```

## ?? Update Church

```json
{
  "name": "Igreja Nova Esperança - Filial Centro",
  "phone": "11987654322",
  "description": "Igreja evangélica renovada com cultos modernos",
  "address": {
    "street": "Rua das Flores",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01234-567",
    "country": "Brasil",
    "neighborhood": "Centro",
    "number": "101",
    "complement": "Sala 5, 2º andar"
  }
}
```

## ?? Update Banking Info

```json
{
  "bankName": "Banco do Brasil",
  "agency": "1234-5",
  "account": "98765-4",
  "accountDigit": "3",
  "accountType": "Conta Corrente"
}
```

### Banking Info - PIX Example

```json
{
  "bankName": "Itaú",
  "agency": "0001",
  "account": "12345",
  "accountDigit": "6",
  "accountType": "Conta Corrente",
  "pixKey": "igreja@pix.com.br",
  "pixKeyType": "Email"
}
```

## ?? Update Location Manually

```json
{
  "latitude": -23.550520,
  "longitude": -46.633308,
  "autoGeocode": false
}
```

## ??? Update Location via Geocoding

```json
{
  "autoGeocode": true
}
```

## ?? Query Parameters Examples

### Search Public Churches - Full Example

```
GET /api/Church/public/search
?search=pentecostal
&city=São Paulo
&state=SP
&minRating=4.0
&minReviews=10
&hasServiceToday=true
&userLatitude=-23.550520
&userLongitude=-46.633308
&radiusKm=5
&sortBy=rating
&amenities=estacionamento
&amenities=acessibilidade
&amenities=transmissao_online
&denominations=Assembleia de Deus
&denominations=Batista
&page=1
&pageSize=20
```

### Get Nearby Churches

```
GET /api/Church/public/nearby
?lat=-23.550520
&lng=-46.633308
&radiusKm=10
&max=50
```

## ?? Logo Base64 Examples

### Minimal Valid Base64

```json
{
  "logo": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=="
}
```

### Tips for Base64 Images

1. **Tamanho máximo recomendado**: 5MB
2. **Formatos aceitos**: JPEG, PNG, GIF
3. **Prefixo obrigatório**: `data:image/{tipo};base64,`
4. **Exemplo completo**:
   ```
   data:image/jpeg;base64,/9j/4AAQSkZJRgABA...
   ```

## ?? Response Examples

### Successful Church Creation

```json
{
  "churchId": 123,
  "message": "Igreja criada com sucesso"
}
```

### Church Details Response

```json
{
  "id": 1,
  "name": "Igreja Nova Esperança",
  "description": "Igreja evangélica",
  "phone": "11987654321",
  "logo": "https://s3.amazonaws.com/bucket/logo.jpg",
  "address": {
    "street": "Rua das Flores",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01234-567",
    "country": "Brasil",
    "neighborhood": "Centro",
    "number": "100"
  },
  "latitude": -23.550520,
  "longitude": -46.633308,
  "averageRating": 4.5,
  "totalReviews": 42,
  "isVerified": true,
  "hasParking": true,
  "isAccessible": true,
  "hasLiveStream": true,
  "denomination": "Assembleia de Deus"
}
```

### Public Search Response

```json
{
  "items": [
    {
      "id": 1,
      "name": "Igreja Pentecostal",
      "city": "São Paulo",
      "state": "SP",
      "averageRating": 4.8,
      "totalReviews": 156,
      "distance": 2.3,
      "hasServiceToday": true,
      "nextServiceTime": "19:00",
      "coverPhoto": "https://...",
      "denomination": "Assembleia de Deus"
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

### Location Update Response

```json
{
  "success": true,
  "message": "Localização atualizada com sucesso",
  "latitude": -23.550520,
  "longitude": -46.633308,
  "method": "manual"
}
```

### Geocoding Response

```json
{
  "success": true,
  "message": "Endereço geocodificado com sucesso",
  "latitude": -23.550520,
  "longitude": -46.633308,
  "formattedAddress": "Av. Paulista, 1500 - Bela Vista, São Paulo - SP, 01310-100, Brasil",
  "method": "google_geocoding"
}
```

## ? Error Responses

### 400 Bad Request

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [
      "O nome da igreja é obrigatório"
    ],
    "Phone": [
      "Telefone inválido"
    ]
  }
}
```

### 401 Unauthorized

```json
{
  "message": "Token de autenticação inválido ou expirado"
}
```

### 403 Forbidden

```json
{
  "message": "Você não tem permissão para acessar este recurso"
}
```

### 404 Not Found

```json
{
  "message": "Igreja não encontrada"
}
```

### 429 Too Many Requests

```json
{
  "message": "Muitas requisições. Tente novamente em 1 minuto.",
  "retryAfter": 60
}
```

## ?? Test Data Sets

### Small Church (Minimal Data)

```json
{
  "name": "Congregação Pequena",
  "description": "Igreja local",
  "phone": "11999999999",
  "planId": 1,
  "document": "00000000000100",
  "address": {
    "street": "Rua A",
    "city": "Cidade",
    "state": "SP",
    "zipCode": "00000-000",
    "country": "Brasil",
    "neighborhood": "Centro",
    "number": "1"
  }
}
```

### Large Church (Full Data)

```json
{
  "name": "Catedral Internacional da Fé",
  "description": "Mega igreja com múltiplos ministérios, cultos em 3 horários diários, escola bíblica, ministério infantil premiado, coral profissional e transmissão online 24/7",
  "phone": "11987654321",
  "planId": 3,
  "document": "12345678000190",
  "logo": "data:image/jpeg;base64,/9j/4AAQ...",
  "address": {
    "street": "Av. Principal",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01310-100",
    "country": "Brasil",
    "neighborhood": "Centro",
    "number": "1000",
    "complement": "Torre A - Auditório Principal"
  }
}
```

---

**Dica**: Use esses exemplos como base e ajuste conforme necessário! ??
