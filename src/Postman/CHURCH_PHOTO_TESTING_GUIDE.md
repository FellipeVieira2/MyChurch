# ?? Guia de Testes - Church Photo Gallery

Guia completo passo a passo para testar a Galeria de Fotos das Igrejas.

## ?? Pré-requisitos

- ? Postman instalado
- ? Collection ChurchPhotoController importada
- ? Environment configurado (MyChurch.Development)
- ? `jwt_token` configurado
- ? `church_id` configurado
- ? API rodando

---

## ?? Fluxo Completo de Teste

### 1?? Upload de Fotos (Com Autenticação)

#### 1.1. Upload Foto - Exterior (Fachada)

**Request:** `Upload Photo - Exterior`  
**Expected:** `200 OK`

**Pré-teste:**
1. Configure `jwt_token` válido
2. Verifique se é membro da igreja `church_id`

**Validações:**
- ? Status 200
- ? Response contém `id` da foto
- ? Response contém `photoUrl` (S3)
- ? `category` retorna "Exterior"
- ? `isApproved` = true (fotos de membros são auto-aprovadas)
- ? `likes` = 0 (inicial)

**Salvar para uso posterior:**
```javascript
// Na aba Tests do Postman
pm.environment.set("photo_id", pm.response.json().id);
```

#### 1.2. Upload Múltiplas Categorias

Execute sequencialmente:
1. ? Upload Photo - Exterior
2. ? Upload Photo - Interior
3. ? Upload Photo - Worship
4. ? Upload Photo - Events
5. ? Upload Photo - Child Ministry
6. ? Upload Photo - Facilities

**Objetivo:** Criar galeria diversificada

**Validações:**
- ? Todas retornam 200
- ? Cada uma tem categoria diferente
- ? Todas são aprovadas automaticamente

---

### 2?? Listar e Filtrar Fotos (Público)

#### 2.1. Listar Todas as Fotos

**Request:** `Get All Photos - Church`  
**Expected:** `200 OK`

**Validações:**
- ? Status 200
- ? `totalPhotos` > 0 (se upload anterior funcionou)
- ? Array `photos` não vazio
- ? `categoriesCount` lista todas categorias usadas
- ? Paginação presente (`currentPage`, `totalPages`, etc)

**Verificar estrutura:**
```javascript
pm.test("Gallery structure is valid", function () {
    var data = pm.response.json();
    pm.expect(data).to.have.property('churchId');
    pm.expect(data).to.have.property('churchName');
    pm.expect(data).to.have.property('totalPhotos');
    pm.expect(data.photos).to.be.an('array');
    pm.expect(data.categoriesCount).to.be.an('array');
});
```

#### 2.2. Filtrar por Categoria - Exterior

**Request:** `Get Photos by Category - Exterior`  
**Expected:** `200 OK`

**Validações:**
- ? Apenas fotos com `category: "Exterior"`
- ? Filtro aplicado corretamente
- ? `totalPhotos` reflete apenas categoria filtrada

**Teste de filtro:**
```javascript
pm.test("All photos are Exterior category", function () {
    var data = pm.response.json();
    data.photos.forEach(function(photo) {
        pm.expect(photo.category).to.equal("Exterior");
    });
});
```

#### 2.3. Filtrar por Categoria - Worship

**Request:** `Get Photos by Category - Worship`  
**Expected:** `200 OK`

**Validações:**
- ? Apenas fotos de cultos
- ? `category: "Worship"`

#### 2.4. Apenas Fotos em Destaque

**Request:** `Get Featured Photos Only`  
**Expected:** `200 OK`

**Validações:**
- ? Todas têm `isFeatured: true`
- ? Ordenadas por `displayOrder`

**Nota:** Inicialmente pode retornar vazio se nenhuma foto foi marcada como destaque.

#### 2.5. Paginação

**Testes:**
1. Execute `Get All Photos` com `pageSize=10`
2. Execute `Get Photos - Page 2`

**Validações:**
- ? Página 1 tem primeiras 10 fotos
- ? Página 2 tem próximas 10
- ? `hasNextPage` correto
- ? `hasPreviousPage` correto

---

### 3?? Sistema de Likes (Com Autenticação)

#### 3.1. Curtir uma Foto

**Request:** `Like Photo`  
**Expected:** `204 No Content`

