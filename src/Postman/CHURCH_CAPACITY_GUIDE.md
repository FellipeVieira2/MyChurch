# ??? Capacidade e Infraestrutura - Guia Completo

Guia completo para gerenciar **capacidade e infraestrutura** da igreja, incluindo lotação, estacionamento, equipamentos e instalações.

---

## ?? Overview

Este módulo permite que igrejas cadastrem informações detalhadas sobre:

### ?? Capacidade
- **Lotação de Assentos** - Capacidade máxima sentada
- **Capacidade em Pé** - Para eventos especiais (shows, conferências)
- **Vagas de Estacionamento** - Número de vagas disponíveis

### ?? Conectividade
- **WiFi** - Disponibilidade e senha
- **Transmissão Online** - Equipamento para lives

### ?? Instalações
- **Cafeteria/Lanchonete** - Espaço para alimentação
- **Livraria** - Venda de livros e materiais
- **Berçário** - Cuidado de crianças
- **Batistério** - Para batismos

### ??? Equipamentos
- **Sistema de Som** - Qualidade e marca
- **Projetor/Telão** - Para apresentações
- **Ar-Condicionado** - Climatização

### ?? Instalações Adicionais
- Academia, quadra esportiva, biblioteca, etc

---

## ?? ENDPOINT ÚNICO

### PUT /api/Church/capacity

**Autenticação:** Bearer Token (Admin only)

**Request Body:**
```json
{
  "seatingCapacity": 800,
  "standingCapacity": 200,
  "parkingSpaces": 150,
  "hasWifi": true,
  "wifiPassword": "IgrejaWiFi2024",
  "hasCafeteria": true,
  "hasBookstore": true,
  "hasNursery": true,
  "hasSoundSystem": true,
  "hasProjector": true,
  "hasAirConditioning": true,
  "hasBaptistery": true,
  "additionalFacilities": ["Academia", "Quadra esportiva"],
  "equipmentNotes": "Sistema de som Yamaha. Projetor 4K."
}
```

**Response:**
```json
{
  "message": "Capacidade e infraestrutura atualizadas com sucesso"
}
```

---

## ?? CASOS DE USO

### 1. Igreja Grande (Mega Church)

```json
{
  "seatingCapacity": 3000,
  "standingCapacity": 500,
  "parkingSpaces": 800,
  "hasWifi": true,
  "wifiPassword": "MegaChurchWiFi",
  "hasCafeteria": true,
  "hasBookstore": true,
  "hasNursery": true,
  "hasSoundSystem": true,
  "hasProjector": true,
  "hasAirConditioning": true,
  "hasBaptistery": true,
  "additionalFacilities": [
    "Auditório secundário (500 lugares)",
    "Food court",
    "Livraria Saraiva",
    "Academia completa",
    "Quadra poliesportiva",
    "Piscina",
    "15 salas de aula",
    "Estúdio de TV"
  ],
  "equipmentNotes": "Sistema de som DiGiCo SD7. Telões LED 4K (4 unidades). 20 câmeras 4K. Sistema de iluminação robotizado."
}
```

**Caso de uso:** Mega igreja com infraestrutura completa para eventos de grande porte

---

### 2. Igreja Média

```json
{
  "seatingCapacity": 300,
  "standingCapacity": 50,
  "parkingSpaces": 40,
  "hasWifi": true,
  "wifiPassword": "Igreja2024",
  "hasCafeteria": false,
  "hasBookstore": false,
  "hasNursery": true,
  "hasSoundSystem": true,
  "hasProjector": true,
  "hasAirConditioning": true,
  "hasBaptistery": true,
  "equipmentNotes": "Mesa de som Behringer X32. Projetor Epson 3K lumens."
}
```

**Caso de uso:** Igreja de médio porte com infraestrutura padrão

---

### 3. Igreja Pequena

```json
{
  "seatingCapacity": 100,
  "parkingSpaces": 15,
  "hasWifi": false,
  "hasSoundSystem": true,
  "hasProjector": false,
  "hasAirConditioning": false,
  "equipmentNotes": "Caixas amplificadas. 2 microfones sem fio."
}
```

**Caso de uso:** Igreja iniciante com equipamento básico

---

### 4. Foco em Famílias

```json
{
  "hasNursery": true,
  "hasCafeteria": true,
  "additionalFacilities": [
    "Brinquedoteca",
    "Fraldário",
    "Sala de amamentação",
    "Área externa com playground"
  ]
}
```

**Caso de uso:** Igreja que atende famílias com crianças pequenas

---

### 5. Foco Educacional

```json
{
  "hasBookstore": true,
  "additionalFacilities": [
    "Biblioteca teológica (5.000 volumes)",
    "5 salas de estudo bíblico",
    "Sala de informática (20 computadores)",
    "Auditório para seminários (100 lugares)"
  ],
  "equipmentNotes": "Equipamento de videoconferência para EAD."
}
```

**Caso de uso:** Igreja com seminário teológico

---

### 6. Igreja com Eventos

