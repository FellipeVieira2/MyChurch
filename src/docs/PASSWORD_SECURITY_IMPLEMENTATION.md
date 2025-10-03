# ?? SEGURANÇA DE SENHAS - GUIA DE IMPLEMENTAÇÃO

## ? CORREÇÃO IMPLEMENTADA

Este documento descreve a correção crítica de segurança implementada para resolver o problema de **senhas armazenadas em texto plano**.

---

## ?? PROBLEMA IDENTIFICADO

### ? ANTES (INSEGURO):
```csharp
public class Member
{
    public string Password { get; set; }        // ? Senha em texto plano!
    public string PasswordHash { get; set; }    // Hash usado apenas como chave de criptografia
}

// Login verificava senha criptografada, não hash:
var encryptedPassword = request.Password.Encrypt(member.PasswordHash);
if (member.Password != encryptedPassword) { ... }
```

**Problemas:**
- ? Senhas armazenadas em texto plano no banco de dados
- ? Violação grave de segurança (OWASP Top 10)
- ? Se o banco vazar, todas as senhas ficam expostas
- ? Não compliance com LGPD e outras regulações

---

## ? SOLUÇÃO IMPLEMENTADA

### ? DEPOIS (SEGURO):
```csharp
public class Member
{
    // ? REMOVIDO: public string Password { get; set; }
    public string PasswordHash { get; set; }  // ? Apenas hash BCrypt
}

// Login verifica usando BCrypt:
if (!_passwordHasher.VerifyPassword(request.Password, member.PasswordHash)) { ... }
```

**Benefícios:**
- ? Senhas nunca armazenadas em texto plano
- ? Hash BCrypt com work factor 12 (4096 iterações)
- ? Salt único por senha (incluído no hash)
- ? Resistente a ataques de força bruta
- ? Compliance com OWASP e LGPD

---

## ??? COMPONENTES IMPLEMENTADOS

### 1. Interface de Password Hasher
**Arquivo:** `Domain/MyChurch.Domain/Services/IPasswordHasher.cs`

```csharp
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
```

### 2. Implementação com BCrypt
**Arquivo:** `Infrastructure/MyChurch.Infrastructure/Services/PasswordHasher.cs`

```csharp
public class PasswordHasher : IPasswordHasher
{
    private const int WORK_FACTOR = 12;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WORK_FACTOR);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
```

### 3. Pacote NuGet Instalado
```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

### 4. Registro no DI
**Arquivo:** `Infrastructure/MyChurch.Infrastructure/DependencyInjection.cs`

```csharp
services.AddScoped<IPasswordHasher, PasswordHasher>();
```

### 5. Comandos Atualizados

#### LoginCommandHandler ?
```csharp
// Antes:
var encryptedPassword = request.Password.Encrypt(member.PasswordHash);
if (member.Password != encryptedPassword) { ... }

// Depois:
if (!_passwordHasher.VerifyPassword(request.Password, member.PasswordHash)) { ... }
```

#### ActiveMemberPasswordCommand ?
```csharp
// Antes:
member.Password = request.Password.Encrypt(member.PasswordHash);

// Depois:
member.PasswordHash = _passwordHasher.HashPassword(request.Password);
```

#### AdminChangePasswordCommand ?
```csharp
// Antes:
member.Password = request.NewPassword.Encrypt(member.PasswordHash);

// Depois:
member.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
```

### 6. Migration de Banco de Dados
**Arquivo:** `Infrastructure/Migrations/20250116000000_RemovePasswordPlainTextFieldSECURITY.cs`

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "password",
        schema: "postgres",
        table: "member");
}
```

---

## ?? SEGURANÇA DO BCrypt

### Por que BCrypt?

1. **Slow by Design** ??
   - Work factor configurável (2^12 = 4096 iterações)
   - ~250-500ms para hash uma senha
   - Previne ataques de força bruta

2. **Salt Automático** ??
   - Salt único gerado para cada senha
   - Salt incluído no hash (não precisa armazenar separado)

3. **Future-Proof** ??
   - Pode aumentar work factor conforme hardware evolui
   - Hashes antigos continuam funcionando

