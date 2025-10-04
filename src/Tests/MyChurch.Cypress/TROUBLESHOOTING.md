# ?? Diagnóstico de Problemas com Cypress

## Problema: Cypress trava ao executar testes

### Checklist de Diagnóstico

#### 1. API não está acessível
```bash
# Testar se a API está respondendo
curl http://localhost:5210/swagger/index.html
# ou
curl http://localhost:5210/api/health
```

**Se não responder:**
- ? Verificar se a API está rodando
- ? Verificar porta correta (5210)
- ? Verificar firewall/antivírus

---

#### 2. Banco de dados não conectado
```bash
# Verificar se PostgreSQL está rodando
# Windows:
services.msc
# Procurar por "postgresql"

# Testar conexão
psql -U postgres -d mychurch_db
```

**Se não conectar:**
- ? Iniciar PostgreSQL
- ? Verificar connection string em appsettings.json
- ? Criar banco se necessário

---

#### 3. Migrations não executadas
```bash
cd Infrastructure/MyChurch.Infrastructure
dotnet ef database update
```

**Erros comuns:**
- ? Tabelas não existem
- ? Colunas faltando
- ? Constraints violadas

---

#### 4. Seeds não executados
```bash
# Verificar se há dados para testar
SELECT COUNT(*) FROM postgres.church;
SELECT COUNT(*) FROM postgres.member;
```

**Se estiver vazio:**
- ? Executar seeds manualmente
- ? Criar dados de teste

---

#### 5. Credenciais incorretas no Cypress
**Arquivo:** `Tests/MyChurch.Cypress/cypress.config.js`

```javascript
env: {
  adminEmail: 'admin@test.com',      // ? Pode não existir
  adminPassword: 'Test@123456',      // ? Pode estar errado
}
```

**Solução:**
- Criar usuário de teste no banco
- Ou usar credenciais existentes

---

#### 6. CORS não configurado
```csharp
// Program.cs
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
```

---

#### 7. Timeout do Cypress muito curto
```javascript
// cypress.config.js
requestTimeout: 10000,      // Aumentar para 30000
responseTimeout: 30000,     // Aumentar para 60000
defaultCommandTimeout: 10000 // Aumentar para 30000
```

---

## ??? SCRIPT DE DIAGNÓSTICO AUTOMÁTICO

Crie este arquivo e execute antes dos testes:

**Arquivo:** `Tests/MyChurch.Cypress/scripts/diagnose.js`

