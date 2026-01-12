using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Queries.GetChurchBankingInfos
{
    public class GetChurchBankingInfosQuery : JwtMemberDto, IRequest<List<BankingInfoDto>>
    {
    }

    public class GetChurchBankingInfosQueryHandler : IRequestHandler<GetChurchBankingInfosQuery, List<BankingInfoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChurchBankingInfosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BankingInfoDto>> Handle(GetChurchBankingInfosQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Permissão", "Apenas administradores podem visualizar contas bancárias.");

            var items = await _unitOfWork.BankingInfos.Query()
                .Where(b => b.ChurchId == member.ChurchId)
                .OrderByDescending(b => b.Created)
                .ToListAsync(cancellationToken);

            return items.Select(BankingInfoDto.New).ToList();
        }
    }
}
