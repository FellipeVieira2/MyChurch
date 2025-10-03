@echo off
REM Script para aplicar migrations do MyChurch (Windows)

echo ?? Aplicando Migrations do MyChurch...
echo.

REM Verificar se dotnet está instalado
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ? .NET SDK não encontrado. Por favor, instale o .NET 8 SDK.
    exit /b 1
)

echo ?? Verificando projetos...

REM Diretório base
set BASE_DIR=%~dp0..
set STARTUP_PROJECT=%BASE_DIR%\src\Web\MyChurch.Api.Web
set INFRA_PROJECT=%BASE_DIR%\src\Infrastructure\MyChurch.Infrastructure

REM Verificar se os projetos existem
if not exist "%STARTUP_PROJECT%\MyChurch.Api.Web.csproj" (
    echo ? Projeto Web não encontrado em: %STARTUP_PROJECT%
    exit /b 1
)

if not exist "%INFRA_PROJECT%\MyChurch.Infrastructure.csproj" (
    echo ? Projeto Infrastructure não encontrado em: %INFRA_PROJECT%
    exit /b 1
)

echo ? Projetos encontrados
echo.

REM Listar migrations pendentes
echo ?? Migrations disponíveis:
dotnet ef migrations list --startup-project "%STARTUP_PROJECT%" --project "%INFRA_PROJECT%"

echo.
echo ?? Aplicando migrations...

REM Aplicar migrations
dotnet ef database update --startup-project "%STARTUP_PROJECT%" --project "%INFRA_PROJECT%"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ? Migrations aplicadas com sucesso!
    echo.
    echo ?? Melhorias aplicadas:
    echo   ? Template de email UTF-8 corrigido
    echo   ? Validação de votos implementada
    echo   ? Índices de performance adicionados
    echo   ? Cache distribuído configurado
    echo   ? Sistema de localização atualizado
    echo   ? Moderação automática implementada
    echo.
    echo ?? Próximos passos (opcional):
    echo   1. Configurar Redis para cache em produção
    echo   2. Configurar Google Maps API Key
    echo   3. Testar endpoints de reviews e localização
    echo.
) else (
    echo.
    echo ? Erro ao aplicar migrations
    echo Verifique a string de conexão no appsettings.json
    exit /b 1
)
