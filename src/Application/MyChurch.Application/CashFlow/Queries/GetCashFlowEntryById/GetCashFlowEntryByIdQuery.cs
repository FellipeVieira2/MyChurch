using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.CashFlow.Queries.GetCashFlowEntryById
{
    public class GetCashFlowEntryByIdQuery : JwtMemberDto, IRequest<CashFlowEntryDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }

    public class GetCashFlowEntryByIdQueryHandler : IRequestHandler<GetCashFlowEntryByIdQuery, CashFlowEntryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCashFlowEntryByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CashFlowEntryDto> Handle(GetCashFlowEntryByIdQuery request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            if (!UserRoleAccess.CanViewFinancialModule(loggedMember.Role))
                ValidationException.ThrowException("Permissão", "Este perfil não possui acesso ao módulo financeiro.");

            // Busca o lançamento e inclui navegações relevantes
            var entry = await _unitOfWork.CashFlowEntries.Query()
                .Include(e => e.Member)
                .Include(e => e.Category)
                .Include(e => e.Church)
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (entry == null)
                ValidationException.ThrowException("CashFlowEntry", "Lançamento de fluxo de caixa não encontrado ou não pertence à sua igreja.");

            return CashFlowEntryDto.New(entry);
        }
    }
}
