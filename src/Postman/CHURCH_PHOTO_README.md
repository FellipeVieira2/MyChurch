# ?? MyChurch API - Church Photo Gallery Collection

Collection completa do Postman para testar todos os endpoints da **Galeria de Fotos das Igrejas**.

## ?? Conteúdo

- ? **ChurchPhotoController.postman_collection.json** - Collection principal
- ? Compartilha environment com ChurchController

## ?? Como Importar

### 1. Importar a Collection

1. Abra o Postman
2. Clique em **Import** (canto superior esquerdo)
3. Selecione o arquivo `ChurchPhotoController.postman_collection.json`
4. Clique em **Import**

### 2. Usar Environment Existente

Esta collection usa o mesmo environment do ChurchController:
- **MyChurch.Development.postman_environment.json**

Certifique-se de que está ativo!

## ?? Configuração Inicial

### Variáveis Necessárias

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `base_url` | URL da API | `https://localhost:7163` |
| `jwt_token` | Token JWT | `eyJhbGciOi...` |
| `church_id` | ID da igreja | `1` |
| `photo_id` | ID da foto (para like/delete) | `1` |

### Nova Variável: photo_id

Esta collection adiciona a variável `photo_id` para operações de like/delete.

**Como obter:**
1. Execute "Get All Photos - Church"
2. Copie o `id` de uma foto do response
3. Cole em: Environment > `photo_id`

## ?? Estrutura da Collection

### ?? Upload Photos (6 requests)
Upload de fotos em diferentes categorias:

- **Upload Photo - Exterior** - Fachada (categoria 1)
- **Upload Photo - Interior** - Interior do templo (categoria 2)
- **Upload Photo - Worship** - Durante culto (categoria 3)
- **Upload Photo - Events** - Eventos especiais (categoria 4)
- **Upload Photo - Child Ministry** - Ministério infantil (categoria 7)
- **Upload Photo - Facilities** - Instalações (categoria 6)

### ?? List & Filter Photos (6 requests)
Listar e filtrar fotos (público):

- **Get All Photos - Church** - Todas as fotos aprovadas
- **Get Photos by Category - Exterior** - Filtro: Fachada
- **Get Photos by Category - Worship** - Filtro: Cultos
- **Get Featured Photos Only** - Apenas fotos em destaque
- **Get Photos - Large Page Size** - Paginação grande (50)
- **Get Photos - Page 2** - Segunda página

### ?? Like System (2 requests)
Sistema de curtidas:

- **Like Photo** - Curtir foto
- **Unlike Photo** - Remover curtida

### ??? Delete Photos (1 request)
Deletar fotos:

- **Delete Photo** - Deletar (admin ou autor)

**Total: 15 endpoints**

## ?? Categorias de Fotos

### PhotoCategory Enum

| Valor | Categoria | Descrição |
|-------|-----------|-----------|
| 1 | Exterior | Fachada da igreja |
| 2 | Interior | Interior do templo |
| 3 | Worship | Durante o culto |
| 4 | Events | Eventos especiais |
| 5 | Community | Comunidade/pessoas |
| 6 | Facilities | Instalações (estacionamento, banheiro) |
| 7 | ChildMinistry | Ministério infantil |
| 8 | YouthMinistry | Ministério jovem |
| 9 | Menu | Programação de atividades |
| 10 | Other | Outros |

## ?? Autenticação

### Endpoints Públicos ? (sem auth)
- `GET /api/ChurchPhoto/church/{id}` - Listar fotos

### Endpoints Protegidos ?? (requer JWT)
- `POST /api/ChurchPhoto` - Upload foto
- `POST /api/ChurchPhoto/{id}/like` - Curtir/descurtir
- `DELETE /api/ChurchPhoto/{id}` - Deletar foto

### Permissões Especiais ??
- **Delete**: Apenas admin ou autor da foto

## ?? Exemplos de Uso

### 1. Upload de Foto - Exterior

```http
POST /api/ChurchPhoto
Authorization: Bearer {token}
Content-Type: application/json

{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQ...",
  "caption": "Fachada principal da igreja",
  "category": 1,
  "originalFileName": "fachada.jpg"
}
```

