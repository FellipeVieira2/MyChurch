Write-Host "?? Testando ambiente MyChurch API..." -ForegroundColor Cyan
Write-Host ""

# Configurações
$apiUrl = "http://localhost:5210"
$timeout = 5

# 1. Testar se API está acessível
Write-Host "1?? Testando conexão com API..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$apiUrl/swagger/index.html" -UseBasicParsing -TimeoutSec $timeout -ErrorAction Stop
    
    if ($response.StatusCode -eq 200) {
        Write-Host "   ? API está rodando!" -ForegroundColor Green
        $apiRunning = $true
    }
} catch {
    Write-Host "   ? API não está acessível" -ForegroundColor Red
    Write-Host "   ?? Execute: dotnet run --project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj" -ForegroundColor Yellow
    $apiRunning = $false
}

if (-not $apiRunning) {
    Write-Host ""
    Write-Host "? Não é possível continuar sem a API rodando" -ForegroundColor Red
    exit 1
}

# 2. Testar endpoint de login
Write-Host ""
Write-Host "2?? Testando endpoint de login..." -ForegroundColor Yellow
try {
    $body = @{
        identifier = "admin@test.com"
        password = "Test@123456"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "$apiUrl/api/auth/login" `
        -Method Post `
        -Body $body `
        -ContentType "application/json" `
        -TimeoutSec 10 `
        -ErrorAction Stop
    
    if ($response.token) {
        Write-Host "   ? Login funcionando!" -ForegroundColor Green
        Write-Host "   ? Usuário: $($response.member.name)" -ForegroundColor Green
        $loginOk = $true
    }
} catch {
    Write-Host "   ? Login falhou" -ForegroundColor Red
    Write-Host "   ?? Erro: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "   ?? Possíveis causas:" -ForegroundColor Yellow
    Write-Host "      - Usuário admin@test.com não existe no banco"
    Write-Host "      - Senha incorreta"
    Write-Host "      - Migrations não executadas"
    Write-Host ""
    Write-Host "   ?? Solução:" -ForegroundColor Yellow
    Write-Host "      1. cd Infrastructure\MyChurch.Infrastructure"
    Write-Host "      2. dotnet ef database update"
    Write-Host "      3. Criar usuário admin (consulte TROUBLESHOOTING.md)"
    $loginOk = $false
}

# 3. Testar endpoint de busca
Write-Host ""
Write-Host "3?? Testando endpoint de busca..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$apiUrl/api/church/search/nearby?latitude=-23.5505&longitude=-46.6333&radiusKm=10" `
        -Method Get `
        -TimeoutSec 10 `
        -ErrorAction Stop
    
    Write-Host "   ? Endpoint de busca funcionando!" -ForegroundColor Green
    Write-Host "   ?? $($response.totalResults) igrejas encontradas" -ForegroundColor Green
} catch {
    Write-Host "   ? Busca falhou" -ForegroundColor Red
    Write-Host "   ?? Erro: $($_.Exception.Message)" -ForegroundColor Red
}

# Resumo
Write-Host ""
Write-Host "?" * 60 -ForegroundColor Cyan
Write-Host "  RESUMO DO DIAGNÓSTICO" -ForegroundColor Cyan
Write-Host "?" * 60 -ForegroundColor Cyan
Write-Host ""

if ($apiRunning -and $loginOk) {
    Write-Host "? Ambiente pronto para testes!" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Executar testes Cypress:" -ForegroundColor Cyan
    Write-Host "   cd Tests\MyChurch.Cypress" -ForegroundColor Yellow
    Write-Host "   npx cypress open" -ForegroundColor Yellow
    Write-Host "   ou" -ForegroundColor Yellow
    Write-Host "   npx cypress run" -ForegroundColor Yellow
} else {
    Write-Host "? Ambiente com problemas" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? Consulte: Tests\MyChurch.Cypress\TROUBLESHOOTING.md" -ForegroundColor Yellow
}

Write-Host ""
