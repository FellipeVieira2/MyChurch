# Script para corrigir o problema da tabela Reviews
# Este script cria a tabela Reviews que estava faltando devido a uma migration vazia

param(
    [string]$ConnectionString = "Host=localhost;Port=5432;Database=mychurch_db;Username=postgres;Password=postgres"
)

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Fix Reviews Table Migration Issue" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se psql está disponível
try {
    $psqlVersion = psql --version
    Write-Host "? PostgreSQL client encontrado: $psqlVersion" -ForegroundColor Green
} catch {
    Write-Host "? ERRO: PostgreSQL client (psql) não encontrado!" -ForegroundColor Red
    Write-Host "Instale o PostgreSQL ou adicione-o ao PATH do sistema." -ForegroundColor Yellow
    exit 1
}

# Extrair informações da connection string
if ($ConnectionString -match "Host=([^;]+).*Database=([^;]+).*Username=([^;]+).*Password=([^;]+)") {
    $Host = $Matches[1]
    $Database = $Matches[2]
    $Username = $Matches[3]
    $Password = $Matches[4]
} else {
    Write-Host "? ERRO: Connection string inválida!" -ForegroundColor Red
    exit 1
}

Write-Host "Conexão:" -ForegroundColor Yellow
Write-Host "  Host: $Host" -ForegroundColor Gray
Write-Host "  Database: $Database" -ForegroundColor Gray
Write-Host "  Username: $Username" -ForegroundColor Gray
Write-Host ""

# Caminho do script SQL
$scriptPath = Join-Path $PSScriptRoot "FixReviewsTableMigration.sql"

if (-not (Test-Path $scriptPath)) {
    Write-Host "? ERRO: Script SQL não encontrado em: $scriptPath" -ForegroundColor Red
    exit 1
}

Write-Host "Executando script de correção..." -ForegroundColor Yellow

# Definir variável de ambiente para senha (evita prompt)
$env:PGPASSWORD = $Password

try {
    # Executar o script SQL
    $output = psql -h $Host -U $Username -d $Database -f $scriptPath 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "? Script executado com sucesso!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Output:" -ForegroundColor Cyan
        Write-Host $output -ForegroundColor Gray
        Write-Host ""
        Write-Host "Próximos passos:" -ForegroundColor Yellow
        Write-Host "1. Execute: dotnet ef database update --project Infrastructure\MyChurch.Infrastructure --startup-project Web\MyChurch.Api.Web" -ForegroundColor White
        Write-Host "2. Inicie a aplicação para verificar se tudo está funcionando" -ForegroundColor White
    } else {
        Write-Host ""
        Write-Host "? ERRO ao executar o script!" -ForegroundColor Red
        Write-Host $output -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host ""
    Write-Host "? ERRO: $_" -ForegroundColor Red
    exit 1
} finally {
    # Limpar variável de ambiente
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Correção concluída!" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Cyan
