# ?? TROUBLESHOOTING - ENTITY FRAMEWORK MIGRATIONS

## ? ERRO: "Dependência Circular" (MSB4006)

### Causa:
O Entity Framework não consegue determinar qual é o projeto de startup.

### ? Solução 1: Adicionar --startup-project

Sempre especifique o startup project nos comandos:

```powershell
# ? ERRADO
dotnet ef database update --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj

# ? CORRETO
dotnet ef database update `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
```

### ? Solução 2: Usar Scripts Prontos

Execute o script que já tem tudo configurado:

```powershell
cd Infrastructure\MyChurch.Infrastructure\Scripts
.\ResetDatabase.ps1
```

### ? Solução 3: Reset Manual (SEMPRE FUNCIONA)

Se os scripts do EF não funcionarem, use SQL direto:

```powershell
.\ResetDatabase-Alternative.ps1
```

---

## ?? COMANDOS CORRETOS PARA ENTITY FRAMEWORK

### 1. Listar Migrations

```powershell
dotnet ef migrations list `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
```

### 2. Adicionar Nova Migration

```powershell
dotnet ef migrations add NomeDaMigration `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
```

### 3. Aplicar Migrations

```powershell
dotnet ef database update `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
```

### 4. Reverter Migration

```powershell
dotnet ef database update NomeDaMigrationAnterior `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
```

### 5. Drop Database

```powershell
dotnet ef database drop --force `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
```

---

## ??? CONFIGURAR ALIAS NO POWERSHELL (OPCIONAL)

Para não precisar digitar comandos longos, crie aliases:

### Edite seu perfil do PowerShell:

```powershell
notepad $PROFILE
```

### Adicione os aliases:

```powershell
# Aliases para Entity Framework MyChurch
function ef-list {
    dotnet ef migrations list `
        --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
        --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
}

function ef-update {
    dotnet ef database update `
        --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
        --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
}

function ef-drop {
    dotnet ef database drop --force `
        --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
        --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
}

function ef-reset {
    ef-drop
    ef-update
}

function ef-add {
    param([string]$name)
    dotnet ef migrations add $name `
        --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
        --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
}
```

### Depois é só usar:

```powershell
ef-list        # Listar migrations
ef-update      # Aplicar migrations
ef-drop        # Dropar banco
ef-reset       # Drop + Update
ef-add MinhaNovaFeature  # Adicionar migration
```

---

## ?? DIAGNÓSTICO DE PROBLEMAS

### 1. Verificar Versão do EF Tools

```powershell
dotnet ef --version
```

**Deve ser:** 8.x.x (compatível com .NET 8)

**Se não estiver instalado:**

```powershell
dotnet tool install --global dotnet-ef
```

**Se estiver desatualizado:**

```powershell
dotnet tool update --global dotnet-ef
```

### 2. Verificar Connection String

**Arquivo:** `Web\MyChurch.Api.Web\appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=mychurch_db;Username=postgres;Password=SUA_SENHA"
  }
}
```

### 3. Verificar se PostgreSQL está Rodando

```powershell
# Windows
Get-Service postgresql*

# Ou teste conexão
psql -U postgres -c "SELECT version();"
```

### 4. Verificar Migrations no Banco

```sql
SELECT * FROM postgres.__efmigrationshistory ORDER BY migration_id;
```

---

## ?? ERROS COMUNS

### Erro: "Cannot drop database because it is in use"

**Solução:**

```sql
-- Feche todas as conexões
SELECT pg_terminate_backend(pid) 
FROM pg_stat_activity 
WHERE datname = 'mychurch_db' AND pid <> pg_backend_pid();

-- Depois drope
DROP DATABASE mychurch_db;
```

### Erro: "relation does not exist"

**Causa:** Migration não foi aplicada

**Solução:**

```powershell
ef-update
```

### Erro: "column already exists"

**Causa:** Migration duplicada ou aplicada parcialmente

**Solução:**

```powershell
# Opção 1: Remover migration problemática
Remove-Item Infrastructure\MyChurch.Infrastructure\Migrations\20XXX_Nome.cs

# Opção 2: Reset completo
.\Scripts\ResetDatabase.ps1
```

---

## ? CHECKLIST DE VALIDAÇÃO

Depois de aplicar migrations, verifique:

- [ ] API inicia sem erros
- [ ] Swagger abre (http://localhost:5210/swagger)
- [ ] Usuários de teste existem (`admin@test.com` / `member@test.com`)
- [ ] Login funciona (use `diagnose.ps1`)
- [ ] Testes Cypress passam

---

## ?? REFERÊNCIAS

- [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
- [Migrations Overview](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Troubleshooting](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/managing)
