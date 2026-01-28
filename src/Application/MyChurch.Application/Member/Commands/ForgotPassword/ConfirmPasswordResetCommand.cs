using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;

namespace MyChurch.Application.Member.Commands.ForgotPassword
{
    public class ConfirmPasswordResetCommand : IRequest<Unit>
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ConfirmPasswordResetCommandHandler : IRequestHandler<ConfirmPasswordResetCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<ConfirmPasswordResetCommandHandler> _logger;

        public ConfirmPasswordResetCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher, ILogger<ConfirmPasswordResetCommandHandler> logger)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Unit> Handle(ConfirmPasswordResetCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                ValidationException.ThrowException("Token", "Token é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
                ValidationException.ThrowException("Password", "A nova senha deve ter no mínimo 6 caracteres.");

            var now = DateTime.UtcNow;

            var member = await _uow.Members.Query().FirstOrDefaultAsync(x =>
                x.PasswordResetToken == request.Token &&
                x.PasswordResetTokenExpiresAt != null &&
                x.PasswordResetTokenExpiresAt > now, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Token", "Token inválido ou expirado.");

            member.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            member.PasswordResetToken = null;
            member.PasswordResetTokenExpiresAt = null;
            member.IsActive = true;

            _uow.Members.Update(member);
            await _uow.CommitAsync();

            _logger.LogInformation("Password reset confirmed for member {MemberId}", member.Id);
            return Unit.Value;
        }
    }
}