**Response:**
```json
{
  "id": 42,
  "churchId": 1,
  "photoUrl": "https://s3.../churches/1/photos/abc123.jpg",
  "caption": "Fachada principal da igreja",
  "category": "Exterior",
  "categoryName": "Fachada Externa",
  "uploadedAt": "2024-10-05T12:00:00Z",
  "likes": 0,
  "isApproved": true,
  "isFeatured": false
}
```

### 2. Listar Fotos por Categoria

```http
GET /api/ChurchPhoto/church/1?category=3&page=1&pageSize=10
```

**Response:**
```json
{
  "churchId": 1,
  "churchName": "Igreja Nova Esperança",
  "totalPhotos": 15,
  "photos": [
    {
      "id": 5,
      "photoUrl": "https://...",
      "caption": "Culto de domingo",
      "category": "Worship",
      "likes": 25,
      "uploadedByName": "João Silva"
    }
  ],
  "categoriesCount": [
    {
      "category": "Worship",
      "categoryDisplay": "Culto",
      "count": 15
    }
  ],
  "currentPage": 1,
  "pageSize": 10,
  "totalPages": 2,
  "hasNextPage": true
}
```

### 3. Curtir uma Foto

```http
POST /api/ChurchPhoto/5/like?like=true
Authorization: Bearer {token}
```

**Response:** `204 No Content`

### 4. Deletar Foto

```http
DELETE /api/ChurchPhoto/5?churchId=1
Authorization: Bearer {token}
```

**Response:** `204 No Content`

## ?? Casos de Uso Comuns

### Caso 1: Upload de Múltiplas Fotos

```javascript
// Execute sequencialmente:
1. Upload Photo - Exterior
2. Upload Photo - Interior
3. Upload Photo - Worship
4. Upload Photo - Events

// Salve os IDs retornados para uso posterior
```

### Caso 2: Galeria Completa com Filtros

```javascript
// 1. Buscar todas as fotos
GET /church/1?page=1&pageSize=50

// 2. Filtrar por categoria específica
GET /church/1?category=3&page=1

// 3. Apenas fotos em destaque
GET /church/1?onlyFeatured=true
```

### Caso 3: Interação Social

```javascript
// 1. Listar fotos
GET /church/1

// 2. Curtir foto favorita
POST /5/like?like=true

// 3. Descurtir
POST /5/like?like=false
```

## ?? Payload Examples

### Minimal Upload (apenas obrigatórios)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQ...",
  "category": 1
}
```

### Full Upload (com todos os campos)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "caption": "Vista completa da fachada principal - Entrada acessível para cadeirantes",
  "category": 1,
  "originalFileName": "fachada_principal_2024.jpg"
}
```

### Base64 Image Tips

1. **Tamanho máximo**: ~5MB (recomendado)
2. **Formatos**: JPEG, PNG, GIF
3. **Prefixo**: `data:image/{tipo};base64,`
4. **Exemplo completo**:
   ```
   data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/...
   ```

## ?? Scripts de Automação

### Pré-request Script (Global)

```javascript
// Auto-inject timestamp
pm.environment.set('timestamp', new Date().toISOString());
```

### Test Script - Upload Photo

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Photo uploaded successfully", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('id');
    pm.expect(jsonData).to.have.property('photoUrl');
    pm.expect(jsonData.photoUrl).to.include('s3');
});

// Salva photo_id para uso posterior
pm.test("Save photo ID", function () {
    var jsonData = pm.response.json();
    pm.environment.set("photo_id", jsonData.id);
    console.log("Photo ID saved: " + jsonData.id);
});
```

### Test Script - List Photos

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has gallery structure", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('photos');
    pm.expect(jsonData).to.have.property('totalPhotos');
    pm.expect(jsonData).to.have.property('categoriesCount');
});

pm.test("Photos array is valid", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.photos).to.be.an('array');
});

pm.test("Each photo has required fields", function () {
    var jsonData = pm.response.json();
    if (jsonData.photos.length > 0) {
        var photo = jsonData.photos[0];
        pm.expect(photo).to.have.property('id');
        pm.expect(photo).to.have.property('photoUrl');
        pm.expect(photo).to.have.property('category');
        pm.expect(photo).to.have.property('likes');
    }
});
```

### Test Script - Like Photo