```json
{
  "seatingCapacity": 1000,
  "standingCapacity": 300,
  "parkingSpaces": 250,
  "hasSoundSystem": true,
  "hasProjector": true,
  "hasAirConditioning": true,
  "additionalFacilities": [
    "Palco profissional (15x10m)",
    "Camarins (4)",
    "Sala de imprensa",
    "Área de food trucks"
  ],
  "equipmentNotes": "Sistema de som line array. Iluminação DMX. Estrutura para shows."
}
```

**Caso de uso:** Igreja que realiza eventos e shows gospel

---

## ?? CAMPOS DISPONÍVEIS

### ?? Capacidade

| Campo | Tipo | Descrição | Validação |
|-------|------|-----------|-----------|
| `seatingCapacity` | int? | Lotação de assentos | ? 0 |
| `standingCapacity` | int? | Capacidade em pé | ? 0 |
| `parkingSpaces` | int? | Vagas de estacionamento | ? 0 |

---

### ?? Conectividade

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `hasWifi` | bool | Possui WiFi |
| `wifiPassword` | string? | Senha do WiFi (opcional) |

---

### ?? Instalações

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `hasCafeteria` | bool | Cafeteria/Lanchonete |
| `hasBookstore` | bool | Livraria |
| `hasNursery` | bool | Berçário |
| `hasBaptistery` | bool | Batistério |

---

### ??? Equipamentos

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `hasSoundSystem` | bool | Sistema de som |
| `hasProjector` | bool | Projetor/Telão |
| `hasAirConditioning` | bool | Ar-condicionado |

---

### ?? Extras

| Campo | Tipo | Descrição | Exemplo |
|-------|------|-----------|---------|
| `additionalFacilities` | string[]? | Instalações extras | `["Academia", "Quadra"]` |
| `equipmentNotes` | string? | Notas sobre equipamentos | `"Sistema Yamaha"` |

---

## ? VALIDAÇÕES

### Capacidades Positivas
```json
// ? Erro 400
{
  "seatingCapacity": -100
}

// Mensagem
{
  "errors": {
    "SeatingCapacity": ["Capacidade de assentos deve ser positiva"]
  }
}
```

### Permissão Admin
```json
// ? Erro 403
{
  "errors": {
    "Permission": ["Apenas administradores podem atualizar capacidade e infraestrutura"]
  }
}
```

---

## ?? VISUALIZAR DADOS

Os dados aparecem automaticamente no `GET /api/Church/{id}`:

```json
{
  "id": 1,
  "name": "Igreja Batista Central",
  
  // Capacidade
  "seatingCapacity": 800,
  "standingCapacity": 200,
  "parkingSpaces": 150,
  
  // Conectividade
  "hasWifi": true,
  "wifiPassword": "IgrejaWiFi2024",  // Visível apenas para membros
  
  // Instalações
  "hasCafeteria": true,
  "hasBookstore": true,
  "hasNursery": true,
  "hasBaptistery": true,
  
  // Equipamentos
  "hasSoundSystem": true,
  "hasProjector": true,
  "hasAirConditioning": true,
  
  // Extras
  "additionalFacilities": ["Academia", "Quadra esportiva"],
  "equipmentNotes": "Sistema de som Yamaha. Projetor 4K."
}
```

---

## ?? CENÁRIOS DE ATUALIZAÇÃO

### Cenário 1: Setup Inicial
1. Igreja recém-criada
2. Admin acessa e preenche dados básicos
3. Cadastra lotação e estacionamento

```json
{
  "seatingCapacity": 200,
  "parkingSpaces": 30
}
```

---

### Cenário 2: Expansão
1. Igreja construiu novo anexo
2. Aumenta capacidade
3. Adiciona instalações

```json
{
  "seatingCapacity": 500,  // Era 200
  "parkingSpaces": 100,     // Era 30
  "additionalFacilities": ["Auditório anexo (100 lugares)"]
}
```

---

### Cenário 3: Melhoria de Equipamentos
1. Igreja adquiriu novos equipamentos
2. Atualiza lista de equipamentos

```json
{
  "hasSoundSystem": true,
  "hasProjector": true,
  "hasAirConditioning": true,
  "equipmentNotes": "Upgrade: Mesa digital Yamaha M7CL. Projetores 4K (2 unidades). Ar split 48.000 BTUs."
}
```

---

### Cenário 4: WiFi para Membros
1. Igreja instalou WiFi
2. Disponibiliza senha para membros

```json
{
  "hasWifi": true,
  "wifiPassword": "MinhaIgreja#2024"
}
```

---

### Cenário 5: Igreja Familiar
1. Foco em famílias com crianças
2. Adiciona berçário e instalações infantis

```json
{
  "hasNursery": true,
  "additionalFacilities": [
    "Brinquedoteca",
    "Fraldário",
    "Sala de amamentação",
    "Playground externo"
  ]
}
```

---

## ?? BENEFÍCIOS

