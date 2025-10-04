#!/bin/bash

# MyChurch Cypress Test Runner
# Script helper para executar testes localmente

set -e

echo "?? MyChurch Cypress Test Runner"
echo "================================"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para checar se a API está rodando
check_api() {
    echo -e "${YELLOW}Verificando se a API está rodando...${NC}"
    
    for i in {1..10}; do
        if curl -k -s https://localhost:7163/api/health > /dev/null 2>&1; then
            echo -e "${GREEN}? API está rodando!${NC}"
            return 0
        fi
        echo "Tentativa $i/10..."
        sleep 2
    done
    
    echo -e "${RED}? API não está rodando!${NC}"
    echo "Por favor, inicie a API antes de rodar os testes."
    echo "Execute: cd src/Web/MyChurch.Api.Web && dotnet run"
    exit 1
}

# Função para instalar dependências
install_deps() {
    echo -e "${YELLOW}Instalando dependências do Cypress...${NC}"
    cd Tests/MyChurch.Cypress
    npm install
    cd ../..
    echo -e "${GREEN}? Dependências instaladas!${NC}"
}

# Função para rodar todos os testes
run_all() {
    echo -e "${YELLOW}Executando todos os 217 testes...${NC}"
    cd Tests/MyChurch.Cypress
    npm test
    cd ../..
}

# Função para rodar testes específicos
run_specific() {
    local test=$1
    echo -e "${YELLOW}Executando testes de ${test}...${NC}"
    cd Tests/MyChurch.Cypress
    npm run test:${test}
    cd ../..
}

# Função para abrir Cypress UI
open_ui() {
    echo -e "${YELLOW}Abrindo Cypress UI...${NC}"
    cd Tests/MyChurch.Cypress
    npm run cy:open
    cd ../..
}

# Função para abrir dashboard
open_dashboard() {
    echo -e "${BLUE}?? Abrindo Dashboard...${NC}"
    
    # Detectar sistema operacional e abrir browser
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        xdg-open Tests/MyChurch.Cypress/dashboard.html
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        open Tests/MyChurch.Cypress/dashboard.html
    elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
        start Tests/MyChurch.Cypress/dashboard.html
    else
        echo "Abra manualmente: Tests/MyChurch.Cypress/dashboard.html"
    fi
}

# Menu principal
show_menu() {
    echo ""
    echo "Escolha uma opção:"
    echo "????????????????????????????????????????"
    echo "0) ?? Abrir Dashboard (Visual)"
    echo "1) ??  Rodar todos os testes (217 testes)"
    echo "2) ?? Auth (8 testes)"
    echo "3) ?? Bible (12 testes)"
    echo "4) ?? CashFlow (24 testes)"
    echo "5) ? Church (12 testes)"
    echo "6) ?? Member (18 testes)"
    echo "7) ?? Event (18 testes)"
    echo "8) ?? Journey (15 testes)"
    echo "9) ?? Presentation (15 testes)"
    echo "10) ?? WorshipActivity (22 testes)"
    echo "11) ?? PastorBot (15 testes)"
    echo "12) ?? Subscription (12 testes)"
    echo "13) ?? Donation (15 testes)"
    echo "14) ?? Feed (16 testes)"
    echo "15) ? Reviews (12 testes)"
    echo "????????????????????????????????????????"
    echo "16) ?? Abrir Cypress UI (modo interativo)"
    echo "17) ?? Instalar/atualizar dependências"
    echo "18) ?? Sair"
    echo "????????????????????????????????????????"
    echo ""
    read -p "Opção: " option
    
    case $option in
        0) open_dashboard ;;
        1) check_api && run_all ;;
        2) check_api && run_specific "auth" ;;
        3) check_api && run_specific "bible" ;;
        4) check_api && run_specific "cashflow" ;;
        5) check_api && run_specific "church" ;;
        6) check_api && run_specific "member" ;;
        7) check_api && run_specific "event" ;;
        8) check_api && run_specific "journey" ;;
        9) check_api && run_specific "presentation" ;;
        10) check_api && run_specific "worshipactivity" ;;
        11) check_api && run_specific "pastorbot" ;;
        12) check_api && run_specific "subscription" ;;
        13) check_api && run_specific "donation" ;;
        14) check_api && run_specific "feed" ;;
        15) check_api && run_specific "reviews" ;;
        16) check_api && open_ui ;;
        17) install_deps ;;
        18) echo "Saindo..."; exit 0 ;;
        *) echo -e "${RED}Opção inválida!${NC}" ;;
    esac
}

# Verificar se npm está instalado
if ! command -v npm &> /dev/null; then
    echo -e "${RED}? npm não está instalado!${NC}"
    echo "Por favor, instale Node.js e npm primeiro."
    exit 1
fi

# Show header
echo -e "${BLUE}"
echo "????????????????????????????????????????????"
echo "?   MyChurch E2E Tests - 100% Cobertura   ?"
echo "?   217 Testes | 14 Controllers           ?"
echo "????????????????????????????????????????????"
echo -e "${NC}"

# Loop do menu
while true; do
    show_menu
    echo ""
    read -p "Executar novamente? (s/n): " again
    if [ "$again" != "s" ] && [ "$again" != "S" ]; then
        break
    fi
done

echo -e "${GREEN}Obrigado por usar o MyChurch Test Runner!${NC}"
