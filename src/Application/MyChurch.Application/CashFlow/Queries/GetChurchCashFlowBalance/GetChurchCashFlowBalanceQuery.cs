using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.CashFlow.Queries.GetChurchCashFlowBalance
{
    public class GetChurchCashFlowBalanceQuery : JwtMemberDto, IRequest<decimal>
    {
    }

    public class GetChurchCashFlowBalanceQueryHandler : IRequestHandler<GetChurchCashFlowBalanceQuery, decimal>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChurchCashFlowBalanceQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> Handle(GetChurchCashFlowBalanceQuery request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Este membro não existe.");

            if (!UserRoleAccess.CanViewFinancialModule(member.Role))
                ValidationException.ThrowException("Permissão", "Este perfil não possui acesso ao módulo financeiro.");

            int churchId = member.ChurchId;

            // Calcula o saldo diretamente no banco
            var incomes = await _unitOfWork.CashFlowEntries.Query()
                .Where(e => e.ChurchId == churchId && e.Type == CashFlowType.Income)
                .SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0m;

            var expenses = await _unitOfWork.CashFlowEntries.Query()
                .Where(e => e.ChurchId == churchId && e.Type == CashFlowType.Expense)
                .SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0m;

            return incomes - expenses;
        }
    }
}
