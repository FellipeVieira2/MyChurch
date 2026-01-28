using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.SetDefaultBankingInfo
{
    public class SetDefaultBankingInfoCommand : JwtMemberDto, IRequest<bool>
    {
        public int? BankingInfoId { get; set; }
    }

    public class SetDefaultBankingInfoCommandHandler : IRequestHandler<SetDefaultBankingInfoCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public SetDefaultBankingInfoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(SetDefaultBankingInfoCommand request, CancellationToken cancellationToken)
        {
            var member = await _uow.Members.Query().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Permissão", "Apenas administradores podem definir a conta principal.");

            var church = await _uow.Churchs.Query().FirstOrDefaultAsync(c => c.Id == member.ChurchId, cancellationToken);
            if (church == null)
                ValidationException.ThrowException("Church", "Igreja não encontrada.");

            if (request.BankingInfoId.HasValue)
            {
                var exists = await _uow.BankingInfos.Query()
                    .AnyAsync(b => b.Id == request.BankingInfoId.Value && b.ChurchId == church.Id, cancellationToken);

                if (!exists)
                    ValidationException.ThrowException("BankingInfo", "Conta bancária não encontrada para esta igreja.");
            }

            church.DefaultBankingInfoId = request.BankingInfoId;
            church.Updated = DateTime.UtcNow;

            _uow.Churchs.Update(church);
            await _uow.CommitAsync();

            return true;
        }
    }
}
