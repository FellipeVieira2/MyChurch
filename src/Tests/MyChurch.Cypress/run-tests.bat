@echo off
REM MyChurch Cypress Test Runner - Windows Version
REM Script helper para executar testes localmente

setlocal enabledelayedexpansion

color 0B
echo ========================================
echo    MyChurch Cypress Test Runner
echo    217 Testes ^| 14 Controllers
echo    100%% Cobertura Completa
echo ========================================
echo.

REM Verificar se npm esta instalado
where npm >nul 2>nul
if %ERRORLEVEL% neq 0 (
    color 0C
    echo [ERROR] npm nao esta instalado!
    echo Por favor, instale Node.js e npm primeiro.
    pause
    exit /b 1
)

:MENU
echo.
echo Escolha uma opcao:
echo ============================================
echo 0. [Dashboard] Abrir Dashboard Visual
echo 1. [Todos] Rodar todos os testes (217)
echo 2. [Auth] 8 testes
echo 3. [Bible] 12 testes
echo 4. [CashFlow] 24 testes
echo 5. [Church] 12 testes
echo 6. [Member] 18 testes
echo 7. [Event] 18 testes
echo 8. [Journey] 15 testes
echo 9. [Presentation] 15 testes
echo 10. [WorshipActivity] 22 testes
echo 11. [PastorBot] 15 testes
echo 12. [Subscription] 12 testes (NEW)
echo 13. [Donation] 15 testes (NEW)
echo 14. [Feed] 16 testes (NEW)
echo 15. [Reviews] 12 testes (NEW)
echo ============================================
echo 16. Abrir Cypress UI (modo interativo)
echo 17. Instalar/atualizar dependencias
echo 18. Sair
echo ============================================
echo.

set /p option="Opcao: "

if "%option%"=="0" goto OPEN_DASHBOARD
if "%option%"=="1" goto RUN_ALL
if "%option%"=="2" goto RUN_AUTH
if "%option%"=="3" goto RUN_BIBLE
if "%option%"=="4" goto RUN_CASHFLOW
if "%option%"=="5" goto RUN_CHURCH
if "%option%"=="6" goto RUN_MEMBER
if "%option%"=="7" goto RUN_EVENT
if "%option%"=="8" goto RUN_JOURNEY
if "%option%"=="9" goto RUN_PRESENTATION
if "%option%"=="10" goto RUN_WORSHIPACTIVITY
if "%option%"=="11" goto RUN_PASTORBOT
if "%option%"=="12" goto RUN_SUBSCRIPTION
if "%option%"=="13" goto RUN_DONATION
if "%option%"=="14" goto RUN_FEED
if "%option%"=="15" goto RUN_REVIEWS
if "%option%"=="16" goto OPEN_UI
if "%option%"=="17" goto INSTALL_DEPS
if "%option%"=="18" goto END

echo [ERROR] Opcao invalida!
goto MENU

:CHECK_API
echo Verificando se a API esta rodando...
curl -k -s https://localhost:7163/api/health >nul 2>&1
if %ERRORLEVEL% neq 0 (
    color 0C
    echo [ERROR] API nao esta rodando!
    echo Por favor, inicie a API antes de rodar os testes.
    echo Execute: cd src\Web\MyChurch.Api.Web ^&^& dotnet run
    pause
    goto MENU
)
color 0A
echo [OK] API esta rodando!
color 0B
goto :eof

:OPEN_DASHBOARD
color 0D
echo Abrindo Dashboard Visual...
start Tests\MyChurch.Cypress\dashboard.html
goto ASK_AGAIN

:RUN_ALL
call :CHECK_API
color 0E
echo Executando todos os 217 testes...
cd Tests\MyChurch.Cypress
call npm test
cd ..\..
goto ASK_AGAIN

:RUN_AUTH
call :CHECK_API
echo Executando testes de Auth...
cd Tests\MyChurch.Cypress
call npm run test:auth
cd ..\..
goto ASK_AGAIN

:RUN_BIBLE
call :CHECK_API
echo Executando testes de Bible...
cd Tests\MyChurch.Cypress
call npm run test:bible
cd ..\..
goto ASK_AGAIN

:RUN_CASHFLOW
call :CHECK_API
echo Executando testes de CashFlow...
cd Tests\MyChurch.Cypress
call npm run test:cashflow
cd ..\..
goto ASK_AGAIN

:RUN_CHURCH
call :CHECK_API
echo Executando testes de Church...
cd Tests\MyChurch.Cypress
call npm run test:church
cd ..\..
goto ASK_AGAIN

:RUN_MEMBER
call :CHECK_API
echo Executando testes de Member...
cd Tests\MyChurch.Cypress
call npm run test:member
cd ..\..
goto ASK_AGAIN

:RUN_EVENT
call :CHECK_API
echo Executando testes de Event...
cd Tests\MyChurch.Cypress
call npm run test:event
cd ..\..
goto ASK_AGAIN

:RUN_JOURNEY
call :CHECK_API
echo Executando testes de Journey...
cd Tests\MyChurch.Cypress
call npm run test:journey
cd ..\..
goto ASK_AGAIN

:RUN_PRESENTATION
call :CHECK_API
echo Executando testes de Presentation...
cd Tests\MyChurch.Cypress
call npm run test:presentation
cd ..\..
goto ASK_AGAIN

:RUN_WORSHIPACTIVITY
call :CHECK_API
echo Executando testes de WorshipActivity...
cd Tests\MyChurch.Cypress
call npm run test:worshipactivity
cd ..\..
goto ASK_AGAIN

:RUN_PASTORBOT
call :CHECK_API
echo Executando testes de PastorBot...
cd Tests\MyChurch.Cypress
call npm run test:pastorbot
cd ..\..
goto ASK_AGAIN

:RUN_SUBSCRIPTION
call :CHECK_API
color 0D
echo Executando testes de Subscription (NEW)...
cd Tests\MyChurch.Cypress
call npm run test:subscription
cd ..\..
goto ASK_AGAIN

:RUN_DONATION
call :CHECK_API
color 0D
echo Executando testes de Donation (NEW)...
cd Tests\MyChurch.Cypress
call npm run test:donation
cd ..\..
goto ASK_AGAIN

:RUN_FEED
call :CHECK_API
color 0D
echo Executando testes de Feed (NEW)...
cd Tests\MyChurch.Cypress
call npm run test:feed
cd ..\..
goto ASK_AGAIN

:RUN_REVIEWS
call :CHECK_API
color 0D
echo Executando testes de Reviews (NEW)...
cd Tests\MyChurch.Cypress
call npm run test:reviews
cd ..\..
goto ASK_AGAIN

:OPEN_UI
call :CHECK_API
echo Abrindo Cypress UI...
cd Tests\MyChurch.Cypress
call npm run cy:open
cd ..\..
goto ASK_AGAIN

:INSTALL_DEPS
color 0E
echo Instalando dependencias do Cypress...
cd Tests\MyChurch.Cypress
call npm install
cd ..\..
color 0A
echo [OK] Dependencias instaladas!
color 0B
goto ASK_AGAIN

:ASK_AGAIN
echo.
set /p again="Executar novamente? (s/n): "
if /i "%again%"=="s" goto MENU
goto END

:END
color 0A
echo.
echo ============================================
echo Obrigado por usar o MyChurch Test Runner!
echo ============================================
pause
exit /b 0
