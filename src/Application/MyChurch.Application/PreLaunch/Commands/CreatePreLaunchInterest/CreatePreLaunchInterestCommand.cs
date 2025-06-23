using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure.Utils.SES;
using System.Text.RegularExpressions;

namespace MyChurch.Application.PreLaunch.Commands.CreatePreLaunchInterest
{
    public class CreatePreLaunchInterestCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ChurchName { get; set; }
        public string ChurchRole { get; set; }
        public string Comments { get; set; }
    }

    public class CreatePreLaunchInterestCommandHandler : IRequestHandler<CreatePreLaunchInterestCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreatePreLaunchInterestCommandHandler> _logger;
        private readonly IEmailService _emailService;

        public CreatePreLaunchInterestCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreatePreLaunchInterestCommandHandler> logger,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<int> Handle(CreatePreLaunchInterestCommand request, CancellationToken cancellationToken)
        {
            // Validate input
            ValidateRequest(request);

            // Check if email already exists
            var emailExists = await _unitOfWork.PreLaunchInterests.IsEmailRegisteredAsync(request.Email, cancellationToken);
            if (emailExists)
            {
                ValidationException.ThrowException("Email", "Este email já está registrado na nossa lista de interessados.");
            }

            // Create new pre-launch interest
            var preLaunchInterest = new PreLaunchInterest
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                ChurchName = request.ChurchName,
                ChurchRole = request.ChurchRole,
                Comments = request.Comments,
                RegisterDate = DateTime.UtcNow,
                IsEmailConfirmed = false,
                ConfirmationToken = Guid.NewGuid().ToString("N")
            };

            _unitOfWork.PreLaunchInterests.Create(preLaunchInterest);
            await _unitOfWork.CommitAsync();

            // Send confirmation email
            //await SendConfirmationEmailAsync(preLaunchInterest, cancellationToken);

            _logger.LogInformation("New pre-launch interest registered: {Email}", request.Email);

            return preLaunchInterest.Id;
        }

        private void ValidateRequest(CreatePreLaunchInterestCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                ValidationException.ThrowException("Name", "O nome é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                ValidationException.ThrowException("Email", "O email é obrigatório.");
            }

            // Basic email validation using regex
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(request.Email, emailPattern))
            {
                ValidationException.ThrowException("Email", "Por favor, forneça um email válido.");
            }
        }

        private async Task SendConfirmationEmailAsync(Domain.Entities.PreLaunchInterest interest, CancellationToken cancellationToken)
        {
            try
            {
                // Criar o HTML do email diretamente, sem depender do arquivo de template
                string htmlContent = @"<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <title>Confirmação de Interesse MyChurch</title>
</head>
<body style=""font-family: Arial, sans-serif; background: #f2f2f2; padding: 30px;"">
    <div style=""max-width: 600px; margin: auto; background: #fff; padding: 40px; border-radius: 8px;"">
        <div style=""text-align: center; margin-bottom: 30px;"">
            <img src=""https://mychurchbucketnet.s3.us-east-2.amazonaws.com/logo.png"" alt=""MyChurch Logo"" style=""max-width: 150px;"">
        </div>
        <h2 style=""color: #4a4a4a; text-align: center;"">Confirmação de Interesse</h2>
        <p>Olá {0},</p>
        <p>Obrigado por demonstrar interesse em conhecer o MyChurch! Estamos muito animados com o lançamento da nossa plataforma que irá ajudar igrejas a gerenciar suas atividades de forma mais eficiente.</p>
        <p>Para confirmar seu interesse e garantir que você receba todas as novidades sobre o lançamento, por favor, clique no botão abaixo:</p>
        <p style=""text-align: center;"">
            <a href=""{1}"" style=""background: #007BFF; color: white; padding: 12px 25px; border-radius: 5px; text-decoration: none; display: inline-block; font-weight: bold;"">Confirmar meu interesse</a>
        </p>
        <p>Você será um dos primeiros a saber quando o MyChurch estiver disponível e terá acesso a promoções exclusivas para o pré-lançamento.</p>
        <p>Se você não solicitou este cadastro, pode simplesmente ignorar este e-mail.</p>
        <div style=""margin-top: 40px; padding-top: 20px; border-top: 1px solid #eaeaea; text-align: center; color: #888; font-size: 12px;"">
            <p>© 2025 MyChurch. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";

                // Build confirmation link
                var confirmationLink = $"https://www.mychurchlab.net/pre-lancamento/confirmar?token={interest.ConfirmationToken}";

                // Formatar o HTML com os dados do usuário
                htmlContent = string.Format(htmlContent, interest.Name, confirmationLink);

                await _emailService.EnviarEmailAsync(
                    destinatario: interest.Email,
                    assunto: "Confirme seu interesse no MyChurch",
                    corpoHtml: htmlContent
                );

                _logger.LogInformation("Confirmation email sent to: {Email}", interest.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email to: {Email}", interest.Email);
                // We'll continue even if email fails - the registration is still valid
            }
        }
    }
}