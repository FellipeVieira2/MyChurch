# ?? SCRIPT DE MIGRAÇÃO DE SENHAS - CRÍTICO DE SEGURANÇA

## ?? IMPORTANTE: Leia antes de executar!

Este script serve para migrar senhas existentes de texto plano para hash BCrypt.

### Pré-requisitos:
1. **BACKUP DO BANCO DE DADOS** - Faça backup antes de executar!
2. Certifique-se de que o pacote BCrypt.Net-Next está instalado
3. Teste primeiro em ambiente de desenvolvimento

### Opções de Migração:

#### Opção 1: Reset de Senhas (RECOMENDADO para produção)
```sql
-- Limpar todas as senhas e forçar reset
UPDATE postgres.member 
SET password = NULL, 
    password_hash = gen_random_uuid()::text
WHERE password IS NOT NULL;

-- Enviar email para todos os usuários resetarem suas senhas
```

#### Opção 2: Migração Programática (Apenas DEV - NUNCA use em produção!)
```csharp
// ?? APENAS PARA DESENVOLVIMENTO/TESTES
// NUNCA execute isso em produção com senhas reais!

public class MigratePasswordsCommand : IRequest<Unit>
{
    public class Handler : IRequestHandler<MigratePasswordsCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<Handler> _logger;

        public async Task<Unit> Handle(MigratePasswordsCommand request, CancellationToken cancellationToken)
        {
            // ATENÇÃO: Isso só funciona se você ainda tiver acesso às senhas em texto plano
            // Em produção, você NUNCA deve ter senhas em texto plano!
            
            var members = await _unitOfWork.Members.Query()
                .Where(m => m.Password != null)
                .ToListAsync(cancellationToken);

            foreach (var member in members)
            {
                // Descriptografar senha antiga (se estiver usando o método Encrypt)
                var plainPassword = DecryptOldPassword(member.Password, member.PasswordHash);
                
                // Gerar novo hash BCrypt
                member.PasswordHash = _passwordHasher.HashPassword(plainPassword);
                member.Password = null; // Limpar campo de texto plano
                
                _unitOfWork.Members.Update(member);
            }

            await _unitOfWork.CommitAsync();
            
            _logger.LogInformation("Migrated {Count} passwords to BCrypt", members.Count);
            return Unit.Value;
        }
        
        private string DecryptOldPassword(string encryptedPassword, string key)
        {
            // Implementar lógica de descriptografia do método antigo
            // Isso depende da sua implementação de StringExtensions.Decrypt
            throw new NotImplementedException("Implementar descriptografia do método antigo");
        }
    }
}
```

### Opção 3: Reset Manual Individual
```csharp
// Para cada membro, gerar um novo token de reset de senha
var hash = Guid.NewGuid().ToString("N");
member.PasswordHash = hash;
member.Password = null;

// Enviar email com link de reset:
// https://seusite.com/reset-password?token={hash}
```

## ?? Checklist de Implementação:

- [ ] ? Backup do banco de dados realizado
- [ ] ? Pacote BCrypt.Net-Next instalado
- [ ] ? IPasswordHasher implementado e registrado no DI
- [ ] ? LoginCommandHandler atualizado
- [ ] ? ActiveMemberPasswordCommand atualizado
- [ ] ? AdminChangePasswordCommand atualizado
- [ ] ? Migration para remover campo 'password' criada
- [ ] ? Entidade Member atualizada (removido campo Password)
- [ ] ?? Senhas existentes migradas OU usuários notificados para reset
- [ ] ?? Testes realizados em ambiente de desenvolvimento
- [ ] ?? Migration executada: `dotnet ef database update`

## ?? Após Migração:

1. Verifique se login está funcionando corretamente
2. Teste criação de nova senha
3. Teste reset de senha
4. Monitore logs para erros
5. Certifique-se de que não há mais referências ao campo `Password` no código

## ?? Template de Email para Usuários:

```
Assunto: Atualização de Segurança - Reset de Senha Necessário

Olá [NOME],

Implementamos melhorias de segurança em nosso sistema e, por precaução, 
solicitamos que você redefina sua senha.

Clique no link abaixo para criar uma nova senha:
[LINK DE RESET]

Este link expirará em 24 horas.

Obrigado,
Equipe MyChurch
```

## ?? Verificação Pós-Migração:

```sql
-- Verificar se não há mais senhas em texto plano
SELECT COUNT(*) FROM postgres.member WHERE password IS NOT NULL;
-- Resultado esperado: 0

-- Verificar se todos têm PasswordHash
SELECT COUNT(*) FROM postgres.member WHERE password_hash IS NULL;
-- Resultado esperado: Apenas membros não ativados

-- Verificar formato dos hashes (BCrypt começa com $2a$, $2b$ ou $2y$)
SELECT password_hash FROM postgres.member WHERE password_hash IS NOT NULL LIMIT 5;
-- Resultado esperado: Hashes no formato BCrypt ($2a$12$...)
```

## ?? Documentação BCrypt:
- Work Factor: 12 (recomendado, 2^12 = 4096 iterações)
- Tempo de hash: ~250-500ms (proposital para prevenir brute force)
- Hash inclui salt automaticamente
- Formato: $2a$12$[22 chars de salt][31 chars de hash]

---

**Data:** 16/01/2025  
**Autor:** GitHub Copilot  
**Status:** ?? CRÍTICO - Implementar imediatamente
