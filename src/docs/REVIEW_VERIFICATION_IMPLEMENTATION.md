# ?? Sistema de Verificação de Reviews - Documentação Completa

## ? Implementação Concluída

Sistema de verificação de reviews baseado em presença confirmada (check-in), garantindo que apenas visitantes reais possam avaliar igrejas.

---

## ?? O Que Foi Implementado

### 1. **Campos de Verificação na Review**
- ? `IsVerified` - Review verificada por check-in
- ? `WorshipPresenceId` - ID da presença que valida a review
- ? `VerifiedAt` - Data/hora da verificação
- ? Métodos `MarkAsVerified()` e `RemoveVerification()`

**Localização:** `Domain/MyChurch.Domain/Entities/Review.cs`

### 2. **Serviço de Verificação**
Interface `IReviewVerificationService` com métodos:
- ? `HasMemberVisitedChurchAsync()` - Verifica se membro visitou igreja
- ? `HasVisitorVisitedChurchAsync()` - Verifica visitante anônimo
- ? `GetLatestPresenceIdAsync()` - Busca ID da última presença
- ? `CanMemberReviewChurchAsync()` - Valida se pode avaliar

**Localização:** 
- `Domain/MyChurch.Domain/Services/IReviewVerificationService.cs`
- `Infrastructure/MyChurch.Infrastructure/Services/ReviewVerificationService.cs`

### 3. **Regras de Validação**

#### ? **Verificação de Presença**
- Membro deve ter presença confirmada (check-in) na igreja
- Presença deve ser dos últimos **6 meses**
- Presença mais recente é usada para verificação

#### ? **Restrições**
- Um membro só pode avaliar uma igreja uma vez
- Review não verificada é aceita, mas sem badge
- Admin pode fazer bypass com `SkipVerification = true`

### 4. **Comando Atualizado**
`SubmitReviewCommand` agora:
- ? Valida presença antes de criar review
- ? Vincula review à presença (WorshipPresenceId)
- ? Marca automaticamente como verificada se válida
- ? Retorna `IsVerified` no resultado

**Localização:** `Application/MyChurch.Application/Reviews/Commands/SubmitReview/SubmitReviewCommand.cs`

### 5. **Nova Query: CanReviewChurch**
Verifica se membro pode avaliar ANTES de tentar:
```csharp
{
  "canReview": true,
  "reason": null,
  "hasVisited": true,
  "lastVisitDate": "2024-12-15T10:30:00Z",
  "hasExistingReview": false
}
```

**Localização:** `Application/MyChurch.Application/Reviews/Queries/CanReviewChurch/CanReviewChurchQuery.cs`

### 6. **Endpoints da API**

#### **GET** `/api/reviews/can-review/{churchId}`
Verifica se usuário logado pode avaliar a igreja
```json
{
  "canReview": false,
  "reason": "Você precisa visitar esta igreja antes de avaliá-la.",
  "hasVisited": false,
  "lastVisitDate": null,
  "hasExistingReview": false
}
```

#### **POST** `/api/reviews`
Cria review com verificação automática
```json
{
  "entityId": 1,
  "entityType": "Church",
  "reviewerId": 123,
  "score": 5,
  "comment": "Excelente culto!"
}
```

**Resposta:**
```json
{
  "success": true,
  "message": "Avaliação verificada enviada com sucesso!",
  "reviewId": 456,
  "isVerified": true
}
```

### 7. **DTOs Atualizados**

#### ReviewDto
- ? `IsVerified` - Badge de verificação
- ? `VerifiedAt` - Data da verificação

#### ReviewSummaryDto (Busca Pública)
- ? `IsVerified` - Mostra badge nas top reviews

### 8. **Migration**
- ? Adiciona colunas: `IsVerified`, `WorshipPresenceId`, `VerifiedAt`
- ? Foreign key para `WorshipPresences`
- ? Índice em `WorshipPresenceId`

**Localização:** `Infrastructure/MyChurch.Infrastructure/Migrations/[timestamp]_AddReviewVerificationFields.cs`

---

## ?? Como Usar

### 1. **Aplicar Migration**
```bash
dotnet ef database update --startup-project Web/MyChurch.Api.Web --project Infrastructure/MyChurch.Infrastructure
```

### 2. **Fluxo do Frontend**

#### Passo 1: Verificar se pode avaliar
```javascript
GET /api/reviews/can-review/1
Authorization: Bearer {token}

// Resposta
{
  "canReview": true,
  "hasVisited": true,
  "lastVisitDate": "2024-12-15T10:30:00Z"
}
```

#### Passo 2: Mostrar formulário apenas se `canReview = true`
```javascript
if (result.canReview) {
  // Exibe formulário de avaliação
} else {
  // Exibe mensagem: result.reason
  // Ex: "Você precisa visitar esta igreja antes de avaliá-la"
}
```

#### Passo 3: Enviar avaliação
```javascript
POST /api/reviews
{
  "entityId": 1,
  "entityType": "Church",
  "reviewerId": 123,
  "score": 5,
  "comment": "Excelente experiência!"
}

// Resposta
{
  "success": true,
  "isVerified": true,
  "message": "Avaliação verificada enviada com sucesso!"
}
```

