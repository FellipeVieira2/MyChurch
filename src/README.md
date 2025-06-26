# MyChurch

MyChurch é uma plataforma completa para gestão de igrejas, oferecendo recursos para administração de membros, campanhas, eventos, planos de leitura bíblica, doações, grupos pequenos, notificações e muito mais.

## Principais Funcionalidades

- **Gestão de Membros:** Cadastro, atualização, importação e acompanhamento do engajamento dos membros.
- **Campanhas e Doações:** Criação e acompanhamento de campanhas financeiras, controle de doações e transferências.
- **Planos de Leitura Bíblica:** Planos personalizados e públicos, acompanhamento do progresso dos membros, estágios e desafios de leitura.
- **Eventos e Grupos:** Gerenciamento de eventos, grupos pequenos, reuniões, presenças e recursos compartilhados.
- **Notificações e Comunicação:** Envio de notificações, integração com e-mail e suporte a webhooks.
- **Painel Administrativo:** Controle de permissões, configurações e relatórios para líderes e administradores.
- **API RESTful:** Backend robusto em .NET 8, com autenticação JWT, documentação via Swagger e integração com SignalR para recursos em tempo real.

## Tecnologias Utilizadas

- **.NET 8 / C# 12**
- **Entity Framework Core** (Migrations, Seed Data)
- **MediatR** (CQRS)
- **FluentValidation**
- **Swagger/OpenAPI**
- **SignalR** (Comunicação em tempo real)
- **Amazon S3** (Armazenamento de arquivos)
- **Serilog** (Logs)
- **JWT Authentication**
- **AWS SES/Postmark** (Envio de e-mails)
- **Docker** (opcional)

## Como Executar

1. Clone o repositório.
2. Configure as variáveis de ambiente (conexão com banco, JWT, AWS, etc).
3. Execute as migrations para criar o banco de dados.
4. Rode o projeto `MyChurch.Api.Web` para iniciar a API.
5. Acesse o Swagger em `/swagger` para explorar os endpoints.

## Estrutura do Projeto

- `MyChurch.Domain`: Entidades e contratos de domínio.
- `MyChurch.Application`: Casos de uso, DTOs, comandos e queries.
- `MyChurch.Infrastructure`: Implementação de repositórios, contexto EF, integrações externas.
- `MyChurch.Api.Web`: API REST, controllers, configuração de middlewares e hubs SignalR.
- `Mychurch.Common`: Utilitários e integrações comuns.

## Contribuição

Contribuições são bem-vindas! Abra uma issue ou envie um pull request.
