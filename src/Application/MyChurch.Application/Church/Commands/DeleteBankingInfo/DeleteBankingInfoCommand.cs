using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.DeleteBankingInfo
{
    public class DeleteBankingInfoCommand : JwtMemberDto, IRequest<Unit>
    {
        [JsonIgnore]
        public int BankingInfoId { get; set; }
    }

    public class DeleteBankingInfoCommandHandler : IRequestHandler<DeleteBankingInfoCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBankingInfoCommandHandler> _logger;

        public DeleteBankingInfoCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteBankingInfoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteBankingInfoCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Permissão", "Apenas administradores podem remover contas bancárias.");

            var entity = await _unitOfWork.BankingInfos.Query()
                .FirstOrDefaultAsync(b => b.Id == request.BankingInfoId && b.ChurchId == member.ChurchId, cancellationToken);

            if (entity == null)
                ValidationException.ThrowException("BankingInfo", "Conta bancária não encontrada.");

            _unitOfWork.BankingInfos.Delete(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Conta bancária removida. BankingInfoId: {BankingInfoId}", request.BankingInfoId);

            return Unit.Value;
        }
    }
}
