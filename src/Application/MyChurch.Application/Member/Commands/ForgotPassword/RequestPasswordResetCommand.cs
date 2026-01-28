using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Security.Cryptography;

namespace MyChurch.Application.Member.Commands.ForgotPassword
{
    public class RequestPasswordResetCommand : IRequest<Unit>
    {
        public string Email { get; set; } = string.Empty;
        public string? ResetBaseUrl { get; set; } // ex.: https://app.mychurch.com/reset-password
    }

    public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailService _emailService;
        private readonly ILogger<RequestPasswordResetCommandHandler> _logger;

        public RequestPasswordResetCommandHandler(IUnitOfWork uow, IEmailService emailService, ILogger<RequestPasswordResetCommandHandler> logger)
        {
            _uow = uow;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Unit> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                ValidationException.ThrowException("Email", "Email é obrigatório.");

            var email = request.Email.Trim().ToLowerInvariant();

            // Não vaza existência de conta
            var member = await _uow.Members.Query().FirstOrDefaultAsync(x => x.Email != null && x.Email.ToLower() == email, cancellationToken);
            if (member == null)
            {
                _logger.LogInformation("Password reset requested for non-existing email {Email}", email);
                return Unit.Value;
            }

            var token = CreateToken();
            member.PasswordResetToken = token;
            member.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(2);
            _uow.Members.Update(member);
            await _uow.CommitAsync();

            var baseUrl = string.IsNullOrWhiteSpace(request.ResetBaseUrl) ? null : request.ResetBaseUrl.Trim();
            var resetLink = baseUrl == null ? token : $"{baseUrl}{(baseUrl.Contains('?') ? '&' : '?')}token={Uri.EscapeDataString(token)}";

            var subject = "Recuperação de senha";
            var html = $"<p>Olá {System.Net.WebUtility.HtmlEncode(member.Name)}.</p>" +
                       $"<p>Para redefinir sua senha, use o código abaixo (válido por 2 horas):</p>" +
                       $"<p style=\"font-size:18px\"><b>{System.Net.WebUtility.HtmlEncode(token)}</b></p>" +
                       $"<p>Ou clique: <a href=\"{System.Net.WebUtility.HtmlEncode(resetLink)}\">Redefinir senha</a></p>" +
                       $"<p>Se você não solicitou, ignore este email.</p>";

            await _emailService.SendEmailAsync(member.Email!, subject, html);

            _logger.LogInformation("Password reset token generated for member {MemberId}", member.Id);
            return Unit.Value;
        }

        private static string CreateToken()
        {
            // Base64url (sem caracteres problemáticos)
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
