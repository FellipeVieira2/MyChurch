# ?? Church Social Media & Contacts - Postman Guide

Guia completo para testar os endpoints de **Redes Sociais e Contatos** das igrejas.

## ?? Overview

Este módulo permite que igrejas centralizem todos os seus canais de comunicação:
- ?? Website oficial
- ?? Email institucional
- ?? Instagram
- ?? Facebook
- ?? YouTube
- ?? WhatsApp
- ?? Twitter/X
- ?? TikTok

## ?? Autenticação

**Endpoint:** `PUT /api/Church/social-media`  
**Requer:** JWT Token com role **Admin**

Apenas administradores da igreja podem atualizar as redes sociais.

## ?? Endpoint Único

```http
PUT /api/Church/social-media
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

### ? Atualização Completa

```json
{
  "website": "https://www.minhaigreja.com.br",
  "email": "contato@minhaigreja.com.br",
  "instagramUrl": "https://www.instagram.com/minhaigreja",
  "facebookUrl": "https://www.facebook.com/minhaigreja",
  "youtubeUrl": "https://www.youtube.com/@minhaigreja",
  "whatsAppNumber": "+5511987654321",
  "twitterUrl": "https://twitter.com/minhaigreja",
  "tiktokUrl": "https://www.tiktok.com/@minhaigreja"
}
```

**Response:**
```json
{
  "message": "Redes sociais atualizadas com sucesso"
}
```

## ?? Exemplos de Uso

### 1. Website + Email (Básico)

```json
{
  "website": "https://www.igrejanovaesperanca.com.br",
  "email": "admin@igrejanovaesperanca.com.br"
}
```

**Caso de uso:** Igreja começando presença digital

---

### 2. Apenas Instagram

```json
{
  "instagramUrl": "https://www.instagram.com/igreja.renovada"
}
```

**Caso de uso:** Igreja focada em público jovem

---

### 3. Facebook + YouTube (Transmissões)

```json
{
  "facebookUrl": "https://www.facebook.com/IgrejaPentecostal",
  "youtubeUrl": "https://www.youtube.com/@IgrejaPentecostal"
}
```

**Caso de uso:** Igreja com cultos online

---

### 4. WhatsApp Comercial

```json
{
  "whatsAppNumber": "+5511999888777"
}
```

**Caso de uso:** Atendimento direto aos membros

---

### 5. Redes Jovens (TikTok + Twitter)

```json
{
  "tiktokUrl": "https://www.tiktok.com/@igrejajovem",
  "twitterUrl": "https://twitter.com/IgrejaJovem"
}
```

**Caso de uso:** Ministério jovem ativo em redes

---

## ?? Validações

### URLs Válidas
- ? Devem começar com `http://` ou `https://`
- ? Formato completo: `https://www.instagram.com/usuario`
- ? Inválido: `instagram.com/usuario` (sem protocolo)

### Email Válido
- ? Formato: `usuario@dominio.com`
- ? Inválido: `usuario@` ou `@dominio.com`

### WhatsApp
- ? Formato internacional: `+5511987654321`
- ?? Opcional: pode ser com ou sem `+`

## ?? Visualizar Redes Sociais

As redes sociais aparecem automaticamente no `GET /api/Church/{id}`:

```json
{
  "id": 1,
  "name": "Igreja Nova Esperança",
  ...
  "website": "https://www.igrejanovaesperanca.com.br",
  "email": "contato@igrejanovaesperanca.com.br",
  "instagramUrl": "https://www.instagram.com/igrejanovaesperanca",
  "facebookUrl": "https://www.facebook.com/igrejanovaesperanca",
  "youtubeUrl": "https://www.youtube.com/@igrejanovaesperanca",
  "whatsAppNumber": "+5511987654321",
  "twitterUrl": null,
  "tiktokUrl": null
}
```

## ?? Tratamento de Erros

### 400 Bad Request - URL Inválida

```json
{
  "errors": {
    "InstagramUrl": ["URL inválida"]
  }
}
```

**Solução:** Verifique o formato da URL (deve incluir `https://`)

---

### 400 Bad Request - Email Inválido

```json
{
  "errors": {
    "Email": ["Email inválido"]
  }
}
```

**Solução:** Use formato `usuario@dominio.com`

---

### 401 Unauthorized

```json
{
  "message": "Token de autenticação inválido ou expirado"
}
```

**Solução:** Obtenha um novo JWT token

---

### 403 Forbidden

