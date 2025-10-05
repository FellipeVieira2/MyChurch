# ?? Busca Avançada de Igrejas - Guia Completo

Guia completo para utilizar a **Busca Avançada de Igrejas** com filtros por denominação, amenidades, idiomas, avaliações e muito mais.

---

## ?? Overview

O sistema permite busca avançada de igrejas com:

### ?? Filtros Disponíveis
- **Denominação** - Batista, Assembleia de Deus, Adventista, Católica, etc
- **Amenidades** - Estacionamento, Acessibilidade, Transmissão Online, Ministério Infantil
- **Localização** - Cidade, Estado, Raio de distância
- **Avaliação** - Rating mínimo, quantidade mínima de reviews
- **Idiomas** - Português, Inglês, Espanhol, etc

### ?? Ordenações
- **Relevância** (padrão) - Score calculado por rating + reviews + distância
- **Rating** - Melhor avaliadas primeiro
- **Distância** - Mais próximas primeiro
- **Reviews** - Mais avaliadas primeiro
- **Mais Recentes** - Últimas adicionadas

---

## ?? ENDPOINTS

### 1. Busca Pública (GET /api/Church/public/search)

**Não requer autenticação**

#### Parâmetros da Query

| Parâmetro | Tipo | Descrição | Exemplo |
|-----------|------|-----------|---------|
| `search` | string | Termo de busca (nome, descrição) | `igreja` |
| `city` | string | Cidade | `São Paulo` |
| `state` | string | Estado (UF) | `SP` |
| `denominations` | string[] | Denominações | `Batista`, `Assembleia de Deus` |
| `amenities` | string[] | Amenidades | `estacionamento`, `acessibilidade` |
| `languages` | string[] | Idiomas | `Português`, `Inglês` |
| `minRating` | double | Rating mínimo (0-5) | `4.0` |
| `minReviews` | int | Mínimo de avaliações | `10` |
| `userLatitude` | double | Latitude do usuário | `-23.550520` |
| `userLongitude` | double | Longitude do usuário | `-46.633308` |
| `radiusKm` | double | Raio de busca (km) | `5` |
| `sortBy` | string | Ordenação | `relevance`, `rating`, `distance` |
| `page` | int | Página | `1` |
| `pageSize` | int | Items por página | `10` |

---

### 2. Atualizar Características (PUT /api/Church/characteristics)

**Requer autenticação: Admin**

#### Request Body

```json
{
  "denomination": "Batista",
  "coverPhoto": "https://s3.amazonaws.com/mybucket/cover.jpg",
  "hasParking": true,
  "isAccessible": true,
  "hasLiveStream": true,
  "hasChildMinistry": true,
  "languages": ["Português", "Inglês", "Espanhol"]
}
```

#### Response
```json
{
  "message": "Características da igreja atualizadas com sucesso"
}
```

---

## ?? EXEMPLOS DE USO

### 1. Buscar por Denominação

```http
GET /api/Church/public/search?denominations=Batista&page=1
```

**Caso de uso:** Encontrar igrejas Batistas

---

### 2. Buscar Múltiplas Denominações

```http
GET /api/Church/public/search?denominations=Batista&denominations=Assembleia de Deus&page=1
```

**Caso de uso:** Listar igrejas evangélicas tradicionais

---

### 3. Buscar com Estacionamento

```http
GET /api/Church/public/search?amenities=estacionamento&sortBy=rating&page=1
```

**Caso de uso:** Igreja para ir de carro

---

### 4. Buscar Igrejas Acessíveis

```http
GET /api/Church/public/search?amenities=acessibilidade&city=São Paulo&page=1
```

**Caso de uso:** Cadeirante procurando igreja

---

### 5. Buscar com Transmissão Online

```http
GET /api/Church/public/search?amenities=transmissao_online&sortBy=reviews&page=1
```

**Caso de uso:** Assistir cultos online

---

### 6. Buscar com Ministério Infantil

```http
GET /api/Church/public/search?amenities=ministerio_infantil&page=1
```

**Caso de uso:** Famílias com crianças

---

### 7. Buscar Todas as Amenidades

```http
GET /api/Church/public/search?amenities=estacionamento&amenities=acessibilidade&amenities=transmissao_online&amenities=ministerio_infantil&minRating=4.0&page=1
```

**Caso de uso:** Igreja completa e bem avaliada

---

### 8. Top Rated (Mais Bem Avaliadas)

```http
GET /api/Church/public/search?minRating=4.5&minReviews=20&sortBy=rating&page=1
```

**Caso de uso:** Melhores igrejas da região

---

### 9. Busca Próxima com Filtros

```http
GET /api/Church/public/search?userLatitude=-23.550520&userLongitude=-46.633308&radiusKm=10&amenities=estacionamento&minRating=4.0&sortBy=distance&page=1
```

