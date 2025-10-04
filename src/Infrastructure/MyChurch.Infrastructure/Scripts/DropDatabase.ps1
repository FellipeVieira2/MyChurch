# ?? DROP MANUAL DO BANCO DE DADOS

Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  DROP MANUAL DO BANCO MyChurch" -ForegroundColor Cyan
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "??  ATENÇÃO: Isso vai DELETAR TODOS OS DADOS!" -ForegroundColor Red
Write-Host ""

# Configurações
$dbHost = "localhost"
$dbPort = "5432"
$dbUser = "postgres"
$dbName = "mychurch_db"

Write-Host "?? Configurações:" -ForegroundColor Cyan
Write-Host "   Host: $dbHost" -ForegroundColor Gray
Write-Host "   Porta: $dbPort" -ForegroundColor Gray
Write-Host "   Usuário: $dbUser" -ForegroundColor Gray
Write-Host "   Banco: $dbName" -ForegroundColor Gray
Write-Host ""

$confirmation = Read-Host "Tem certeza? Digite 'SIM' para continuar"

if ($confirmation -ne "SIM") {
    Write-Host "? Operação cancelada." -ForegroundColor Yellow
    exit 0
}

# Pedir senha
$dbPassword = Read-Host "Digite a senha do PostgreSQL" -AsSecureString
$dbPasswordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($dbPassword))

Write-Host ""
Write-Host "?? Executando DROP DATABASE..." -ForegroundColor Yellow
Write-Host ""

# Set PGPASSWORD
$env:PGPASSWORD = $dbPasswordPlain

try {
    # 1. Fechar conexões
    Write-Host "1?? Fechando conexões ativas..." -ForegroundColor Cyan
    $killConnectionsQuery = @"
SELECT pg_terminate_backend(pid) 
FROM pg_stat_activity 
WHERE datname = '$dbName' 
  AND pid <> pg_backend_pid();
"@
    
    echo $killConnectionsQuery | psql -h $dbHost -p $dbPort -U $dbUser -d postgres 2>&1 | Out-Null
    Write-Host "   ? Conexões fechadas" -ForegroundColor Green
    
    # 2. Drop database
    Write-Host "2?? Dropando banco de dados..." -ForegroundColor Cyan
    $dropQuery = "DROP DATABASE IF EXISTS $dbName;"
    
    echo $dropQuery | psql -h $dbHost -p $dbPort -U $dbUser -d postgres 2>&1 | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ? Banco dropado com sucesso" -ForegroundColor Green
    } else {
        Write-Host "   ??  Erro ao dropar (pode não existir)" -ForegroundColor Yellow
    }
    
    # 3. Create database
    Write-Host "3?? Criando novo banco..." -ForegroundColor Cyan
    $createQuery = @"
CREATE DATABASE $dbName
  WITH 
  ENCODING = 'UTF8'
  LC_COLLATE = 'en_US.UTF-8'
  LC_CTYPE = 'en_US.UTF-8'
  TEMPLATE = template0;
"@
    
    echo $createQuery | psql -h $dbHost -p $dbPort -U $dbUser -d postgres 2>&1 | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ? Banco criado com sucesso" -ForegroundColor Green
    } else {
        Write-Host "   ? Erro ao criar banco" -ForegroundColor Red
        exit 1
    }
    
    # 4. NÃO precisa criar schema 'postgres' - ele já existe por padrão!
    # O schema 'postgres' é criado automaticamente pelo PostgreSQL
    Write-Host "4?? Schema 'postgres' disponível (schema padrão do PostgreSQL)" -ForegroundColor Cyan
    Write-Host "   ? Pronto para migrations" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "  ? BANCO DROPADO E RECRIADO COM SUCESSO!" -ForegroundColor Green
    Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "?? Próximo passo: Aplicar migrations" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Execute:" -ForegroundColor Yellow
    Write-Host "cd C:\Users\Usuario\source\repos\FellipeVieira2\MyChurch\src" -ForegroundColor Gray
    Write-Host ""
    Write-Host "dotnet ef database update ``" -ForegroundColor Gray
    Write-Host "    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj ``" -ForegroundColor Gray
    Write-Host "    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj ``" -ForegroundColor Gray
    Write-Host "    --verbose" -ForegroundColor Gray
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Host "? Erro ao executar comandos SQL: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? Verifique:" -ForegroundColor Yellow
    Write-Host "   - PostgreSQL está rodando?" -ForegroundColor Gray
    Write-Host "   - psql está instalado e no PATH?" -ForegroundColor Gray
    Write-Host "   - Senha está correta?" -ForegroundColor Gray
    Write-Host ""
    exit 1
} finally {
    # Limpar senha
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
}