**Pré-requisito:**
1. Configure `photo_id` (use ID de foto do upload)

**Validações:**
- ? Status 204
- ? Nenhum body no response

**Validar efeito:**
1. Execute novamente `Get All Photos - Church`
2. Procure a foto com `id` = `photo_id`
3. Verifique que `likes` aumentou em 1
4. Verifique que `hasUserLiked: true`

#### 3.2. Descurtir uma Foto

**Request:** `Unlike Photo`  
**Expected:** `204 No Content`

**Validações:**
- ? Status 204

**Validar efeito:**
1. Execute `Get All Photos` novamente
2. Verifique que `likes` diminuiu em 1
3. Verifique que `hasUserLiked: false`

#### 3.3. Teste de Duplicação

**Cenário:** Tentar curtir a mesma foto 2x

1. Execute `Like Photo`
2. Execute `Like Photo` novamente
3. Execute `Get All Photos`

**Validação:**
- ? `likes` deve ser 1 (não 2)
- ? Sistema previne curtidas duplicadas

---

### 4?? Deletar Fotos (Com Autenticação)

#### 4.1. Deletar Foto Própria

**Request:** `Delete Photo`  
**Expected:** `204 No Content`

**Pré-requisito:**
- `photo_id` deve ser de foto que você enviou

**Validações:**
- ? Status 204
- ? Foto deletada

**Confirmar:**
1. Execute `Get All Photos - Church`
2. Procure pelo `id` da foto deletada
3. ? Não deve estar na lista

#### 4.2. Tentar Deletar Foto de Outro Usuário

**Cenário:** Usuário comum tenta deletar foto de outro

**Expected:** `403 Forbidden`

**Validações:**
- ? Status 403
- ? Mensagem: "Apenas administradores ou o autor podem deletar"

#### 4.3. Admin Pode Deletar Qualquer Foto

**Pré-requisito:**
- Token deve ser de usuário Admin

**Expected:** `204 No Content`

**Validações:**
- ? Admin pode deletar foto de qualquer membro
- ? Status 204

---

## ?? Testes de Validação

### Campos Obrigatórios

#### Teste 1: Upload sem `churchId`

```json
{
  "photoBase64": "data:image/jpeg;base64,...",
  "category": 1
}
```

**Expected:** `400 Bad Request`  
**Error:** "ID da igreja é obrigatório"

#### Teste 2: Upload sem `photoBase64`

```json
{
  "churchId": 1,
  "category": 1
}
```

**Expected:** `400 Bad Request`  
**Error:** "Foto é obrigatória"

#### Teste 3: Upload sem `category`

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,..."
}
```

**Expected:** `400 Bad Request`  
**Error:** "Categoria inválida"

### Validações de Formato

#### Teste 4: Base64 Inválido

```json
{
  "churchId": 1,
  "photoBase64": "invalid-base64-string",
  "category": 1
}
```

**Expected:** `400 Bad Request`  
**Error:** "Formato de foto inválido"

#### Teste 5: Categoria Inválida

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,...",
  "category": 99
}
```

**Expected:** `400 Bad Request`  
**Error:** "Categoria inválida"