```javascript
pm.test("Status code is 204", function () {
    pm.response.to.have.status(204);
});

pm.test("Like operation successful", function () {
    pm.expect(pm.response.code).to.equal(204);
    console.log("Photo liked successfully");
});
```

### Test Script - Delete Photo

```javascript
pm.test("Status code is 204", function () {
    pm.response.to.have.status(204);
});

pm.test("Photo deleted successfully", function () {
    pm.expect(pm.response.code).to.equal(204);
    console.log("Photo deleted");
});
```

## ?? Filtros Disponíveis

### Por Categoria

```
GET /church/1?category=1  # Exterior
GET /church/1?category=2  # Interior
GET /church/1?category=3  # Worship
GET /church/1?category=4  # Events
GET /church/1?category=6  # Facilities
GET /church/1?category=7  # ChildMinistry
```

### Por Status

```
GET /church/1?onlyFeatured=true   # Apenas destaque
GET /church/1?onlyFeatured=false  # Todas (padrão)
```

### Paginação

```
GET /church/1?page=1&pageSize=10   # Primeira página, 10 items
GET /church/1?page=2&pageSize=20   # Segunda página, 20 items
GET /church/1?page=1&pageSize=50   # Galeria grande
```

### Combinações

```
GET /church/1?category=3&onlyFeatured=true&page=1&pageSize=5
# Fotos de culto, apenas destaque, primeira página, 5 items
```

## ? Tratamento de Erros

### 400 Bad Request

```json
{
  "errors": {
    "PhotoBase64": ["Foto é obrigatória"],
    "Category": ["Categoria inválida"]
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
  "message": "Apenas administradores ou o autor podem deletar"
}
```

### 404 Not Found

```json
{
  "message": "Foto não encontrada"
}
```

## ?? Troubleshooting

### Erro: "Foto é muito grande"
- **Solução**: Reduza o tamanho da imagem
- **Máximo**: ~5MB
- **Dica**: Use compressão JPEG quality 80-85%

### Erro: "Base64 inválido"
- **Solução**: Certifique-se do formato correto
- **Formato**: `data:image/jpeg;base64,{base64_string}`
- **Validar**: Use um validador online

### Erro: "Você não pertence a esta igreja"
- **Solução**: Verifique se o `church_id` está correto
- **Verificar**: Seu usuário é membro desta igreja?

### Erro: "Foto não encontrada" ao deletar
- **Solução**: Verifique se o `photo_id` existe
- **Dica**: Liste as fotos primeiro para obter IDs válidos

## ?? Métricas de Resposta

### Upload
- ? Tempo esperado: < 2s (depende do tamanho)
- ?? Payload: ~1-5MB

### Listar Fotos
- ? Tempo esperado: < 500ms
- ?? Paginação: 10-50 items

### Like/Unlike
- ? Tempo esperado: < 200ms
- ?? Response: 204 No Content

### Delete
- ? Tempo esperado: < 500ms
- ?? Response: 204 No Content

## ?? Workflow Completo

### 1. Setup Inicial
```
1. Importe a collection
2. Configure jwt_token no environment
3. Configure church_id
```

### 2. Upload de Fotos
```
1. Execute "Upload Photo - Exterior"
2. Execute "Upload Photo - Interior"
3. Execute "Upload Photo - Worship"
4. Salve os photo_ids retornados
```

### 3. Visualizar Galeria
```
1. Execute "Get All Photos - Church"
2. Verifique todas as fotos enviadas
3. Confira categoriesCount
```

### 4. Interagir
```
1. Execute "Like Photo" (use photo_id salvo)
2. Execute "Get All Photos" novamente
3. Confirme que likes aumentou
```

### 5. Gerenciar
```
1. Execute "Delete Photo" se necessário
2. Execute "Get All Photos" para confirmar
```

## ?? Suporte

Dúvidas ou problemas?

1. ?? Consulte esta documentação
2. ?? Veja PAYLOADS.md para exemplos
3. ?? Execute testes passo a passo
4. ?? Verifique troubleshooting

---

**Desenvolvido para MyChurch API - Galeria de Fotos** ??

**Data**: 05/10/2024  
**Versão**: 1.0.0  
**Status**: ? Production Ready