### Para Planejamento de Eventos ??
- ? Sabe quantas pessoas cabem
- ? Planeja estacionamento (manobristas, vagas extras)
- ? Identifica limitações (ar-condicionado, som)
- ? Aluga equipamentos se necessário

### Para Visitantes ??
- ? Sabe se tem WiFi
- ? Verifica se tem berçário (famílias)
- ? Confirma se tem cafeteria
- ? Vê se tem estacionamento

### Para Administração ??
- ? Controla lotação (segurança)
- ? Planeja expansões
- ? Gerencia patrimônio
- ? Faz inventário de equipamentos

---

## ?? EXEMPLOS PRÁTICOS

### Igreja Moderna
```json
{
  "seatingCapacity": 600,
  "parkingSpaces": 120,
  "hasWifi": true,
  "hasLiveStream": true,
  "hasSoundSystem": true,
  "hasProjector": true,
  "hasAirConditioning": true,
  "additionalFacilities": [
    "Estúdio de transmissão",
    "Sala de streaming",
    "Equipamento de produção audiovisual"
  ],
  "equipmentNotes": "3 câmeras PTZ 4K. Mesa de streaming ATEM Mini Pro. Sistema de som digital."
}
```

---

### Igreja Histórica
```json
{
  "seatingCapacity": 400,
  "hasBaptistery": true,
  "hasProjector": false,
  "hasAirConditioning": false,
  "additionalFacilities": [
    "Órgão de tubos (século XIX)",
    "Vitral artístico",
    "Torre com sino"
  ],
  "equipmentNotes": "Preservação do patrimônio histórico. Som discreto."
}
```

---

### Igreja em Casa
```json
{
  "seatingCapacity": 30,
  "parkingSpaces": 5,
  "hasWifi": true,
  "wifiPassword": "CasaDePaz2024",
  "equipmentNotes": "Caixa amplificada. Microfone sem fio. Projetor doméstico."
}
```

---

## ?? ATUALIZAÇÃO INCREMENTAL

O endpoint permite atualizar **apenas os campos enviados**:

```json
// Só atualiza WiFi
{
  "hasWifi": true,
  "wifiPassword": "NovaSenha2024"
}

// Outros campos permanecem inalterados
```

---

## ?? INTEGRAÇÃO COM BUSCA

Esses dados podem ser usados nos filtros de busca:

### Busca por Capacidade
```
GET /api/Church/public/search?minCapacity=500
```

### Busca com WiFi
```
GET /api/Church/public/search?amenities=wifi
```

### Busca com Estacionamento
```
GET /api/Church/public/search?amenities=estacionamento&minParkingSpaces=50
```

---

## ?? PRIVACIDADE

### Senha WiFi
- ? Visível apenas para membros autenticados
- ? Não aparece na busca pública
- ?? Admin pode optar por não cadastrar senha (WiFi público)

### Equipamentos
- ? Informações públicas (ajuda visitantes)
- ? Útil para planejamento de eventos

---

## ?? CHECKLIST DE TESTES

- [ ] Atualizar igreja grande (todos os campos)
- [ ] Atualizar igreja média (campos básicos)
- [ ] Atualizar igreja pequena (mínimo)
- [ ] Configurar WiFi com senha
- [ ] Configurar WiFi público (sem senha)
- [ ] Habilitar todas instalações
- [ ] Adicionar instalações customizadas
- [ ] Registrar equipamentos profissionais
- [ ] Testar capacidade negativa (erro 400)
- [ ] Testar sem autenticação (erro 401)
- [ ] Testar com usuário não-admin (erro 403)
- [ ] Verificar no GET Church by ID

---

## ?? DICAS DE USO

### 1. Seja Realista
- Não exagere nas capacidades
- Considere normas de segurança
- Leve em conta conforto (não só máximo)

### 2. Detalhe Equipamentos
```json
{
  "equipmentNotes": "Detalhes completos aqui:\n- Som: Yamaha M7CL, caixas QSC\n- Vídeo: Projetores Epson 6K (2x)\n- Iluminação: Moving heads (8x)"
}
```

### 3. Atualize Regularmente
- Novos equipamentos? Atualize!
- Ampliou? Atualize capacidade
- Removeu instalação? Atualize

### 4. Use para Marketing
- Destaque diferenciais no site
- Mostre infraestrutura em redes sociais
- Atraia eventos externos

---

## ?? PRÓXIMAS FEATURES

### Planejado
- [ ] Upload de fotos das instalações
- [ ] Mapa interativo das instalações
- [ ] Reserva de salas/equipamentos
- [ ] Calendário de manutenção
- [ ] Inventário detalhado de equipamentos
- [ ] Controle de acesso (fechaduras inteligentes)

---

**??? Pronto para cadastrar sua infraestrutura!**

Execute as requests no Postman e registre todos os recursos da sua igreja.

**Data**: 05/10/2024  
**Versão**: 1.0.0  
**Status**: ? Production Ready  
**Endpoints**: 1 (PUT /capacity)  
**Collection**: 15 requests organizadas por categoria
