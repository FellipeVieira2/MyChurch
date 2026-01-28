using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;

namespace MyChurch.Application.Member.Commands.ChangeMyPassword
{
    public class ChangeMyPasswordCommand : JwtMemberDto, IRequest<Unit>
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangeMyPasswordCommandHandler : IRequestHandler<ChangeMyPasswordCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<ChangeMyPasswordCommandHandler> _logger;

        public ChangeMyPasswordCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher, ILogger<ChangeMyPasswordCommandHandler> logger)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Unit> Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
                ValidationException.ThrowException("Password", "A nova senha deve ter no mínimo 6 caracteres.");

            var member = await _uow.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (string.IsNullOrWhiteSpace(member.PasswordHash) || !_passwordHasher.VerifyPassword(request.CurrentPassword, member.PasswordHash))
                ValidationException.ThrowException("Password", "Senha atual inválida.");

            member.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            _uow.Members.Update(member);
            await _uow.CommitAsync();

            _logger.LogInformation("Member {MemberId} changed own password", member.Id);
            return Unit.Value;
        }
    }
}
