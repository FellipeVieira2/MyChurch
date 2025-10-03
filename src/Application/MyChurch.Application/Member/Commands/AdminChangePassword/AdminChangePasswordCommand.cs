using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;

namespace MyChurch.Application.Member.Commands.AdminChangePassword
{
    public class AdminChangePasswordCommand : JwtMemberDto, IRequest<Unit>
    {
        [JsonIgnore]
        public int MemberId { get; set; }
        public string NewPassword { get; set; }
    }

    public class AdminChangePasswordCommandHandler : IRequestHandler<AdminChangePasswordCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AdminChangePasswordCommandHandler> _logger;
        private readonly IPasswordHasher _passwordHasher;

        public AdminChangePasswordCommandHandler(
            IUnitOfWork unitOfWork, 
            ILogger<AdminChangePasswordCommandHandler> logger,
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(AdminChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // Verificar se o usuário autenticado é um admin
            var adminMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (adminMember == null || adminMember.Role != UserRole.Admin)
            {
                _logger.LogWarning("Usuário não autorizado tentou alterar senha: {UserId}", request.UserId);
                ValidationException.ThrowException("Authorization", "Apenas administradores podem alterar senhas de membros.");
            }

            // Buscar o membro cuja senha será alterada
            var targetMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(x => x.Id == request.MemberId, cancellationToken);

            if (targetMember == null)
            {
                _logger.LogWarning("Membro não encontrado: {MemberId}", request.MemberId);
                ValidationException.ThrowException("Member", "Membro não encontrado.");
            }

            // Verificar se o membro pertence à mesma igreja do admin
            if (targetMember.ChurchId != adminMember.ChurchId)
            {
                _logger.LogWarning("Tentativa de alterar senha de membro de outra igreja: {AdminId}, {MemberId}", request.UserId, request.MemberId);
                ValidationException.ThrowException("Authorization", "Você só pode alterar senhas de membros da sua igreja.");
            }

            // ?? SEGURANÇA: Gerar hash seguro usando BCrypt
            targetMember.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);

            _unitOfWork.Members.Update(targetMember);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Senha alterada para o membro com ID: {MemberId} pelo admin {AdminId}", targetMember.Id, request.UserId);

            return Unit.Value;
        }
    }
}