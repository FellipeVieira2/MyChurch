# ?? GUIA RÁPIDO - RESET DO BANCO DE DADOS

## ?? ANTES DE COMEÇAR

### ? Pré-requisitos:
- PostgreSQL rodando
- .NET 8 SDK instalado
- EF Tools instalado: `dotnet tool install --global dotnet-ef`

---

## ?? OPÇÃO 1: RESET AUTOMÁTICO (RECOMENDADO)

### Execute o script de reset:

```powershell
cd C:\Users\Usuario\source\repos\FellipeVieira2\MyChurch\src\Infrastructure\MyChurch.Infrastructure\Scripts
.\ResetDatabase.ps1
```

**O que ele faz:**
1. ? Dropa o banco `mychurch_db`
2. ? Recria com todas as migrations
3. ? Cria usuários de teste automaticamente:
   - `admin@test.com` / `Test@123456`
   - `member@test.com` / `Test@123456`

---

## ?? OPÇÃO 2: RESET MANUAL

Se o script automático der erro de "dependência circular":

```powershell
# 1. Drop
dotnet ef database drop --force `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj

# 2. Update
dotnet ef database update `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
```

---

## ?? OPÇÃO 3: RESET ALTERNATIVO (SQL DIRETO)

Se nada funcionar, use SQL direto:

```powershell
.\ResetDatabase-Alternative.ps1
```

Esse script vai pedir:
- Host (padrão: localhost)
- Porta (padrão: 5432)
- Usuário (padrão: postgres)
- Senha
- Nome do banco (padrão: mychurch_db)

---

## ?? VALIDAR O RESET

### 1. Verificar Usuários Criados:

```sql
SELECT email, name, role 
FROM postgres.member 
WHERE email IN ('admin@test.com', 'member@test.com');
```

**Deve retornar:**
```
       email        |         name          | role
--------------------+-----------------------+------
 admin@test.com     | Admin Teste Cypress   |    0
 member@test.com    | Membro Teste Cypress  |    2
```

### 2. Testar API:

```powershell
# Terminal 1: Rodar API
cd C:\Users\Usuario\source\repos\FellipeVieira2\MyChurch\src
dotnet run --project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj

# Terminal 2: Testar Login
cd C:\Users\Usuario\source\repos\FellipeVieira2\MyChurch\src\Tests\MyChurch.Cypress
.\scripts\diagnose.ps1
```

**Resultado esperado:**
```
? API está rodando!
? Login funcionando!
? Usuário: Admin Teste Cypress
```

### 3. Executar Cypress:

```powershell
cd Tests\MyChurch.Cypress
npx cypress open
```

---

## ?? SE DER ERRO

### ? "Dependência Circular" (MSB4006)

**Causa:** EF não sabe qual é o startup project

**Solução:** Use `--startup-project` em TODOS os comandos

**Ou:** Execute `.\ResetDatabase.ps1` que já tem tudo configurado

---

### ? "Cannot drop database because it is in use"

**Solução:** Feche todas as conexões:

```sql
SELECT pg_terminate_backend(pid) 
FROM pg_stat_activity 
WHERE datname = 'mychurch_db' AND pid <> pg_backend_pid();
```

Depois tente novamente.

---

### ? "No database provider configured"

**Solução:** Verifique `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=mychurch_db;Username=postgres;Password=SUA_SENHA"
  }
}
```

---

### ? "relation does not exist"

**Causa:** Migrations não foram aplicadas

**Solução:**

```powershell
dotnet ef database update `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj
```

---

## ?? DOCUMENTAÇÃO COMPLETA

- **Troubleshooting:** [EF_TROUBLESHOOTING.md](./EF_TROUBLESHOOTING.md)
- **Migração de Senhas:** [PASSWORD_MIGRATION.md](../../Tests/MyChurch.Cypress/PASSWORD_MIGRATION.md)
- **Testes Cypress:** [README.md](../../Tests/MyChurch.Cypress/README.md)

---

## ? STATUS ATUAL

- ? Migration de campos de descoberta (`20250116000002_AddChurchDiscoveryFields`)
- ? Migration de warning de senhas antigas (`20250116000003_MigrateLegacyPasswordsWarning`)
- ? **Migration de seed de usuários (`20250116000004_SeedTestUsersForCypress`)** ? **NOVO!**
- ? Scripts de reset automatizados
- ? Documentação completa

---

**Última atualização:** 2025-01-16  
**Status:** ? Pronto para uso
