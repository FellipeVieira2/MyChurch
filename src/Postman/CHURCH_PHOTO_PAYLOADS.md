# ?? Church Photo Gallery - Payload Examples

Exemplos completos de payloads para facilitar o upload e testes de fotos.

## ?? Upload Photo Payloads

### 1. Exterior (Fachada)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/...",
  "caption": "Fachada principal da igreja - Vista frontal com estacionamento",
  "category": 1,
  "originalFileName": "fachada_principal.jpg"
}
```

**Dicas para Exterior:**
- Capture em dia ensolarado
- Mostre entrada principal
- Inclua placas/letreiros
- Evite horários com contraluz

---

### 2. Interior (Templo)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "caption": "Interior do templo - Altar principal e palco de louvor",
  "category": 2,
  "originalFileName": "interior_altar.jpg"
}
```

**Dicas para Interior:**
- Iluminação adequada
- Mostre capacidade do templo
- Capture detalhes arquitetônicos
- Fotografe sem pessoas (ou com permissão)

---

### 3. Worship (Culto)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "caption": "Culto de domingo - Louvor e adoração com a banda de música",
  "category": 3,
  "originalFileName": "culto_domingo.jpg"
}
```

**Dicas para Worship:**
- Capture momentos de louvor
- Mostre engajamento da congregação
- Respeite privacidade (evite rostos muito próximos)
- Prefira fotos amplas do ambiente

---

### 4. Events (Eventos Especiais)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "caption": "Batismo coletivo - 15 novos membros batizados",
  "category": 4,
  "originalFileName": "batismo_coletivo_2024.jpg"
}
```

**Eventos sugeridos:**
- Batismos
- Casamentos
- Conferências
- Retiros espirituais
- Aniversários da igreja
- Programações especiais

---

### 5. Community (Comunidade)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "caption": "Confraternização após o culto - Membros e visitantes",
  "category": 5,
  "originalFileName": "comunidade_cafe.jpg"
}
```

**Momentos de Comunidade:**
- Café da manhã/almoços
- Grupos pequenos
- Trabalhos voluntários
- Ações sociais
- Momentos de oração

---

### 6. Facilities (Instalações)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "caption": "Estacionamento amplo - Capacidade para 50 veículos com vagas acessíveis",
  "category": 6,
  "originalFileName": "estacionamento.jpg"
}
```

**Instalações importantes:**
- Estacionamento
- Banheiros (incluindo acessíveis)
- Área de café/cozinha
- Salas de som/multimídia
- Áreas externas
- Rampas de acesso

---

### 7. Child Ministry (Ministério Infantil)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "caption": "Escola Bíblica Dominical - Sala infantil com atividades lúdicas",
  "category": 7,
  "originalFileName": "ministerio_infantil.jpg"
}
```

**?? IMPORTANTE:**
- **Sempre obtenha autorização dos pais**
- Evite fotos identificáveis de crianças
- Prefira fotos de atividades (sem rostos próximos)
- Foque no ambiente e recursos

**Dicas:**
- Mostre brinquedos educativos
- Destaque segurança do ambiente
- Fotografe materiais didáticos
- Capture momentos de atividades (sem identificar crianças)

---

### 8. Youth Ministry (Ministério Jovem)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "caption": "Reunião de jovens - Grupo de louvor e estudo bíblico",
  "category": 8,
  "originalFileName": "jovens_reuniao.jpg"
}
```

**Atividades de Jovens:**
- Reuniões semanais
- Acampamentos
- Grupos de música
- Eventos evangelísticos
- Retiros

---

### 9. Menu (Programação)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "caption": "Programação semanal - Cultos, grupos e atividades",
  "category": 9,
  "originalFileName": "programacao_semanal.jpg"
}
```

**Conteúdo de Menu:**
- Cartaz de horários
- Calendário mensal
- Programação especial
- Avisos importantes
- QR Code de eventos

---

### 10. Other (Outros)

```json
{
  "churchId": 1,
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/...",
  "caption": "Biblioteca da igreja - Acervo de livros cristãos disponível",
  "category": 10,
  "originalFileName": "biblioteca.jpg"
}
```

---

## ?? Como Gerar Base64 de Imagens

### Método 1: Online (Rápido)

1. Acesse: https://www.base64-image.de/
2. Faça upload da imagem
3. Copie o código gerado
4. Cole no payload

### Método 2: JavaScript (Browser)

```javascript
// HTML
<input type="file" id="imageInput" accept="image/*">

// JavaScript
document.getElementById('imageInput').addEventListener('change', function(e) {
    const file = e.target.files[0];
    const reader = new FileReader();
    
    reader.onload = function(event) {
        const base64 = event.target.result;
        console.log(base64);
        // Copie e cole no Postman
    };
    
    reader.readAsDataURL(file);
});
```

### Método 3: Python

```python
import base64

def image_to_base64(image_path):
    with open(image_path, "rb") as image_file:
        encoded = base64.b64encode(image_file.read()).decode('utf-8')
        return f"data:image/jpeg;base64,{encoded}"

# Uso
base64_string = image_to_base64("fachada.jpg")
print(base64_string)
```

### Método 4: C# (.NET)

```csharp
using System;
using System.IO;

public string ImageToBase64(string imagePath)
{
    byte[] imageBytes = File.ReadAllBytes(imagePath);
    string base64String = Convert.ToBase64String(imageBytes);
    return $"data:image/jpeg;base64,{base64String}";
}

