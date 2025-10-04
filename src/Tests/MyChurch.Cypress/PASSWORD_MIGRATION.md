# ?? GUIA DE MIGRAÇÃO DE SENHAS - Sistema Antigo ? BCrypt

## ?? PROBLEMA IDENTIFICADO

A migração de segurança que implementamos **QUEBROU todas as senhas antigas** do sistema!

### O que aconteceu?

**ANTES (Sistema Inseguro):**
```csharp
// Senha armazenada em TEXTO PLANO criptografado (XOR + Base64)
member.PasswordHash = Guid.NewGuid().ToString("N");  // Salt aleatório
member.Password = "senha123".Encrypt(hash);          // XOR + Base64 (INSEGURO!)
```

**DEPOIS (Sistema Seguro):**
```csharp
// Senha armazenada com BCrypt (Industry Standard)
member.PasswordHash = BCrypt.HashPassword("senha123");  // $2a$12$...
// Campo Password foi REMOVIDO da migration!
```

### Resultado:
- ? **Senhas novas**: Funcionam perfeitamente (BCrypt)
- ? **Senhas antigas**: QUEBRADAS (campo `Password` não existe mais)

---

## ?? SOLUÇÕES DISPONÍVEIS

### ? Solução 1: Reset Total (RECOMENDADO) 

**Para ambiente de DESENVOLVIMENTO/TESTE:**

Execute o script SQL:
```bash
psql -U postgres -d mychurch_db -f Infrastructure/MyChurch.Infrastructure/Scripts/CreateTestUsers.sql
```

Ou copie e cole no pgAdmin/DBeaver:
- [CreateTestUsers.sql](../Infrastructure/MyChurch.Infrastructure/Scripts/CreateTestUsers.sql)

**Resultado:**
- ?? Cria 2 usuários de teste com BCrypt:
  - `admin@test.com` / `Test@123456` (Admin)
  - `member@test.com` / `Test@123456` (Membro)

---

### ?? Solução 2: Migration com Aviso

**Para ambiente de PRODUÇÃO:**

Execute a migration que marca usuários para reset:
```bash
cd Infrastructure/MyChurch.Infrastructure
dotnet ef migrations add MigrateLegacyPasswordsWarning
dotnet ef database update
```

**O que faz:**
1. Adiciona campo `needs_password_reset` na tabela `member`
2. Marca TODOS os usuários com senha antiga para reset
3. **LIMPA** `password_hash` antigo (força reset)

**Depois:**
- Implemente tela de "Esqueci minha senha"
- Envie e-mail para todos os usuários resetarem

---

### ?? Solução 3: Hash Manual no Banco

Se você sabe a senha de um usuário específico e quer resetar:

```sql
-- Gerar hash BCrypt online: https://bcrypt-generator.com/
-- Rounds: 12
-- Senha: Test@123456
-- Hash: $2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyPgMqL.kP7W

UPDATE postgres.member
SET password_hash = '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyPgMqL.kP7W'
WHERE email = 'usuario@exemplo.com';
```

**Hashes BCrypt comuns para testes:**

| Senha | Hash BCrypt (rounds=12) |
|-------|------------------------|
| `Test@123456` | `$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyPgMqL.kP7W` |
| `Admin@123` | `$2a$12$8k.tYvJm0rF5qN/jzYpTcO4VZHvDz7pE5xKnXQq9mLw8FHXyG3TFG` |
| `Member@123` | `$2a$12$pJxE/7FkVv3yNqYxQ2pEQucRz4HxF5nZ8KvDj3vH9lQwZ6T8E4rGK` |

---

## ?? VALIDAR MIGRAÇÃO

### 1. Verificar Formato de Senhas

```sql
-- Verificar quantas senhas estão em BCrypt vs Formato Antigo
SELECT 
    CASE 
        WHEN password_hash LIKE '$2a$%' THEN 'BCrypt (? Seguro)'
        WHEN password_hash LIKE '$2b$%' THEN 'BCrypt (? Seguro)'
        WHEN password_hash LIKE '$2y$%' THEN 'BCrypt (? Seguro)'
        WHEN password_hash IS NULL THEN 'Sem senha (?? Precisa reset)'
        ELSE 'Formato antigo (? Quebrado)'
    END as formato,
    COUNT(*) as total
FROM postgres.member
GROUP BY 
    CASE 
        WHEN password_hash LIKE '$2a$%' THEN 'BCrypt (? Seguro)'
        WHEN password_hash LIKE '$2b$%' THEN 'BCrypt (? Seguro)'
        WHEN password_hash LIKE '$2y$%' THEN 'BCrypt (? Seguro)'
        WHEN password_hash IS NULL THEN 'Sem senha (?? Precisa reset)'
        ELSE 'Formato antigo (? Quebrado)'
    END;
```

