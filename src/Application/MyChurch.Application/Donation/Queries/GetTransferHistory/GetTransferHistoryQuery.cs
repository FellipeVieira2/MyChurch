using MediatR;
using Microsoft.EntityFrameworkCore;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Donation.Queries.GetTransferHistory
{
    public class GetTransferHistoryQuery : JwtMemberDto, IRequest<PagedResultDto<TransferHistoryDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string? Status { get; set; }
    }

    public class GetTransferHistoryQueryHandler : IRequestHandler<GetTransferHistoryQuery, PagedResultDto<TransferHistoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTransferHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<TransferHistoryDto>> Handle(GetTransferHistoryQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (!UserRoleAccess.CanViewFinancialModule(member.Role))
                ValidationException.ThrowException("Permissão", "Apenas perfis com acesso financeiro podem ver o histórico de retiradas.");
                ValidationException.ThrowException("Permissão", "Apenas administradores podem ver o histórico de retiradas.");

            var query = _unitOfWork.TransferHistories.Query()
                .AsNoTracking()
                .Where(t => t.ChurchId == member.ChurchId);

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(t => t.Status == request.Status);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(t => t.RequestedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<TransferHistoryDto>(
                items: items.Select(TransferHistoryDto.New).ToList(),
                pageNumber: request.Page,
                pageSize: request.PageSize,
                totalCount: total);
        }
    }
}
