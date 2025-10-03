# ?? Guia de Configuração - MyChurch

## ?? Configurações Necessárias

### 1. **String de Conexão do Banco de Dados**

**appsettings.json** (Desenvolvimento)
```json
{
  "ConnectionStrings": {
    "MyChurchDb": "Host=localhost;Database=mychurch;Username=postgres;Password=yourpassword"
  }
}
```

**appsettings.Production.json** (Produção)
```json
{
  "ConnectionStrings": {
    "MyChurchDb": "Host=your-production-host;Database=mychurch;Username=user;Password=secure_password;SSL Mode=Require"
  }
}
```

---

### 2. **Google Maps API (Geocoding)**

Necessário para funcionalidades de localização de igrejas.

**Obter API Key:**
1. Acesse [Google Cloud Console](https://console.cloud.google.com/)
2. Crie um projeto ou selecione existente
3. Ative a API "Geocoding API"
4. Gere uma API Key em "Credenciais"

**appsettings.json**
```json
{
  "GoogleGeocoding": {
    "ApiKey": "AIzaSyXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
  }
}
```

**Restrições recomendadas:**
- Limitar a API apenas para Geocoding API
- Restringir por IP (servidor) ou HTTP referrer (frontend)
- Configurar cota diária para evitar custos excessivos

---

### 3. **Cache Redis (Produção - Opcional)**

**Instalação do Redis:**
```bash
# Docker
docker run --name redis -p 6379:6379 -d redis:alpine

# Ubuntu/Debian
sudo apt-get install redis-server

# macOS
brew install redis
```

**appsettings.Production.json**
```json
{
  "Redis": {
    "ConnectionString": "localhost:6379,abortConnect=false"
  }
}
```

**Habilitar no código:**
Descomentar em `Infrastructure/DependencyInjection.cs`:
```csharp
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:ConnectionString"];
    options.InstanceName = "MyChurch:";
});
```

---

### 4. **AWS S3 (Upload de Arquivos)**

**appsettings.json**
```json
{
  "S3Settings": {
    "AccessKey": "YOUR_AWS_ACCESS_KEY",
    "SecretKey": "YOUR_AWS_SECRET_KEY",
    "BucketName": "mychurch-bucket",
    "Region": "us-east-1"
  }
}
```

**IAM Permissions necessárias:**
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:PutObject",
        "s3:GetObject",
        "s3:DeleteObject"
      ],
      "Resource": "arn:aws:s3:::mychurch-bucket/*"
    }
  ]
}
```

---

### 5. **Email Provider (Postmark/SES)**

#### **Opção A: Postmark** (Recomendado)
```json
{
  "Email": {
    "Remetente": "noreply@yourdomain.com"
  },
  "Postmark": {
    "ServerToken": "YOUR_POSTMARK_SERVER_TOKEN"
  }
}
```

#### **Opção B: AWS SES**
```json
{
  "Email": {
    "Remetente": "noreply@yourdomain.com"
  },
  "AWS": {
    "AccessKey": "YOUR_AWS_ACCESS_KEY",
    "SecretKey": "YOUR_AWS_SECRET_KEY",
    "Region": "us-east-1"
  }
}
```

---

### 6. **JWT Authentication**

**appsettings.json**
```json
{
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARS_LONG_12345",
    "Issuer": "MyChurch",
    "Audience": "MyChurchAPI",
    "ExpirationMinutes": 1440
  }
}
```

**?? IMPORTANTE:** 
- Use uma chave forte em produção (>= 32 caracteres)
- Nunca commite a chave no Git
- Use variáveis de ambiente ou Azure Key Vault

---

### 7. **ASAAS Payment Gateway (Opcional)**

Para funcionalidades de pagamento e doações.

```json
{
  "Asaas": {
    "ApiKey": "YOUR_ASAAS_API_KEY",
    "BaseUrl": "https://sandbox.asaas.com/api/v3",
    "Sandbox": true
  }
}
```

---

## ?? Variáveis de Ambiente (Produção)

Para maior segurança, use variáveis de ambiente:

```bash
# Linux/macOS
export ConnectionStrings__MyChurchDb="Host=prod-db;Database=mychurch;..."
export GoogleGeocoding__ApiKey="AIzaSyXXX..."
export Redis__ConnectionString="redis-prod:6379"
export Jwt__Key="super-secret-production-key"
export S3Settings__AccessKey="AKIAXXXX"
export S3Settings__SecretKey="secretkey"
export Postmark__ServerToken="token"

# Windows PowerShell
$env:ConnectionStrings__MyChurchDb="Host=prod-db;Database=mychurch;..."
$env:GoogleGeocoding__ApiKey="AIzaSyXXX..."
```

**Azure App Service:**
Configurar em: Settings > Configuration > Application Settings

**Docker:**
```yaml
# docker-compose.yml
services:
  api:
    environment:
      - ConnectionStrings__MyChurchDb=Host=db;Database=mychurch;...
      - GoogleGeocoding__ApiKey=AIzaSyXXX...
      - Redis__ConnectionString=redis:6379
```

---

## ?? Aplicar Migrations

### Windows:
```cmd
cd scripts
apply-migrations.bat
```

### Linux/macOS:
```bash
cd scripts
chmod +x apply-migrations.sh
./apply-migrations.sh
```

### Manual:
```bash
dotnet ef database update \
  --startup-project src/Web/MyChurch.Api.Web \
  --project src/Infrastructure/MyChurch.Infrastructure
```

---

## ? Checklist de Configuração

Antes de deploy em produção:

- [ ] String de conexão do banco configurada
- [ ] Google Maps API Key configurada e testada
- [ ] Redis configurado (se usar cache)
- [ ] AWS S3 ou storage alternativo configurado
- [ ] Provider de email configurado (Postmark/SES)
- [ ] JWT Key forte e segura (>= 32 chars)
- [ ] HTTPS habilitado (certificado SSL válido)
- [ ] CORS configurado corretamente
- [ ] Logs configurados (Serilog/Application Insights)
- [ ] Migrations aplicadas
- [ ] Variáveis de ambiente configuradas
- [ ] Secrets não expostos no código
- [ ] Health checks funcionando
- [ ] Rate limiting configurado (opcional)

---

## ?? Verificação de Configuração

**Endpoint de Health Check:**
```bash
GET /health
```

**Testar Geocoding:**
```bash
POST /api/church/1/geocode
Authorization: Bearer {admin_token}
```

**Testar Cache:**
```bash
GET /api/reviews?entityId=1&entityType=Church
# Primeira chamada: busca do banco
# Segunda chamada: retorna do cache
```

---

## ?? Troubleshooting

### Erro: "Geocoding failed"
- Verifique se Google Maps API Key está configurada
- Confirme se Geocoding API está habilitada no Google Cloud
- Verifique restrições de API Key
- Confira logs para ver mensagem de erro completa

### Erro: "Redis connection failed"
- Verifique se Redis está rodando: `redis-cli ping`
- Confirme string de conexão
- Em desenvolvimento, use cache em memória (já configurado)

### Erro: "S3 upload failed"
- Verifique credenciais AWS
- Confirme permissões IAM
- Teste conexão com AWS CLI: `aws s3 ls s3://mychurch-bucket`

### Erro: "Email not sent"
- Verifique token do Postmark ou credenciais SES
- Confirme remetente verificado
- Verifique logs de exceção