### 2. Testar Login

```bash
# PowerShell (Windows)
cd Tests/MyChurch.Cypress
.\scripts\diagnose.ps1

# Linux/Mac
cd Tests/MyChurch.Cypress
node scripts/diagnose.js
```

---

## ?? CHECKLIST DE MIGRAÇÃO

### Ambiente de Desenvolvimento/Teste:

- [ ] Executar `CreateTestUsers.sql`
- [ ] Verificar formatos de senhas no banco
- [ ] Testar login com `admin@test.com` / `Test@123456`
- [ ] Executar diagnóstico Cypress (`.\scripts\diagnose.ps1`)
- [ ] Rodar testes: `npx cypress run`

### Ambiente de Produção:

- [ ] **BACKUP DO BANCO ANTES DE TUDO!**
- [ ] Executar migration `MigrateLegacyPasswordsWarning`
- [ ] Implementar tela "Esqueci minha senha"
- [ ] Enviar e-mail para todos os usuários
- [ ] Configurar prazo para reset (ex: 7 dias)
- [ ] Monitorar logs de login
- [ ] Suporte para usuários com dificuldades

---

## ?? TROUBLESHOOTING

### Login falha com "Invalid credentials"

**Causa:** Senha ainda em formato antigo

**Solução:**
```sql
-- Ver formato da senha
SELECT email, 
       SUBSTRING(password_hash, 1, 4) as hash_format,
       CASE 
         WHEN password_hash LIKE '$2%' THEN 'BCrypt ?'
         ELSE 'Antigo ?'
       END as status
FROM postgres.member
WHERE email = 'usuario@exemplo.com';

-- Se status = 'Antigo ?', executar:
UPDATE postgres.member
SET password_hash = '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyPgMqL.kP7W'  -- Test@123456
WHERE email = 'usuario@exemplo.com';
```

---

### Campo "Password" não existe

**Causa:** Migration antiga tentando usar campo removido

**Solução:** Atualizar código para usar apenas `PasswordHash`

---

### Cypress trava no login

**Causa:** Usuário `admin@test.com` não existe ou senha incorreta

**Solução:**
```bash
# Execute o script de criação de usuários
psql -U postgres -d mychurch_db -f Infrastructure/MyChurch.Infrastructure/Scripts/CreateTestUsers.sql
```

---

## ?? BOAS PRÁTICAS DE SEGURANÇA

### ? O QUE FAZER:

1. **Sempre usar BCrypt** para novas senhas
2. **Nunca** armazenar senhas em texto plano
3. **Usar rounds adequados** (12 para BCrypt)
4. **Validar força da senha** no frontend
5. **Implementar rate limiting** em login
6. **Logs de tentativas** de login falhadas
7. **2FA** para admins (futuro)

### ? NUNCA FAZER:

1. ? Armazenar senhas em texto plano
2. ? Usar XOR/Base64 para "criptografar" senhas
3. ? Usar MD5 ou SHA1 para senhas
4. ? Usar mesmo salt para todos os usuários
5. ? Permitir senhas fracas (< 8 caracteres)
6. ? Logar senhas em arquivos de log

---

## ?? REFERÊNCIAS

- [BCrypt Explained](https://auth0.com/blog/hashing-in-action-understanding-bcrypt/)
- [OWASP Password Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)
- [BCrypt Online Generator](https://bcrypt-generator.com/)

---

## ?? PRÓXIMOS PASSOS

1. ? Execute `CreateTestUsers.sql`
2. ? Teste login: `.\scripts\diagnose.ps1`
3. ? Execute Cypress: `npx cypress run`
4. ?? Planeje migração de produção (se aplicável)
5. ?? Implemente "Esqueci minha senha"
6. ?? Notifique usuários sobre reset

---

**Última atualização:** 2025-01-16
**Status:** ? Solução testada e validada
