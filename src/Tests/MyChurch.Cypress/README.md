# MyChurch Cypress Tests

Testes end-to-end para a API MyChurch usando Cypress.

## ?? Quick Start

### 1?? Pré-requisitos

```bash
# Node.js 16+ instalado
node --version

# API MyChurch rodando
dotnet run --project ../../Web/MyChurch.Api.Web/MyChurch.Api.Web.csproj
```

### 2?? Instalação

```bash
cd Tests/MyChurch.Cypress
npm install
```

### 3?? Diagnóstico (RECOMENDADO)

Antes de executar os testes, verifique se o ambiente está configurado:

**Windows (PowerShell):**
```powershell
.\scripts\diagnose.ps1
```

**Linux/Mac:**
```bash
node scripts/diagnose.js
```

### 4?? Executar Testes

**Modo Interativo (Interface):**
```bash
npx cypress open
```

**Modo Headless (CI/CD):**
```bash
npx cypress run
```

**Executar teste específico:**
```bash
npx cypress run --spec "cypress/e2e/api/church-search.cy.js"
```

---

## ?? Troubleshooting

### ? Testes Travam / Não Executam

**Causas comuns:**

1. **API não está rodando**
   ```bash
   # Solução: Inicie a API
   cd ../../Web/MyChurch.Api.Web
   dotnet run
   ```

2. **Porta incorreta**
   - Verifique se a API está em `http://localhost:5210`
   - Confira `cypress.config.js` ? `baseUrl`

3. **Banco de dados não conectado**
   ```bash
   # Verifique PostgreSQL
   # Windows: services.msc ? procurar "postgresql"
   
   # Execute migrations
   cd ../../Infrastructure/MyChurch.Infrastructure
   dotnet ef database update
   ```

4. **Credenciais incorretas / Senhas quebradas** ??
   - **PROBLEMA COMUM**: A migração de segurança para BCrypt quebrou senhas antigas!
   - **SINTOMA**: Login falha com usuário `admin@test.com`
   - **CAUSA**: Senhas antigas usavam formato inseguro (XOR + Base64)
   - **SOLUÇÃO**: Execute o script de criação de usuários de teste:
   
   ```bash
   # Opção 1: SQL direto
   psql -U postgres -d mychurch_db -f ../../Infrastructure/MyChurch.Infrastructure/Scripts/CreateTestUsers.sql
   
   # Opção 2: Copie e cole no pgAdmin/DBeaver
   # Arquivo: Infrastructure/MyChurch.Infrastructure/Scripts/CreateTestUsers.sql
   ```
   
   ?? **Guia completo**: [PASSWORD_MIGRATION.md](./PASSWORD_MIGRATION.md)

5. **CORS bloqueando**
   - Verifique se CORS está habilitado em `Program.cs`

### ?? Guia Completo

Consulte: **[TROUBLESHOOTING.md](./TROUBLESHOOTING.md)**

---

## ?? Estrutura dos Testes

```
cypress/
??? e2e/
?   ??? api/
?       ??? church-search.cy.js     # Busca geoespacial
?       ??? church-photos.cy.js     # Galeria de fotos
?       ??? donation.cy.js          # Doações
?       ??? subscription.cy.js      # Assinaturas
?       ??? pastorbot.cy.js         # PastorBot
??? support/
?   ??? e2e.js                      # Comandos customizados
??? fixtures/
    ??? example.json                # Dados de teste
```

---

## ?? Testes Disponíveis

### ?? Church Search (Busca Geoespacial)
- ? Busca por proximidade
- ? Filtros (denominação, avaliação, características)
- ? Ordenação (distância, rating, popularidade)
- ? Paginação
- ? Mapa e bounds

**Executar:**
```bash
npx cypress run --spec "cypress/e2e/api/church-search.cy.js"
```

### ?? Church Photos (Galeria)
- ? Upload de fotos
- ? Moderação (aprovar/rejeitar)
- ? Sistema de likes
- ? Filtros por categoria

**Executar:**
```bash
npx cypress run --spec "cypress/e2e/api/church-photos.cy.js"
```

---

## ?? Configuração

### Variáveis de Ambiente

Edite `cypress.config.js`:

```javascript
env: {
  adminEmail: 'admin@test.com',
  adminPassword: 'Test@123456',
  memberEmail: 'member@test.com',
  memberPassword: 'Test@123456'
}
```

### Timeouts

Se os testes estão lentos:

```javascript
requestTimeout: 30000,      // 30 segundos
responseTimeout: 60000,     // 60 segundos
defaultCommandTimeout: 30000 // 30 segundos
```

---

## ?? Criar Novo Teste

```javascript
/// <reference types="cypress" />

describe('Meu Teste', () => {
  let authToken;

  before(() => {
    // Login antes dos testes
    cy.request({
      method: 'POST',
      url: `${Cypress.config('baseUrl')}/api/auth/login`,
      body: {
        identifier: Cypress.env('adminEmail'),
        password: Cypress.env('adminPassword')
      }
    }).then((response) => {
      authToken = response.body.token;
    });
  });

  it('Deve fazer algo', () => {
    cy.request({
      method: 'GET',
      url: `${Cypress.config('baseUrl')}/api/endpoint`,
      headers: {
        Authorization: `Bearer ${authToken}`
      }
    }).then((response) => {
      expect(response.status).to.eq(200);
    });
  });
});
```

---

## ?? Boas Práticas

1. **Sempre rode o diagnóstico antes** ? `.\scripts\diagnose.ps1`
2. **Use dados isolados** ? Não dependa de dados de outros testes
3. **Limpe o estado** ? Use `before()` e `after()`
4. **Assertions claras** ? Use mensagens descritivas
5. **Evite waits fixos** ? Use `cy.wait('@alias')` ou timeouts condicionais

---

## ?? Relatórios

Os relatórios são gerados em:
```
cypress/reports/
??? index.html          # Relatório visual
??? mochawesome.json    # Dados JSON
??? screenshots/        # Screenshots de falhas
```

Abrir relatório:
```bash
# Windows
start cypress/reports/index.html

# Linux/Mac
open cypress/reports/index.html
```

---

## ?? Scripts Úteis

```json
{
  "scripts": {
    "test": "cypress run",
    "test:open": "cypress open",
    "test:search": "cypress run --spec 'cypress/e2e/api/church-search.cy.js'",
    "test:photos": "cypress run --spec 'cypress/e2e/api/church-photos.cy.js'",
    "diagnose": "node scripts/diagnose.js"
  }
}
```

Executar:
```bash
npm run test
npm run diagnose
```

---

## ?? Precisa de Ajuda?

1. Execute o diagnóstico: `.\scripts\diagnose.ps1`
2. Consulte: `TROUBLESHOOTING.md`
3. Verifique logs da API
4. Abra uma issue no GitHub

---

## ?? Documentação

- [Cypress Docs](https://docs.cypress.io/)
- [MyChurch API Docs](http://localhost:5210/swagger)
- [Troubleshooting Guide](./TROUBLESHOOTING.md)
