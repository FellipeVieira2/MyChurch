# ?? SCRIPT DE RESET COMPLETO DO BANCO DE DADOS (PowerShell)

Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  RESET COMPLETO DO BANCO DE DADOS" -ForegroundColor Cyan
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "??  ATENÇÃO: Isso vai DELETAR TODOS OS DADOS!" -ForegroundColor Red
Write-Host "    Use apenas em DESENVOLVIMENTO!" -ForegroundColor Red
Write-Host ""

$confirmation = Read-Host "Tem certeza? Digite 'SIM' para continuar"

if ($confirmation -ne "SIM") {
    Write-Host "? Operação cancelada." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "?? Iniciando reset do banco de dados..." -ForegroundColor Yellow
Write-Host ""

# Passo 1: Dropar o banco
Write-Host "1?? Dropando banco de dados..." -ForegroundColor Cyan
dotnet ef database drop --force `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Erro ao dropar banco. Verifique se o PostgreSQL está rodando." -ForegroundColor Red
    Write-Host "?? Tente fechar todas as conexões com o banco primeiro." -ForegroundColor Yellow
    exit 1
}

Write-Host "   ? Banco dropado com sucesso" -ForegroundColor Green
Write-Host ""

# Passo 2: Recriar com todas as migrations
Write-Host "2?? Recriando banco com todas as migrations..." -ForegroundColor Cyan
dotnet ef database update `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Erro ao aplicar migrations." -ForegroundColor Red
    exit 1
}

Write-Host "   ? Migrations aplicadas com sucesso" -ForegroundColor Green
Write-Host ""

# Passo 3: Listar migrations aplicadas
Write-Host "3?? Verificando migrations aplicadas..." -ForegroundColor Cyan
dotnet ef migrations list `
    --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj `
    --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj

Write-Host ""
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ? RESET COMPLETO! Banco limpo e atualizado" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Próximos passos:" -ForegroundColor Cyan
Write-Host "   1. Rode a API: dotnet run --project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj" -ForegroundColor Yellow
Write-Host "   2. Teste login: cd Tests\MyChurch.Cypress; .\scripts\diagnose.ps1" -ForegroundColor Yellow
Write-Host "   3. Execute Cypress: npx cypress open" -ForegroundColor Yellow
Write-Host ""
