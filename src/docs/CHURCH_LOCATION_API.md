# ?? API de Localização de Igrejas - Documentação

## Endpoints Implementados

### 1. **Atualizar Localização da Igreja (Manual ou Automática)**

#### **PUT** `/api/church/{id}/location`
Atualiza latitude e longitude de uma igreja. Suporta atualização manual ou geocoding automático.

**Autorização:** Requer role `Admin`

**Request Body:**
```json
{
  "latitude": -22.9068,
  "longitude": -43.1729,
  "autoGeocode": false
}
```

**Parâmetros:**
- `latitude` (opcional): Latitude (-90 a 90)
- `longitude` (opcional): Longitude (-180 a 180)
- `autoGeocode` (bool): Se true, ignora lat/lng e busca via endereço cadastrado

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Localização atualizada com sucesso",
  "latitude": -22.9068,
  "longitude": -43.1729
}
```

**Response 400 Bad Request:**
```json
{
  "success": false,
  "message": "Latitude deve estar entre -90 e 90",
  "latitude": null,
  "longitude": null
}
```

---

### 2. **Geocodificar Localização Automaticamente**

#### **POST** `/api/church/{id}/geocode`
Busca automaticamente lat/lng usando o endereço cadastrado via Google Geocoding API.

**Autorização:** Requer role `Admin`

**Request:** Sem body

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Localização atualizada com sucesso",
  "latitude": -22.9068,
  "longitude": -43.1729
}
```

**Response 404 Not Found:**
```json
{
  "success": false,
  "message": "Igreja não encontrada",
  "latitude": null,
  "longitude": null
}
```

---

### 3. **Buscar Igrejas Próximas**

#### **GET** `/api/church/public/nearby`
Busca igrejas próximas a uma coordenada geográfica.

**Autorização:** Nenhuma (público)

**Query Parameters:**
- `lat` (required): Latitude do ponto de referência
- `lng` (required): Longitude do ponto de referência
- `radiusKm` (optional, default=5): Raio de busca em km
- `max` (optional, default=50): Máximo de resultados

**Request:**
```
GET /api/church/public/nearby?lat=-22.9068&lng=-43.1729&radiusKm=10&max=20
```

**Response 200 OK:**
```json
{
  "churches": [
    {
      "id": 1,
      "name": "Igreja Pentecostal",
      "latitude": -22.9050,
      "longitude": -43.1720,
      "distance": 0.25,
      "distanceUnit": "km",
      "address": "Rua das Flores, 123",
      "city": "Rio de Janeiro",
      "state": "RJ",
      "averageRating": 4.8,
      "totalReviews": 156
    }
  ],
  "totalFound": 5,
  "searchRadius": 10,
  "centerLat": -22.9068,
  "centerLng": -43.1729
}
```

---

## Validações Implementadas

### UpdateChurchLocationCommand
- ? ChurchId > 0
- ? Latitude entre -90 e 90 (quando fornecida)
- ? Longitude entre -180 e 180 (quando fornecida)
- ? Se AutoGeocode = true, lat/lng são ignorados
- ? Validação via FluentValidation

### Church.UpdateLocation()
- ? Validação de coordenadas na entidade
- ? Lança ArgumentException se inválido
- ? Atualiza timestamp automaticamente

---

## Fluxos de Uso

### 1. **Criar Igreja com Geocoding Automático**
Ao criar uma igreja via `POST /api/church`, o sistema já tenta geocodificar automaticamente usando o endereço fornecido.

```csharp
// Em CreateChurchCommandHandler
if (church.Address != null)
{
    var addressStr = $"{church.Address.Street}, {church.Address.Number}...";
    (double? lat, double? lng) = await _geocodingService.GetLatLongAsync(addressStr);
    if (lat.HasValue && lng.HasValue)
    {
        church.Latitude = lat;
        church.Longitude = lng;
    }
}
```

### 2. **Atualizar Manualmente (Frontend com Mapa)**
Frontend pode usar mapa interativo e enviar coordenadas:

```javascript
// Usuário clica no mapa e seleciona localização
const updateLocation = async (churchId, lat, lng) => {
  await fetch(`/api/church/${churchId}/location`, {
    method: 'PUT',
    headers: { 
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({
      latitude: lat,
      longitude: lng,
      autoGeocode: false
    })
  });
};
```

### 3. **Atualizar Automaticamente (Após Mudar Endereço)**
Quando o endereço é atualizado, pode-se chamar geocode:

```javascript
// Após atualizar endereço da igreja
const geocodeChurch = async (churchId) => {
  await fetch(`/api/church/${churchId}/geocode`, {
    method: 'POST',
    headers: { 
      'Authorization': `Bearer ${token}`
    }
  });
};
```

### 4. **Buscar Igrejas Próximas (Mapa Público)**
Frontend pode usar geolocalização do navegador:

```javascript
navigator.geolocation.getCurrentPosition(async (position) => {
  const { latitude, longitude } = position.coords;
  
  const response = await fetch(
    `/api/church/public/nearby?lat=${latitude}&lng=${longitude}&radiusKm=5`
  );
  
  const churches = await response.json();
  // Exibir igrejas no mapa
});
```

---

## Configuração Necessária

### appsettings.json
```json
{
  "GoogleGeocoding": {
    "ApiKey": "YOUR_GOOGLE_MAPS_API_KEY_HERE"
  }
}
```

### DependencyInjection
```csharp
// Já registrado em Infrastructure/DependencyInjection.cs
services.AddHttpClient<GoogleGeocodingService>();
```

---

## Melhorias Futuras

1. **Cache de Geocoding**
   - Evitar chamadas repetidas para mesmo endereço
   - Usar cache distribuído com key = hash do endereço

2. **Fallback para Outros Provedores**
   - Se Google falhar, tentar OpenStreetMap/Nominatim
   - Implementar circuit breaker

3. **Geocoding em Background**
   - Processar geocoding de forma assíncrona
   - Usar queue (RabbitMQ/Azure Service Bus)

4. **Validação de Endereço**
   - Verificar se endereço existe antes de salvar
   - Sugerir correções via Google Places API

5. **Histórico de Localizações**
   - Manter registro de mudanças de localização
   - Útil para auditoria