\`\`\`javascript
const http = require('http');

console.log('?? Iniciando diagnóstico...\n');

// 1. Testar conexão com API
function testAPI() {
  return new Promise((resolve) => {
    const options = {
      hostname: 'localhost',
      port: 5210,
      path: '/swagger/index.html',
      method: 'GET',
      timeout: 5000
    };

    const req = http.request(options, (res) => {
      if (res.statusCode === 200) {
        console.log('? API está respondendo (porta 5210)');
        resolve(true);
      } else {
        console.log(\`? API retornou status: \${res.statusCode}\`);
        resolve(false);
      }
    });

    req.on('timeout', () => {
      console.log('? Timeout ao conectar na API');
      console.log('   Certifique-se de que a API está rodando em http://localhost:5210');
      resolve(false);
    });

    req.on('error', (error) => {
      console.log('? Erro ao conectar na API:', error.message);
      console.log('   A API está rodando?');
      resolve(false);
    });

    req.end();
  });
}

// 2. Testar endpoint de login
async function testLogin() {
  return new Promise((resolve) => {
    const postData = JSON.stringify({
      identifier: 'admin@test.com',
      password: 'Test@123456'
    });

    const options = {
      hostname: 'localhost',
      port: 5210,
      path: '/api/auth/login',
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Content-Length': Buffer.byteLength(postData)
      },
      timeout: 5000
    };

    const req = http.request(options, (res) => {
      let data = '';
      
      res.on('data', (chunk) => {
        data += chunk;
      });

      res.on('end', () => {
        if (res.statusCode === 200) {
          console.log('? Login funcionando');
          resolve(true);
        } else {
          console.log(\`? Login falhou (status: \${res.statusCode})\`);
          console.log('   Resposta:', data);
          console.log('   Verifique as credenciais no cypress.config.js');
          resolve(false);
        }
      });
    });

    req.on('timeout', () => {
      console.log('? Timeout no endpoint de login');
      resolve(false);
    });

    req.on('error', (error) => {
      console.log('? Erro ao testar login:', error.message);
      resolve(false);
    });

    req.write(postData);
    req.end();
  });
}

// Executar diagnóstico
async function runDiagnosis() {
  const apiOk = await testAPI();
  
  if (apiOk) {
    console.log('');
    await testLogin();
  }

  console.log('\n?? Diagnóstico concluído.');
  console.log('\nPróximos passos:');
  console.log('1. Se a API não está respondendo: dotnet run no projeto MyChurch.Api.Web');
  console.log('2. Se login falhou: Criar usuário de teste no banco');
  console.log('3. Se tudo OK: npx cypress open');
}

runDiagnosis();
\`\`\`

---

## ?? SOLUÇÃO RÁPIDA

### Opção 1: Script de Teste Simples

**Arquivo:** `Tests/MyChurch.Cypress/scripts/quick-test.sh`

\`\`\`bash
#!/bin/bash

echo "?? Testando API..."

# Testar se API está acessível
response=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5210/swagger/index.html)

if [ $response -eq 200 ]; then
    echo "? API está rodando!"
    echo "?? Executando testes Cypress..."
    cd Tests/MyChurch.Cypress
    npx cypress run
else
    echo "? API não está acessível"
    echo "Execute: dotnet run --project Web/MyChurch.Api.Web/MyChurch.Api.Web.csproj"
fi
\`\`\`

### Opção 2: PowerShell (Windows)

**Arquivo:** `Tests/MyChurch.Cypress/scripts/quick-test.ps1`

\`\`\`powershell
Write-Host "?? Testando API..." -ForegroundColor Cyan

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5210/swagger/index.html" -UseBasicParsing -TimeoutSec 5
    
    if ($response.StatusCode -eq 200) {
        Write-Host "? API está rodando!" -ForegroundColor Green
        Write-Host "?? Executando testes Cypress..." -ForegroundColor Cyan
        
        Set-Location "Tests\MyChurch.Cypress"
        npx cypress run
    }
} catch {
    Write-Host "? API não está acessível" -ForegroundColor Red
    Write-Host "Execute: dotnet run --project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj" -ForegroundColor Yellow
    exit 1
}
\`\`\`

---

## ?? CRIAR DADOS DE TESTE

**SQL para criar usuário admin:**

\`\`\`sql
-- 1. Criar igreja de teste
INSERT INTO postgres.church (name, phone, description, created)
VALUES ('Igreja Teste', '11999999999', 'Igreja para testes', NOW())
RETURNING id;

-- 2. Criar membro admin (usar ID da igreja acima)
INSERT INTO postgres.member (
    name, email, phone, birth_date, 
    church_id, role, password_hash, created
)
VALUES (
    'Admin Teste',
    'admin@test.com',
    '11999999999',
    '1990-01-01',
    1, -- ID da igreja
    0,  -- Role.Admin
    '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyPgMqL.kP7W', -- BCrypt de "Test@123456"
    NOW()
);
\`\`\`

---

## ?? ATUALIZAR CYPRESS CONFIG

\`\`\`javascript
// Tests/MyChurch.Cypress/cypress.config.js
const { defineConfig } = require('cypress');

module.exports = defineConfig({
  e2e: {
    baseUrl: 'http://localhost:5210',
    supportFile: 'cypress/support/e2e.js',
    specPattern: 'cypress/e2e/**/*.cy.{js,jsx,ts,tsx}',
    video: false, // Desabilitar para debug
    screenshotOnRunFailure: true,
    
    // ? AUMENTAR TIMEOUTS
    requestTimeout: 30000,      // 30 segundos
    responseTimeout: 60000,     // 60 segundos
    defaultCommandTimeout: 30000, // 30 segundos
    
    env: {
      adminEmail: 'admin@test.com',
      adminPassword: 'Test@123456'
    },

    setupNodeEvents(on, config) {
      // Log de debug
      on('task', {
        log(message) {
          console.log(message);
          return null;
        }
      });
      
      return config;
    },

    retries: {
      runMode: 2,
      openMode: 0
    }
  }
});
\`\`\`

---

## ?? MODO DEBUG

Execute com logs detalhados:

\`\`\`bash
# Ver logs da API
dotnet run --project Web/MyChurch.Api.Web/MyChurch.Api.Web.csproj --verbosity detailed

# Ver logs do Cypress
DEBUG=cypress:* npx cypress run

# Ou abrir interface
npx cypress open
\`\`\`

---

## ? CHECKLIST FINAL

Antes de executar os testes:

- [ ] PostgreSQL rodando
- [ ] Banco de dados criado
- [ ] Migrations executadas
- [ ] API rodando em http://localhost:5210
- [ ] Swagger acessível em http://localhost:5210/swagger
- [ ] Usuário admin existe no banco
- [ ] Credenciais corretas no cypress.config.js
- [ ] CORS configurado
- [ ] Timeouts aumentados

---

## ?? COMANDO FINAL

\`\`\`bash
# 1. Rodar API
cd Web/MyChurch.Api.Web
dotnet run

# 2. Em outro terminal, rodar Cypress
cd Tests/MyChurch.Cypress
npx cypress run --spec "cypress/e2e/api/church-search.cy.js"
\`\`\`
