using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Donation.Queries.GetChurchTransferBalance
{
    public class GetChurchTransferBalanceQuery : JwtMemberDto, IRequest<decimal>
    {
    }

    public class GetChurchTransferBalanceQueryHandler : IRequestHandler<GetChurchTransferBalanceQuery, decimal>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChurchTransferBalanceQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> Handle(GetChurchTransferBalanceQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            if (!UserRoleAccess.CanViewFinancialModule(member.Role))
                throw new UnauthorizedAccessException("Este perfil não possui acesso ao módulo financeiro.");

            var churchId = member.ChurchId;

            // Soma das doações pagas e não repassadas
            var total = await _unitOfWork.Donations.Query()
                .Where(d => d.Member.ChurchId == churchId && d.IsTransferred == false &&
                            d.Payments.Any(p => p.PaymentStatus == "Received"))
                .SumAsync(d => d.Amount, cancellationToken);

            return total;
        }
    }
}