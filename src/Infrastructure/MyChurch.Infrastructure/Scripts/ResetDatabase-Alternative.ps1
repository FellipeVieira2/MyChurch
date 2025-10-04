# ?? RESET DE BANCO - MÉTODO ALTERNATIVO (SEMPRE FUNCIONA)
# Se o Entity Framework estiver com problemas, use SQL direto!

Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  RESET DO BANCO - MÉTODO ALTERNATIVO (SQL DIRETO)" -ForegroundColor Cyan
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "??  ATENÇÃO: Isso vai DELETAR TODOS OS DADOS!" -ForegroundColor Red
Write-Host ""

$confirmation = Read-Host "Tem certeza? Digite 'SIM' para continuar"

if ($confirmation -ne "SIM") {
    Write-Host "? Operação cancelada." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "?? Configurações necessárias:" -ForegroundColor Cyan
$dbHost = Read-Host "Host do PostgreSQL (padrão: localhost)"
if ([string]::IsNullOrWhiteSpace($dbHost)) { $dbHost = "localhost" }

$dbPort = Read-Host "Porta (padrão: 5432)"
if ([string]::IsNullOrWhiteSpace($dbPort)) { $dbPort = "5432" }

$dbUser = Read-Host "Usuário (padrão: postgres)"
if ([string]::IsNullOrWhiteSpace($dbUser)) { $dbUser = "postgres" }

$dbPassword = Read-Host "Senha" -AsSecureString
$dbPasswordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($dbPassword))

$dbName = Read-Host "Nome do banco (padrão: mychurch_db)"
if ([string]::IsNullOrWhiteSpace($dbName)) { $dbName = "mychurch_db" }

Write-Host ""
Write-Host "?? Dropando banco de dados via SQL..." -ForegroundColor Yellow

# Set PGPASSWORD environment variable
$env:PGPASSWORD = $dbPasswordPlain

# Drop database usando psql
$dropCommand = "DROP DATABASE IF EXISTS $dbName;"
$createCommand = "CREATE DATABASE $dbName WITH ENCODING='UTF8' LC_COLLATE='en_US.UTF-8' LC_CTYPE='en_US.UTF-8';"

try {
    # Execute DROP
    Write-Host "   Executando DROP DATABASE..." -ForegroundColor Gray
    echo $dropCommand | psql -h $dbHost -p $dbPort -U $dbUser -d postgres 2>&1 | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ? Banco dropado" -ForegroundColor Green
    } else {
        Write-Host "   ??  Banco não existia ou erro ao dropar" -ForegroundColor Yellow
    }
    
    # Execute CREATE
    Write-Host "   Executando CREATE DATABASE..." -ForegroundColor Gray
    echo $createCommand | psql -h $dbHost -p $dbPort -U $dbUser -d postgres 2>&1 | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ? Banco criado" -ForegroundColor Green
    } else {
        Write-Host "   ? Erro ao criar banco" -ForegroundColor Red
        exit 1
    }
    
} catch {
    Write-Host "? Erro ao executar comandos SQL: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "?? Verifique se o psql está instalado e no PATH" -ForegroundColor Yellow
    Write-Host "?? Ou use pgAdmin para dropar/criar o banco manualmente" -ForegroundColor Yellow
    exit 1
} finally {
    # Clear password from environment
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "?? Aplicando migrations via Entity Framework..." -ForegroundColor Cyan

dotnet ef database update `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj `
    --verbose

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "  ? RESET COMPLETO! Banco limpo e atualizado" -ForegroundColor Green
    Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    
    # Verificar usuários criados
    Write-Host "?? Verificando usuários de teste criados..." -ForegroundColor Cyan
    $env:PGPASSWORD = $dbPasswordPlain
    
    $checkQuery = @"
SELECT email, name, role 
FROM postgres.member 
WHERE email IN ('admin@test.com', 'member@test.com')
ORDER BY email;
"@
    
    echo $checkQuery | psql -h $dbHost -p $dbPort -U $dbUser -d $dbName
    
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
    
    Write-Host ""
    Write-Host "?? Próximos passos:" -ForegroundColor Cyan
    Write-Host "   1. Rode a API: dotnet run --project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj" -ForegroundColor Yellow
    Write-Host "   2. Teste login: cd Tests\MyChurch.Cypress; .\scripts\diagnose.ps1" -ForegroundColor Yellow
    Write-Host "   3. Execute Cypress: npx cypress open" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "? Erro ao aplicar migrations" -ForegroundColor Red
    Write-Host "?? Verifique os logs acima para mais detalhes" -ForegroundColor Yellow
}

Write-Host ""