**Caso de uso:** Igreja próxima com estacionamento e bem avaliada

---

### 10. Busca por Relevância

```http
GET /api/Church/public/search?city=São Paulo&sortBy=relevance&page=1
```

**Caso de uso:** Melhores igrejas de São Paulo (score: rating + reviews + proximidade)

---

## ?? VALORES DE AMENIDADES

### Amenidades Suportadas

| Valor | Descrição | Mapeamento no Banco |
|-------|-----------|---------------------|
| `estacionamento` | Possui estacionamento | `HasParking = true` |
| `acessibilidade` | Acessível (rampa, elevador) | `IsAccessible = true` |
| `transmissao_online` | Transmissão ao vivo | `HasLiveStream = true` |
| `ministerio_infantil` | Ministério/Escola Infantil | `HasChildMinistry = true` |

---

## ?? IDIOMAS SUPORTADOS

```json
{
  "languages": [
    "Português",
    "Inglês",
    "Espanhol",
    "Francês",
    "Alemão",
    "Italiano",
    "Coreano",
    "Japonês",
    "Mandarim",
    "Russo",
    "Árabe"
  ]
}
```

---

## ??? DENOMINAÇÕES COMUNS

```
- Batista
- Assembleia de Deus
- Adventista do Sétimo Dia
- Presbiteriana
- Metodista
- Luterana
- Pentecostal
- Católica Apostólica Romana
- Congregação Cristã no Brasil
- Universal do Reino de Deus
- Internacional da Graça de Deus
- Quadrangular
- Sara Nossa Terra
- Renascer em Cristo
- Mundial do Poder de Deus
- Comunidade Evangélica
- Não Denominacional
```

---

## ?? RESPONSE DA BUSCA

### Estrutura do ChurchPublicListItemDto

```json
{
  "items": [
    {
      "id": 1,
      "name": "Igreja Batista Central",
      "description": "Igreja evangélica com foco em jovens",
      "logo": "https://s3.amazonaws.com/logo.jpg",
      "city": "São Paulo",
      "state": "SP",
      "hasServiceToday": true,
      "nextServiceStartTime": "2024-10-06T10:00:00Z",
      "latitude": -23.550520,
      "longitude": -46.633308,
      "distanceKm": 2.5,
      
      // Avaliações
      "averageRating": 4.8,
      "totalReviews": 145,
      "ratingDistribution": {
        "5": 120,
        "4": 20,
        "3": 3,
        "2": 1,
        "1": 1
      },
      "topReviews": [
        {
          "id": 1,
          "reviewerName": "João Silva",
          "score": 5,
          "comment": "Igreja maravilhosa! Louvor excelente...",
          "createdAt": "2024-09-15T14:30:00Z",
          "helpfulCount": 25,
          "reviewerPhoto": "https://s3.amazonaws.com/user1.jpg",
          "isVerified": true
        }
      ],
      
      // Amenidades
      "photos": [
        "https://s3.amazonaws.com/photo1.jpg",
        "https://s3.amazonaws.com/photo2.jpg"
      ],
      "amenities": [
        "Estacionamento",
        "Acessibilidade",
        "Transmissão Online",
        "Ministério Infantil"
      ],
      
      // Outros
      "isVerified": true,
      "priceLevel": "Gratuito",
      "capacityEstimate": 500,
      "hasLiveStream": true,
      "responseRate": "95%",
      "averageResponseTime": "00:02:30",
      "relevanceScore": 87.5
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10
}
```

---

## ?? ATUALIZAR CARACTERÍSTICAS

### Cenário 1: Igreja Completa

```json
{
  "denomination": "Batista",
  "coverPhoto": "https://s3.amazonaws.com/cover.jpg",
  "hasParking": true,
  "isAccessible": true,
  "hasLiveStream": true,
  "hasChildMinistry": true,
  "languages": ["Português", "Inglês"]
}
```

### Cenário 2: Igreja Online

```json
{
  "hasLiveStream": true,
  "languages": ["Português", "Inglês", "Espanhol"]
}
```

### Cenário 3: Igreja Familiar

```json
{
  "hasChildMinistry": true,
  "hasParking": true,
  "isAccessible": true
}
```

### Cenário 4: Igreja Acessível

```json
{
  "isAccessible": true,
  "hasParking": true
}
```

---

## ?? TESTES E VALIDAÇÕES

### ? Checklist de Testes

#### Busca por Denominação
- [ ] Buscar uma denominação específica
- [ ] Buscar múltiplas denominações
- [ ] Verificar retorno correto

#### Busca por Amenidades
- [ ] Buscar com estacionamento
- [ ] Buscar igrejas acessíveis
- [ ] Buscar com transmissão online
- [ ] Buscar com ministério infantil
- [ ] Buscar com todas as amenidades
- [ ] Verificar filtros combinados

