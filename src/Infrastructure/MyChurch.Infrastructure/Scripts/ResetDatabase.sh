# ?? SCRIPT DE RESET COMPLETO DO BANCO DE DADOS

## ?? ATENÇÃO: Isso vai DELETAR TODOS OS DADOS!
## Use apenas em DESENVOLVIMENTO!

## Passo 1: Dropar o banco de dados
dotnet ef database drop --force --project Infrastructure/MyChurch.Infrastructure/MyChurch.Infrastructure.csproj

## Passo 2: Recriar com todas as migrations
dotnet ef database update --project Infrastructure/MyChurch.Infrastructure/MyChurch.Infrastructure.csproj

## Passo 3: Verificar se está tudo OK
dotnet ef migrations list --project Infrastructure/MyChurch.Infrastructure/MyChurch.Infrastructure.csproj

## ? PRONTO! Banco limpo e atualizado
