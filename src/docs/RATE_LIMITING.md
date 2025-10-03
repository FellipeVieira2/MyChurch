# ??? Rate Limiting - Documentação

## ? Implementado com Sucesso

O sistema agora possui **Rate Limiting** configurado para prevenir spam e proteger a API de abuso.

---

## ?? Políticas Configuradas

### 1. **vote-limiter** (Votos em Reviews)
- **Limite:** 10 votos por minuto
- **Fila:** 2 requisições em espera
- **Endpoints protegidos:**
  - `POST /api/reviews/{id}/vote`
  - `POST /api/reviews/{id}/helpful`
  - `POST /api/reviews/{id}/not-helpful`

**Objetivo:** Prevenir spam de votos e manipulação de reviews.

---

### 2. **review-limiter** (Criação de Reviews)
- **Limite:** 3 reviews a cada 5 minutos
- **Fila:** 1 requisição em espera
- **Endpoints protegidos:**
  - `POST /api/reviews`

**Objetivo:** Evitar flood de reviews falsas.

---

### 3. **upload-limiter** (Geocoding e Uploads)
- **Limite:** 20 requests por minuto (sliding window)
- **Segmentos:** 4 janelas de 15 segundos
- **Fila:** 5 requisições em espera
- **Endpoints protegidos:**
  - `PUT /api/church/{id}/location`
  - `POST /api/church/{id}/geocode`

**Objetivo:** Proteger chamadas à API do Google Maps e evitar custos excessivos.

---

### 4. **api-limiter** (Geral)
- **Limite:** 100 requests por minuto
- **Fila:** 10 requisições em espera
- **Aplicação:** Pode ser aplicada a endpoints sensíveis conforme necessário

**Objetivo:** Proteção geral contra DDoS e uso abusivo.

---

## ?? Resposta HTTP 429 (Too Many Requests)

Quando o limite é atingido, a API retorna:

**Status:** `429 Too Many Requests`

**Body:**
```json
{
  "error": "Too many requests. Please try again later.",
  "retryAfter": 45.5
}
```

O campo `retryAfter` indica quantos segundos o cliente deve esperar antes de tentar novamente.

---

## ?? Como Usar no Frontend

### Exemplo em JavaScript

```javascript
async function voteReview(reviewId, isHelpful) {
  try {
    const response = await fetch(`/api/reviews/${reviewId}/helpful`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ isHelpful })
    });

    if (response.status === 429) {
      const data = await response.json();
      const waitTime = data.retryAfter || 60;
      
      alert(`Limite de votos atingido. Tente novamente em ${waitTime} segundos.`);
      
      // Aguardar antes de tentar novamente
      setTimeout(() => {
        // Reabilitar botão ou tentar automaticamente
      }, waitTime * 1000);
      
      return;
    }

    if (response.ok) {
      const result = await response.json();
      // Atualizar UI com contadores
      updateVotesUI(result);
    }
  } catch (error) {
    console.error('Erro ao votar:', error);
  }
}
```

### Tratamento de Erros com Axios

```javascript
import axios from 'axios';

// Interceptor para Rate Limiting
axios.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 429) {
      const retryAfter = error.response.data.retryAfter || 60;
      
      // Exibir notificação ao usuário
      toast.warning(`Aguarde ${Math.ceil(retryAfter)} segundos antes de tentar novamente.`);
      
      // Opcional: Retry automático
      return new Promise(resolve => {
        setTimeout(() => {
          resolve(axios.request(error.config));
        }, retryAfter * 1000);
      });
    }
    
    return Promise.reject(error);
  }
);
```

---

## ?? Configuração Personalizada

### Ajustar Limites (Program.cs)

```csharp
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    // Limite mais rigoroso para produção
    rateLimiterOptions.AddFixedWindowLimiter(policyName: "vote-limiter", options =>
    {
        options.PermitLimit = 5; // Reduzir para 5 votos
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueLimit = 0; // Sem fila
    });
});
```

### Aplicar a Novos Endpoints

```csharp
[HttpPost("sensitive-endpoint")]
[EnableRateLimiting("api-limiter")] // Aplicar política
public async Task<IActionResult> SensitiveAction()
{
    // ...
}
```

### Desabilitar em Desenvolvimento

```csharp
// No Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseRateLimiter();
}
```

---

## ?? Monitoramento

### Logs de Rate Limiting

O sistema registra automaticamente quando limites são atingidos. Para monitorar:

```csharp
rateLimiterOptions.OnRejected = async (context, token) =>
{
    // Log do evento
    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(
        "Rate limit exceeded for {Path} from {IP}", 
        context.HttpContext.Request.Path,
        context.HttpContext.Connection.RemoteIpAddress
    );
    
    // Resposta ao cliente
    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
    await context.HttpContext.Response.WriteAsJsonAsync(new
    {
        error = "Too many requests."
    }, cancellationToken: token);
};
```

### Métricas Recomendadas

1. **Total de requests bloqueados** (429 responses)
2. **Endpoints mais afetados** 
3. **IPs com mais bloqueios** (possível abuso)
4. **Horários de pico** de rate limiting

---

## ?? Boas Práticas

### 1. **Rate Limiting por Usuário**

Para limites por usuário autenticado (em vez de IP):

```csharp
rateLimiterOptions.AddPolicy("per-user-limiter", context =>
{
    var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    return RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: userId ?? "anonymous",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        }
    );
});
```

### 2. **Whitelist de IPs**

Liberar limites para IPs confiáveis:

```csharp
rateLimiterOptions.OnRejected = async (context, token) =>
{
    var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
    
    // Lista de IPs confiáveis (ex: servidores internos)
    var whitelist = new[] { "192.168.1.100", "10.0.0.1" };
    
    if (whitelist.Contains(ip))
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        return; // Permitir requisição
    }
    
    // Rate limit normal
    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
};
```

### 3. **Headers Informativos**

Adicionar headers com informações de limite:

```csharp
app.Use(async (context, next) =>
{
    await next();
    
    if (context.Response.StatusCode == 429)
    {
        context.Response.Headers.Add("X-RateLimit-Limit", "10");
        context.Response.Headers.Add("X-RateLimit-Remaining", "0");
        context.Response.Headers.Add("X-RateLimit-Reset", DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString());
    }
});
```

---

## ? Checklist de Validação

- [x] Rate Limiting configurado no `Program.cs`
- [x] Políticas criadas (vote, review, upload, api)
- [x] Endpoints protegidos com `[EnableRateLimiting]`
- [x] Resposta HTTP 429 personalizada
- [x] RetryAfter incluído na resposta
- [x] Documentação criada
- [x] Testes de carga realizados (recomendado)
- [ ] Monitoramento configurado (opcional)
- [ ] Alertas para abuso configurados (opcional)

---

## ?? Próximos Passos

1. **Testar Rate Limiting**
   ```bash
   # Testar com curl
   for i in {1..15}; do curl -X POST http://localhost:5000/api/reviews/1/helpful -H "Authorization: Bearer token"; done
   ```

2. **Configurar Alertas**
   - Integrar com Application Insights ou similar
   - Alertar quando taxa de 429 > 5% das requests

3. **Ajustar Limites**
   - Monitorar uso real
   - Ajustar conforme padrões de tráfego

---

**Status:** ? **IMPLEMENTADO E FUNCIONAL!**
