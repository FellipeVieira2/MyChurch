# ?? Guia de Execução - MyChurch App

## ?? Pré-requisitos

- ? .NET 8 ou .NET 9 SDK instalado
- ? Visual Studio 2022 (ou VS Code com extensões MAUI)
- ? Emulador/Dispositivo para testar (Android, iOS ou Windows)

---

## ?? Configuração Inicial

### 1?? **Configurar URL da API**

A API está configurada para rodar na porta **5210**.

**Arquivo:** `App/Mychurch.App/Services/AuthService.cs`

```csharp
// Linha 17 - URL configurada:
private const string API_BASE_URL = "https://localhost:5210/api"; // ? Porta 5210
```

**Opções de URL conforme ambiente:**
- **Desenvolvimento local (mesma máquina):** `https://localhost:5210/api`
- **Emulador Android:** `https://10.0.2.2:5210/api` (localhost do host)
- **Dispositivo físico na mesma rede:** `https://192.168.x.x:5210/api` (IP da máquina)
- **API em produção:** `https://api.mychurch.com/api`

---

## ?? Como Executar

### **Opção A: Visual Studio (RECOMENDADO)**

#### **1. Configurar Projetos de Inicialização Múltiplos:**

1. Clique com botão direito na **Solution** (solução)
2. **Properties** (ou **Set Startup Projects**)
3. Selecione **Multiple startup projects**
4. Configure:
   ```
   ? MyChurch.Api.Web      ? Start
   ? Mychurch.App          ? Start
   ```
5. Clique **OK**
6. Pressione **F5** para executar

#### **2. Ou Execute Manualmente (dois projetos):**

**Passo 1 - Rodar a API:**
- Clique com botão direito em `MyChurch.Api.Web`
- **Debug** ? **Start New Instance**
- Verifique se está rodando na porta **5210** (veja no console)

**Passo 2 - Rodar o App:**
- Clique com botão direito em `Mychurch.App`
- **Debug** ? **Start New Instance**
- Selecione a plataforma (Windows, Android, iOS)

---

### **Opção B: Linha de Comando**

#### **Terminal 1 - Rodar a API:**
```powershell
cd C:\Users\Usuario\source\repos\FellipeVieira2\MyChurch\src\Web\MyChurch.Api.Web
dotnet run
```

> **Importante:** Verifique se está rodando na porta **5210** (ex: `https://localhost:5210`)

#### **Terminal 2 - Rodar o App:**
```powershell
cd C:\Users\Usuario\source\repos\FellipeVieira2\MyChurch\src\App\Mychurch.App
dotnet run
```

---

## ?? Verificar se a API está rodando

**Teste no navegador:**
```
https://localhost:5210/swagger
```

Se abrir a página do Swagger, a API está funcionando! ?

**Teste o endpoint de login:**
```
https://localhost:5210/api/Auth/login
```

---

## ?? Plataformas Suportadas

O app funciona em:

| Plataforma | Status | Como Testar |
|------------|--------|-------------|
| ? Windows | Suportado | Selecione `Windows Machine` no VS |
| ? Android | Suportado | Selecione emulador Android ou dispositivo |
| ? iOS | Suportado | Requer Mac com Xcode |
| ? macOS | Suportado | Requer Mac |

---

## ?? Problemas Comuns

### **1. Erro de conexão com a API**

**Problema:** `Falha na comunicação com o servidor`

**Solução:**
- ? Verifique se a API está rodando (`https://localhost:5210/swagger`)
- ? Confirme a URL no `AuthService.cs` (deve ser porta **5210**)
- ? Se estiver no Android, use `https://10.0.2.2:5210/api` no AuthService
- ? Verifique se não há firewall bloqueando a porta 5210

### **2. Certificado SSL inválido (desenvolvimento)**

**Solução:**
```powershell
dotnet dev-certs https --trust
```

### **3. Campos de texto pequenos na tela de login**

**Solução:** ? Já corrigido! Os campos agora ocupam 100% do espaço disponível com `flex: 1 1 auto`.

### **4. Porta incorreta**

**Problema:** API não responde

**Solução:** Verifique se a API está configurada para rodar na porta **5210**. Confira em:
- `launchSettings.json` do projeto `MyChurch.Api.Web`
- A URL que aparece ao rodar a API no terminal

---

## ?? Credenciais de Teste

Para testar o login, use credenciais válidas cadastradas na API:

**Exemplo:**
- **E-mail:** `admin@mychurch.com`
- **Senha:** `SuaSenha123`

> **Nota:** Crie um usuário na API primeiro se ainda não tiver!

---

## ?? Fluxo da Aplicação

```
1. App abre
   ?
2. Verifica autenticação (SecureStorage)
   ?
3. Se NÃO logado ? Tela de Login
   ?
4. Usuário digita credenciais
   ?
5. App faz POST para API (https://localhost:5210/api/Auth/login)
   ?
6. API retorna Token + Dados do usuário
   ?
7. App salva no SecureStorage
   ?
8. Redireciona para /home
   ?
9. Exibe dados do usuário
   ?
10. Botão "Sair" ? Limpa storage ? Volta para Login
```

---

## ? Checklist de Configuração

Antes de executar, verifique:

- [ ] API rodando na porta **5210**
- [ ] Swagger acessível em `https://localhost:5210/swagger`
- [ ] `AuthService.cs` configurado com `https://localhost:5210/api`
- [ ] Certificado SSL confiável (`dotnet dev-certs https --trust`)
- [ ] Visual Studio configurado para iniciar ambos os projetos

---

## ?? Próximos Passos

Após configurar e rodar com sucesso:

1. ? Testar login com usuário válido
2. ? Verificar navegação entre páginas
3. ? Testar logout
4. ? Adicionar mais funcionalidades

---

## ?? Notas Importantes

- ?? **Porta 5210:** A API está configurada para esta porta
- ?? **HTTPS:** Em desenvolvimento, use certificados confiáveis
- ?? **SecureStorage:** Armazena token de forma segura no dispositivo
- ?? **Campos de Input:** Agora ocupam 100% do espaço disponível
- ?? **CORS:** Configure CORS na API se necessário

---

**Dúvidas?** Consulte a documentação do .NET MAUI: https://learn.microsoft.com/dotnet/maui
