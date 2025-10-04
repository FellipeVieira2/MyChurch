# ?? SCRIPT PARA LIMPAR MIGRATIONS CONFLITANTES

Write-Host "?? Limpando migrations antigas e conflitantes..." -ForegroundColor Cyan
Write-Host ""

# Lista de migrations problemáticas para remover (ajuste conforme necessário)
$problematicMigrations = @(
    "20251004012412_addnewcolums",
    # Adicione outras migrations duplicadas aqui se necessário
)

Write-Host "?? Migrations que serão removidas:" -ForegroundColor Yellow
$problematicMigrations | ForEach-Object { Write-Host "   - $_" -ForegroundColor Red }
Write-Host ""

$confirmation = Read-Host "Continuar? (S/N)"
if ($confirmation -ne "S") {
    Write-Host "? Operação cancelada." -ForegroundColor Yellow
    exit 0
}

# Remover arquivos de migration
foreach ($migration in $problematicMigrations) {
    $migrationFile = "Infrastructure\MyChurch.Infrastructure\Migrations\$migration.cs"
    $designerFile = "Infrastructure\MyChurch.Infrastructure\Migrations\$migration.Designer.cs"
    
    if (Test-Path $migrationFile) {
        Remove-Item $migrationFile -Force
        Write-Host "   ? Removido: $migration.cs" -ForegroundColor Green
    }
    
    if (Test-Path $designerFile) {
        Remove-Item $designerFile -Force
        Write-Host "   ? Removido: $migration.Designer.cs" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "? Limpeza concluída!" -ForegroundColor Green
Write-Host ""
Write-Host "?? Próximo passo: Aplique as migrations corretas" -ForegroundColor Cyan
Write-Host "   dotnet ef database update --project Infrastructure\MyChurch.Infrastructure\MyChurch.Infrastructure.csproj --startup-project Web\MyChurch.Api.Web\MyChurch.Api.Web.csproj" -ForegroundColor Yellow