### 3. **Exibir Badge de Verificação**
```javascript
// Reviews com badge de verificação
{
  "id": 1,
  "score": 5,
  "comment": "Ótimo culto!",
  "isVerified": true,  // ? Exibe badge "Visita Confirmada"
  "verifiedAt": "2024-12-15T14:20:00Z"
}
```

---

## ?? Regras de Negócio

### ? **Pode Avaliar SE:**
1. Tem presença confirmada (check-in) na igreja
2. Presença foi nos últimos 6 meses
3. Ainda não avaliou esta igreja

### ? **NÃO Pode Avaliar SE:**
1. Nunca visitou a igreja (sem check-in)
2. Última visita > 6 meses atrás
3. Já tem review desta igreja

### ?? **Bypass (Admin)**
```csharp
// Admin pode forçar review sem verificação
{
  "skipVerification": true,
  "entityId": 1,
  "score": 5,
  "comment": "Review admin"
}
```

---

## ?? Mensagens de Validação

| Cenário | Mensagem |
|---------|----------|
| Sem presença | "Você precisa visitar esta igreja antes de avaliá-la." |
| Visita antiga (>6 meses) | "Sua última visita foi há mais de 6 meses. Visite novamente para poder avaliar." |
| Já avaliou | "Você já avaliou esta igreja." |
| Sucesso verificado | "Avaliação verificada enviada com sucesso!" |
| Sucesso não verificado | "Avaliação enviada com sucesso" |

---

## ?? Benefícios

### **Credibilidade**
- ? Reviews verificadas têm badge especial
- ? Visitantes reais = avaliações confiáveis
- ? Reduz fake reviews

### **Engajamento**
- ? Incentiva check-in presencial
- ? Conecta presença física com feedback digital
- ? Gamificação (badge de verificação)

### **Qualidade**
- ? Reviews relevantes (últimos 6 meses)
- ? Uma avaliação por pessoa
- ? Feedback baseado em experiência real

---

## ?? Melhorias Futuras Sugeridas

### 1. **Verificação Geolocalizada**
Validar que check-in foi feito próximo à igreja:
```csharp
var distance = CalculateDistance(presence.Latitude, church.Latitude);
if (distance > 500) // 500m de raio
{
    return (false, "Check-in deve ser feito na igreja");
}
```

### 2. **Níveis de Verificação**
- ? Verificado - 1 visita
- ? Super Verificado - 3+ visitas
- ?? Elite Verificado - 10+ visitas

### 3. **Pontos de Fé por Review Verificada**
```csharp
if (review.IsVerified) 
{
    member.TotalFaithPoints += 10; // Bônus por review verificada
}
```

### 4. **Filtro de Reviews Verificadas**
```csharp
// Na GetReviewsQuery
public bool? OnlyVerified { get; set; }

// ...
if (request.OnlyVerified == true)
{
    query = query.Where(r => r.IsVerified);
}
```

### 5. **Notificação de Review Pendente**
Após check-in, notificar membro:
> "Você visitou a [Igreja X]. Que tal avaliar sua experiência?"

---

## ?? Integração com Check-In

### Fluxo Completo:
1. Membro faz check-in no culto (WorshipPresence criada)
2. Sistema valida localização e horário
3. Presença é confirmada
4. Após 24h, membro pode avaliar a igreja
5. Review é automaticamente verificada
6. Badge de verificação aparece na review

---

## ? Checklist Final

- [x] Campos de verificação na Review
- [x] ReviewVerificationService implementado
- [x] SubmitReviewCommand com validação
- [x] CanReviewChurchQuery criada
- [x] Endpoint GET /can-review/{churchId}
- [x] DTOs atualizados com IsVerified
- [x] Migration criada
- [x] Dependency Injection configurado
- [x] Regras de 6 meses implementadas
- [x] Bypass para admin
- [x] Compilação bem-sucedida
- [x] Documentação completa

---

## ?? Status: **PRONTO PARA USO!** ?

O sistema de verificação de reviews está 100% funcional e integrado.
Reviews verificadas aumentam a credibilidade da plataforma!

---

## ?? Exemplo de UI

### Badge de Verificação
```html
<!-- Review verificada -->
<div class="review verified">
  <span class="badge">
    ? Visita Confirmada
  </span>
  <p class="comment">Excelente culto!</p>
  <small>Verificado em 15/12/2024</small>
</div>

<!-- Review não verificada -->
<div class="review">
  <p class="comment">Boa experiência</p>
  <small>Não verificado</small>
</div>
```

### Mensagem ao Tentar Avaliar
```html
<!-- Se não pode avaliar -->
<div class="alert alert-warning">
  <strong>Você ainda não pode avaliar esta igreja</strong>
  <p>Visite um culto e faça check-in para desbloquear avaliações!</p>
  <button>Ver Próximos Cultos</button>
</div>
```