#### Busca por Avaliação
- [ ] Filtrar por rating mínimo (4.0+)
- [ ] Filtrar por quantidade de reviews (20+)
- [ ] Ordenar por rating (melhor primeiro)
- [ ] Ordenar por quantidade de reviews

#### Busca por Localização
- [ ] Buscar por cidade
- [ ] Buscar por estado
- [ ] Buscar por raio (5km, 10km, 20km)
- [ ] Ordenar por distância
- [ ] Combinar localização + amenidades

#### Ordenação
- [ ] Ordenar por relevância (padrão)
- [ ] Ordenar por rating
- [ ] Ordenar por distância
- [ ] Ordenar por reviews
- [ ] Ordenar por mais recentes

#### Atualização de Características
- [ ] Atualizar denominação
- [ ] Habilitar todas amenidades
- [ ] Adicionar múltiplos idiomas
- [ ] Atualização parcial (apenas alguns campos)
- [ ] Testar sem autenticação (erro 401)
- [ ] Testar com usuário não-admin (erro 403)

---

## ?? SCORE DE RELEVÂNCIA

### Como é Calculado?

```csharp
double score = 0;

// Peso para avaliação (40%)
score += church.AverageRating * 8;  // Máx: 40 pontos

// Peso para quantidade de reviews (30%)
score += Math.Min(church.TotalReviews / 10.0, 15);  // Máx: 15 pontos

// Peso para proximidade (30% se localização fornecida)
if (hasUserLocation && church.DistanceKm.HasValue)
{
    var distanceScore = Math.Max(0, 15 - (church.DistanceKm.Value * 0.5));
    score += distanceScore;  // Máx: 15 pontos
}

// Bônus para igrejas verificadas (+5 pontos)
if (church.IsVerified)
    score += 5;

// Bônus para igrejas com culto hoje (+3 pontos)
if (church.HasServiceToday)
    score += 3;

return Math.Round(score, 2);  // Máx: ~78 pontos
```

### Exemplos de Score

| Igreja | Rating | Reviews | Distância | Verificada | Culto Hoje | Score |
|--------|--------|---------|-----------|------------|------------|-------|
| Igreja A | 5.0 | 100+ | 2km | ? | ? | 76.0 |
| Igreja B | 4.8 | 50 | 5km | ? | ? | 63.5 |
| Igreja C | 4.5 | 20 | 10km | ? | ? | 51.0 |
| Igreja D | 3.8 | 5 | 15km | ? | ? | 30.9 |

---

## ?? TRATAMENTO DE ERROS

### 400 Bad Request
```json
{
  "errors": {
    "Languages": ["Lista de idiomas inválida"]
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
  "errors": {
    "Permission": ["Apenas administradores podem atualizar as características da igreja"]
  }
}
```

---

## ?? DICAS DE USO

### 1. Combine Filtros
```
?amenities=estacionamento&amenities=acessibilidade&minRating=4.0&sortBy=distance
```

### 2. Use Ordenação Adequada
- **Relevância** - Melhor experiência geral
- **Rating** - Mais confiáveis
- **Distância** - Mais próximas
- **Reviews** - Mais populares

### 3. Filtros Progressivos
1. Primeiro: Localização (cidade/raio)
2. Depois: Amenidades necessárias
3. Por último: Rating mínimo

### 4. Atualize Gradualmente
```json
// Passo 1: Denominação
{ "denomination": "Batista" }

// Passo 2: Amenidades básicas
{ "hasParking": true, "isAccessible": true }

// Passo 3: Recursos online
{ "hasLiveStream": true }

// Passo 4: Idiomas
{ "languages": ["Português", "Inglês"] }
```

---

## ?? BENEFÍCIOS

### Para Usuários
- ? Encontra igrejas próximas rapidamente
- ? Filtra por necessidades específicas (estacionamento, acessibilidade)
- ? Vê avaliações de outros visitantes
- ? Encontra igrejas em seu idioma
- ? Descobre cultos com transmissão online

### Para Igrejas
- ? Maior visibilidade na busca
- ? Atrai público específico (famílias, estrangeiros, etc)
- ? Destaque por avaliações positivas
- ? Alcança pessoas próximas
- ? Promove amenidades e diferenciais

---

## ?? ROADMAP FUTURO

### Próximas Features
- [ ] Filtro por horário de culto
- [ ] Filtro por estilo musical (tradicional, contemporâneo)
- [ ] Filtro por faixa etária predominante
- [ ] Mapa interativo com pins
- [ ] Sugestões personalizadas (IA)
- [ ] Comparação lado a lado

---

**?? Pronto para buscar igrejas!**

Execute as requests no Postman e explore todas as possibilidades de filtros.

**Data**: 05/10/2024  
**Versão**: 1.0.0  
**Status**: ? Production Ready  
**Endpoints**: 2 (GET /public/search, PUT /characteristics)