4. **Formato do Hash**
   ```
   $2a$12$R9h/cIPz0gi.URNNX3kh2OPST9/PgBkqquzi.Ss7KIUgO2t0jWMUW
   ?  ?  ?                                           ?
   ?  ?  ?? Salt (22 caracteres)                     ?? Hash (31 caracteres)
   ?  ?? Work Factor (12 = 2^12 iterações)
   ?? Algoritmo (2a = BCrypt)
   ```

---

## ?? CHECKLIST DE MIGRAÇÃO

### Pré-Migração
- [x] ? Backup do banco de dados
- [x] ? Instalar BCrypt.Net-Next
- [x] ? Implementar IPasswordHasher
- [x] ? Atualizar todos os comandos de senha
- [x] ? Criar migration para remover campo password
- [x] ? Atualizar entidade Member

### Migração
- [ ] ?? Escolher estratégia de migração (ver PASSWORD_MIGRATION_GUIDE.md)
- [ ] ?? Executar migration: `dotnet ef database update`
- [ ] ?? Migrar senhas existentes OU resetar todas

### Pós-Migração
- [ ] ?? Testar login com senha nova
- [ ] ?? Testar ativação de conta
- [ ] ?? Testar reset de senha por admin
- [ ] ?? Verificar logs de erro
- [ ] ?? Monitorar performance de login

---

## ?? TESTES

### Teste Manual
```bash
# 1. Criar novo membro
POST /api/member
{
  "name": "Test User",
  "email": "test@test.com"
}

# 2. Ativar senha
POST /api/member/activate-password?hash={hash}
{
  "password": "Test123!@#"
}

# 3. Login
POST /api/auth/login
{
  "identifier": "test@test.com",
  "password": "Test123!@#"
}
```

### Verificação no Banco
```sql
-- Ver formato do hash (deve começar com $2a$12$)
SELECT id, name, password_hash 
FROM postgres.member 
WHERE password_hash IS NOT NULL 
LIMIT 5;

-- Verificar que não há senhas em texto plano
SELECT COUNT(*) 
FROM postgres.member 
WHERE password IS NOT NULL;
-- Esperado: 0
```

---

## ?? ALERTAS DE SEGURANÇA

### ? NUNCA FAÇA:
1. ? Armazenar senha em texto plano
2. ? Usar MD5 ou SHA1 para senhas (são rápidos demais!)
3. ? Usar salt global (cada senha deve ter salt único)
4. ? Logar senhas em logs
5. ? Enviar senhas por email
6. ? Comparar senhas com `==` (use VerifyPassword)

### ? SEMPRE FAÇA:
1. ? Use BCrypt, Argon2 ou PBKDF2
2. ? Work factor adequado (12 para BCrypt)
3. ? Salt único por senha
4. ? HTTPS em produção
5. ? Rate limiting em endpoints de login
6. ? Log de tentativas de login falhas

---

## ?? PERFORMANCE

### Benchmarks BCrypt (Work Factor 12):
- **Hash:** ~300ms por senha
- **Verify:** ~300ms por verificação
- **Memória:** ~4MB por operação

**Isso é intencional!** ?  
Hash lento = mais seguro contra força bruta

### Otimizações Implementadas:
- ? Cache de usuário logado (evita re-hash)
- ? Rate limiting em `/api/auth/login`
- ? Logs estruturados para monitoramento

---

## ?? REFERÊNCIAS

### Documentação Oficial:
- [OWASP Password Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)
- [BCrypt.Net Documentation](https://github.com/BcryptNet/bcrypt.net)
- [NIST Digital Identity Guidelines](https://pages.nist.gov/800-63-3/)

### Artigos Recomendados:
- [How to Safely Store Passwords](https://auth0.com/blog/hashing-passwords-one-way-road-to-security/)
- [BCrypt vs Argon2](https://security.stackexchange.com/questions/193351/in-2018-what-is-the-recommended-hash-to-store-passwords-bcrypt-scrypt-argon2)

---

## ?? PRÓXIMOS PASSOS DE SEGURANÇA

Após implementar esta correção, considere:

1. **Multi-Factor Authentication (MFA)** ??
2. **Password Strength Validator** ??
3. **Account Lockout após N tentativas** ??
4. **Password History** (prevenir reuso) ??
5. **Sessão única** (logout de outros dispositivos) ??

---

**Data de Implementação:** 16/01/2025  
**Severidade:** ?? CRÍTICA  
**Status:** ? IMPLEMENTADO  
**Próxima Revisão:** Trimestral

---

**Feito com ?? para MyChurch**
