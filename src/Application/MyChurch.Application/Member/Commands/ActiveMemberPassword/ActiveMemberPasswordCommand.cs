using System.Text.Json.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Member.Commands.ActiveMemberPassword
{
    public class ActiveMemberPasswordCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public string? Hash { get; set; }
        public string Password { get; set; }
    }

    public class ActiveMemberPasswordCommandHandler : IRequestHandler<ActiveMemberPasswordCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActiveMemberPasswordCommandHandler> _logger;
        private readonly IPasswordHasher _passwordHasher;

        public ActiveMemberPasswordCommandHandler(
            IUnitOfWork unitOfWork, 
            ILogger<ActiveMemberPasswordCommandHandler> logger, 
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(ActiveMemberPasswordCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(x => x.PasswordHash == request.Hash, cancellationToken);
            
            if (member is null)
            {
                _logger.LogWarning("Invalid activation token: {Hash}", request.Hash);
                ValidationException.ThrowException("Password", "Token de ativação inválido ou expirado.");
            }

            // Verificar se já tem senha BCrypt (evita reativações)
            if (!string.IsNullOrEmpty(member.PasswordHash) && member.PasswordHash.StartsWith("$2"))
            {
                _logger.LogWarning("Member {MemberId} already has an active password", member.Id);
                ValidationException.ThrowException("Password", "Esta conta já possui uma senha ativa.");
            }

            // 🔐 SEGURANÇA: Gerar hash seguro usando BCrypt
            var hashedPassword = _passwordHasher.HashPassword(request.Password);
            
            // ✅ ATIVAR A CONTA ao definir a senha
            member.PasswordHash = hashedPassword;
            member.IsActive = true;

            _unitOfWork.Members.Update(member);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Password activated and account enabled for member with ID: {MemberId}", member.Id);

            return Unit.Value;
        }
    }
}