```json
{
  "errors": {
    "Permission": ["Apenas administradores podem atualizar as redes sociais"]
  }
}
```

**Solução:** Certifique-se de estar logado como Admin

---

## ?? Cenários de Teste

### Cenário 1: Setup Inicial
1. Execute "Update Social Media - Complete"
2. Verifique com `GET /api/Church/{id}`
3. ? Todas as redes devem aparecer

### Cenário 2: Atualização Parcial
1. Execute "Update Social Media - Only Instagram"
2. Verifique com `GET /api/Church/{id}`
3. ? Apenas Instagram atualizado, outros mantidos

### Cenário 3: Remover Redes (enviar null)
```json
{
  "tiktokUrl": null,
  "twitterUrl": null
}
```
4. ? TikTok e Twitter removidos

### Cenário 4: Validação de URLs
1. Tente enviar URL inválida: `"website": "minhaigreja.com"`
2. ? Deve retornar erro 400

## ?? Formatos de URL Recomendados

### Instagram
```
https://www.instagram.com/usuario
https://instagram.com/usuario
```

### Facebook
```
https://www.facebook.com/usuario
https://www.facebook.com/pages/Nome-Pagina/12345
```

### YouTube
```
https://www.youtube.com/@usuario
https://www.youtube.com/channel/UCxxxxxx
```

### WhatsApp
```
+5511987654321
5511987654321
```

### Twitter/X
```
https://twitter.com/usuario
https://x.com/usuario
```

### TikTok
```
https://www.tiktok.com/@usuario
```

## ?? Atualização Incremental

O endpoint permite atualizar **apenas os campos enviados**:

```json
// Atualiza só Instagram
{
  "instagramUrl": "https://www.instagram.com/nova_conta"
}

// Outros campos permanecem inalterados
```

## ?? Response do GET Church

```json
{
  "id": 1,
  "name": "Igreja Batista Central",
  "phone": "11987654321",
  
  // Redes Sociais
  "website": "https://www.batista.com.br",
  "email": "contato@batista.com.br",
  "instagramUrl": "https://www.instagram.com/ibc.oficial",
  "facebookUrl": "https://www.facebook.com/IBCentral",
  "youtubeUrl": "https://www.youtube.com/@IBCentral",
  "whatsAppNumber": "+5511998877665",
  "twitterUrl": "https://twitter.com/IBCentral",
  "tiktokUrl": "https://www.tiktok.com/@ibc.jovem",
  
  // Outros dados...
  "address": {...},
  "schedules": [...]
}
```

## ?? Scripts de Teste (Postman)

### Pre-request Script
```javascript
// Validate JWT token exists
if (!pm.environment.get("jwt_token")) {
    console.error("JWT Token não configurado!");
}
```

### Test Script
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has success message", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('message');
    pm.expect(jsonData.message).to.include('sucesso');
});

pm.test("Response time is acceptable", function () {
    pm.expect(pm.response.responseTime).to.be.below(2000);
});
```

## ?? Checklist de Testes

- [ ] Atualizar todos os campos (completo)
- [ ] Atualizar apenas website + email
- [ ] Atualizar apenas Instagram
- [ ] Atualizar Facebook + YouTube (transmissões)
- [ ] Atualizar WhatsApp
- [ ] Atualizar TikTok + Twitter (público jovem)
- [ ] Testar URL inválida (erro 400)
- [ ] Testar email inválido (erro 400)
- [ ] Testar sem autenticação (erro 401)
- [ ] Testar com usuário não-admin (erro 403)
- [ ] Visualizar no GET /api/Church/{id}

## ?? Dicas de Uso

### 1. URLs Completas
Sempre use URLs completas com protocolo:
```
? https://www.instagram.com/usuario
? instagram.com/usuario
```

### 2. WhatsApp Internacional
Use formato internacional para compatibilidade:
```
? +5511987654321
? 5511987654321
? 11987654321
```

### 3. Atualização Gradual
Atualize conforme a igreja cria presença digital:
1. Primeiro: Website + Email
2. Depois: Instagram + Facebook
3. Por último: YouTube + TikTok

### 4. Remoção de Redes
Para remover uma rede social, envie `null`:
```json
{
  "tiktokUrl": null
}
```

---

**?? Pronto para testar!**

Execute as requests no Postman na ordem sugerida e valide cada resposta.

**Data**: 05/10/2024  
**Versão**: 1.0.0  
**Status**: ? Production Ready