// Uso
string base64 = ImageToBase64("fachada.jpg");
```

---

## ?? Tamanhos Recomendados

| Tipo | Largura | Altura | Tamanho |
|------|---------|--------|---------|
| Exterior | 1920px | 1080px | < 2MB |
| Interior | 1920px | 1080px | < 2MB |
| Worship | 1920px | 1080px | < 2MB |
| Events | 1920px | 1080px | < 2MB |
| Facilities | 1280px | 720px | < 1MB |
| Thumbnail | 640px | 360px | < 500KB |

**Formato:** JPEG (qualidade 80-85%)

---

## ?? Query Parameters Examples

### Listar Todas as Fotos

```
GET /api/ChurchPhoto/church/1
    ?page=1
    &pageSize=20
```

### Filtrar por Categoria

```
# Exterior (Fachada)
GET /api/ChurchPhoto/church/1?category=1

# Interior
GET /api/ChurchPhoto/church/1?category=2

# Cultos
GET /api/ChurchPhoto/church/1?category=3

# Eventos
GET /api/ChurchPhoto/church/1?category=4

# Instalações
GET /api/ChurchPhoto/church/1?category=6
```

### Apenas Fotos em Destaque

```
GET /api/ChurchPhoto/church/1
    ?onlyFeatured=true
    &page=1
    &pageSize=5
```

### Combinação de Filtros

```
GET /api/ChurchPhoto/church/1
    ?category=3
    &onlyFeatured=true
    &page=1
    &pageSize=10
```

---

## ?? Response Examples

### Upload Success

```json
{
  "id": 42,
  "churchId": 1,
  "photoUrl": "https://s3.amazonaws.com/mychurch/churches/1/photos/abc123.jpg",
  "caption": "Fachada principal da igreja",
  "category": "Exterior",
  "categoryName": "Fachada Externa",
  "uploadedAt": "2024-10-05T14:30:00Z",
  "likes": 0,
  "isApproved": true,
  "isRejected": false,
  "isFeatured": false,
  "displayOrder": 0,
  "uploadedByName": "João Silva",
  "uploadedByMemberId": 10
}
```

### List Photos Response

```json
{
  "churchId": 1,
  "churchName": "Igreja Nova Esperança",
  "totalPhotos": 45,
  "photos": [
    {
      "id": 42,
      "churchId": 1,
      "photoUrl": "https://s3.../abc123.jpg",
      "caption": "Fachada principal",
      "category": "Exterior",
      "categoryDisplay": "Fachada",
      "uploadedAt": "2024-10-05T14:30:00Z",
      "likes": 15,
      "isApproved": true,
      "isFeatured": true,
      "uploadedByMemberName": "João Silva",
      "hasUserLiked": false
    },
    {
      "id": 43,
      "photoUrl": "https://s3.../def456.jpg",
      "caption": "Interior do templo",
      "category": "Interior",
      "categoryDisplay": "Interior",
      "uploadedAt": "2024-10-05T15:00:00Z",
      "likes": 8,
      "isApproved": true,
      "isFeatured": false,
      "hasUserLiked": true
    }
  ],
  "categoriesCount": [
    {
      "category": "Exterior",
      "categoryDisplay": "Fachada",
      "count": 12
    },
    {
      "category": "Interior",
      "categoryDisplay": "Interior",
      "count": 8
    },
    {
      "category": "Worship",
      "categoryDisplay": "Culto",
      "count": 15
    },
    {
      "category": "Events",
      "categoryDisplay": "Eventos",
      "count": 10
    }
  ],
  "currentPage": 1,
  "pageSize": 20,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

---

## ? Checklist de Qualidade da Foto

### Antes de Fazer Upload

- [ ] Imagem bem iluminada
- [ ] Foco nítido
- [ ] Resolução adequada (mín. 1280x720)
- [ ] Tamanho < 5MB
- [ ] Formato JPEG ou PNG
- [ ] Sem elementos ofensivos
- [ ] Sem informações sensíveis visíveis
- [ ] Autorização de pessoas visíveis (se aplicável)

### Caption (Legenda)

- [ ] Descritiva e clara
- [ ] Máximo 500 caracteres
- [ ] Sem erros ortográficos
- [ ] Contextualiza a foto

### Categoria Correta

- [ ] Categoria escolhida faz sentido
- [ ] Não duplicar fotos similares
- [ ] Considerar valor informativo

---

## ?? Dicas Profissionais

### Fotografia de Igrejas

**Iluminação:**
- ? Luz natural quando possível
- ? Evitar flash direto
- ? Horários: 9-11h ou 14-16h

**Composição:**
- ? Regra dos terços
- ? Linhas guia naturais
- ? Pontos focais claros

**Ângulos:**
- ? Altura dos olhos para fotos gerais
- ? Ângulo baixo para destacar arquitetura
- ? Ângulo alto para mostrar capacidade

### Privacidade e Ética

**?? Crianças:**
- SEMPRE obtenha autorização dos pais
- Evite identificação individual
- Prefira fotos de grupo (sem close-ups)
- Foque em atividades, não em rostos

**Pessoas em Geral:**
- Respeite privacidade
- Evite fotos constrangedoras
- Obtenha consentimento para close-ups
- Blur de rostos se necessário

**Informações Sensíveis:**
- Não fotografe dados pessoais
- Evite números de telefone visíveis
- Esconda placas de carros
- Proteja informações financeiras

---

**Happy Uploading! ??**
