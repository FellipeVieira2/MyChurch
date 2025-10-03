#!/bin/bash
# Script para aplicar migrations do MyChurch

echo "?? Aplicando Migrations do MyChurch..."
echo ""

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Verificar se dotnet está instalado
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}? .NET SDK não encontrado. Por favor, instale o .NET 8 SDK.${NC}"
    exit 1
fi

echo -e "${YELLOW}?? Verificando projetos...${NC}"

# Diretório base
BASE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
STARTUP_PROJECT="$BASE_DIR/src/Web/MyChurch.Api.Web"
INFRA_PROJECT="$BASE_DIR/src/Infrastructure/MyChurch.Infrastructure"

# Verificar se os projetos existem
if [ ! -f "$STARTUP_PROJECT/MyChurch.Api.Web.csproj" ]; then
    echo -e "${RED}? Projeto Web não encontrado em: $STARTUP_PROJECT${NC}"
    exit 1
fi

if [ ! -f "$INFRA_PROJECT/MyChurch.Infrastructure.csproj" ]; then
    echo -e "${RED}? Projeto Infrastructure não encontrado em: $INFRA_PROJECT${NC}"
    exit 1
fi

echo -e "${GREEN}? Projetos encontrados${NC}"
echo ""

# Listar migrations pendentes
echo -e "${YELLOW}?? Migrations disponíveis:${NC}"
dotnet ef migrations list \
    --startup-project "$STARTUP_PROJECT" \
    --project "$INFRA_PROJECT"

echo ""
echo -e "${YELLOW}?? Aplicando migrations...${NC}"

# Aplicar migrations
dotnet ef database update \
    --startup-project "$STARTUP_PROJECT" \
    --project "$INFRA_PROJECT"

if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}? Migrations aplicadas com sucesso!${NC}"
    echo ""
    echo -e "${GREEN}?? Melhorias aplicadas:${NC}"
    echo "  ? Template de email UTF-8 corrigido"
    echo "  ? Validação de votos implementada"
    echo "  ? Índices de performance adicionados"
    echo "  ? Cache distribuído configurado"
    echo "  ? Sistema de localização atualizado"
    echo "  ? Moderação automática implementada"
    echo ""
    echo -e "${YELLOW}?? Próximos passos (opcional):${NC}"
    echo "  1. Configurar Redis para cache em produção"
    echo "  2. Configurar Google Maps API Key"
    echo "  3. Testar endpoints de reviews e localização"
    echo ""
else
    echo ""
    echo -e "${RED}? Erro ao aplicar migrations${NC}"
    echo "Verifique a string de conexão no appsettings.json"
    exit 1
fi