#### Teste 6: Caption Muito Longa

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,...",
  "caption": "Lorem ipsum dolor sit amet... (> 500 caracteres)",
  "category": 1
}
```

**Expected:** `400 Bad Request`  
**Error:** "Legenda deve ter no máximo 500 caracteres"

---

## ?? Testes de Autenticação e Permissões

### Sem Token

#### Teste 7: Upload sem Autenticação

Remove header `Authorization`

**Expected:** `401 Unauthorized`

#### Teste 8: Like sem Autenticação

**Expected:** `401 Unauthorized`

#### Teste 9: Delete sem Autenticação

**Expected:** `401 Unauthorized`

### Token Inválido/Expirado

#### Teste 10: Upload com Token Inválido

Configure `jwt_token` = "invalid_token_xyz"

**Expected:** `401 Unauthorized`

### Sem Pertencer à Igreja

#### Teste 11: Upload para Igreja Diferente

1. Configure `church_id` = igreja que você NÃO é membro
2. Tente upload

**Expected:** `403 Forbidden`  
**Error:** "Você não pertence a esta igreja"

---

## ?? Testes de Performance

### Teste 12: Upload de Foto Grande

**Tamanho:** 4-5MB

**Validações:**
- ? Upload completa em < 5s
- ? S3 URL retornada corretamente

### Teste 13: Listar Muitas Fotos

**Cenário:** Igreja com 100+ fotos

**Validações:**
- ? Response em < 500ms
- ? Paginação funciona corretamente
- ? Não retorna todas de uma vez

### Teste 14: Filtros com Muitos Dados

**Cenário:** Filtrar categoria com 50+ fotos

**Validações:**
- ? Filtro aplicado corretamente
- ? Performance aceitável (< 500ms)

---

## ?? Cenários Avançados

### Cenário 1: Galeria Completa

**Objetivo:** Criar galeria representativa

1. Upload 2-3 fotos de cada categoria
2. Like algumas fotos
3. Liste todas com categoriesCount
4. Valide que todas categorias aparecem

**Validações:**
- ? Mínimo de 10 fotos total
- ? Pelo menos 5 categorias diferentes
- ? categoriesCount correto

### Cenário 2: Popular e Limpar

**Fluxo:**
1. Upload 10 fotos
2. Like 5 delas
3. Delete 3 fotos
4. Liste novamente

**Validações:**
- ? Total correto após deletes
- ? Likes preservados
- ? Fotos deletadas não aparecem

### Cenário 3: Múltiplos Usuários

**Setup:**
1. User A: Upload 5 fotos
2. User B: Like fotos de User A
3. User B: Upload 5 fotos
4. User A: Like fotos de User B

**Validações:**
- ? Likes de ambos contados
- ? `hasUserLiked` correto por usuário
- ? Cada um só pode deletar suas próprias

---

## ?? Checklist de Testes

### Upload
- [ ] Upload com todas categorias (1-10)
- [ ] Upload com caption longa (máx 500)
- [ ] Upload sem caption (opcional)
- [ ] Upload retorna URL S3 válida
- [ ] Foto é aprovada automaticamente
- [ ] Erro sem token
- [ ] Erro com token inválido
- [ ] Erro sem pertencer à igreja

### Listar
- [ ] Listar todas as fotos
- [ ] Filtrar por cada categoria
- [ ] Filtrar por featured
- [ ] Paginação (páginas 1, 2, 3)
- [ ] PageSize diferente (10, 20, 50)
- [ ] categoriesCount correto

### Likes
- [ ] Curtir foto
- [ ] Descurtir foto
- [ ] Curtir 2x (prevenir duplicata)
- [ ] hasUserLiked correto
- [ ] Contador de likes incrementa/decrementa

### Delete
- [ ] Deletar foto própria
- [ ] Admin deletar qualquer foto
- [ ] User comum não pode deletar foto alheia
- [ ] Erro ao deletar foto inexistente

### Validações
- [ ] Campos obrigatórios validados
- [ ] Base64 inválido rejeitado
- [ ] Categoria inválida rejeitada
- [ ] Caption > 500 rejeitada

---

## ?? Métricas de Sucesso

### Cobertura
- ? 100% dos endpoints testados
- ? Todos casos de erro validados
- ? Todas categorias testadas
- ? Permissões validadas

### Performance
- ? Upload < 5s (fotos grandes)
- ? List < 500ms
- ? Like/Unlike < 200ms
- ? Delete < 500ms

---

## ?? Problemas Comuns

### Upload Falha

**Sintoma:** 400 Bad Request

**Verificar:**
- [ ] Base64 está correto (com prefixo `data:image/...`)
- [ ] Categoria é número 1-10
- [ ] Token JWT válido
- [ ] É membro da igreja

### Foto Não Aparece

**Sintoma:** Lista vazia após upload

**Verificar:**
- [ ] Upload retornou 200?
- [ ] `isApproved` = true?
- [ ] Filtro de categoria correto?
- [ ] `church_id` correto?

### Like Não Funciona

**Sintoma:** 204 mas contador não muda

**Verificar:**
- [ ] `photo_id` correto?
- [ ] Token válido?
- [ ] Refresh da lista após like

### Delete Bloqueado

**Sintoma:** 403 Forbidden

**Verificar:**
- [ ] É autor da foto OU admin?
- [ ] `photo_id` correto?
- [ ] `churchId` query parameter correto?

---

**Happy Testing! ????**
