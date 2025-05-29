using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.CashFlow.Queries.GetAllCashFlowEntries
{
    public class GetAllCashFlowEntriesQuery : JwtMemberDto, IRequest<CashFlowEntryPagedResultDto>
    {
        /// <summary>Filtro por valor exato</summary>
        public decimal? Amount { get; set; }

        /// <summary>Filtro por data exata</summary>
        public DateTime? Date { get; set; }

        /// <summary>Filtro por data inicial (>=)</summary>
        public DateTime? StartDate { get; set; }

        /// <summary>Filtro por data final (<=)</summary>
        public DateTime? EndDate { get; set; }

        /// <summary>Filtro por tipo (Entrada/Saída)</summary>
        public CashFlowType? Type { get; set; }

        /// <summary>Filtro por categoria</summary>
        public int? CategoryId { get; set; }

        /// <summary>Número da página (começa em 1)</summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>Tamanho da página</summary>
        public int PageSize { get; set; } = 20;
    }
    public class CashFlowEntryPagedResultDto
    {
        public PagedResultDto<CashFlowEntryDto> Result { get; set; } = null!;
        public decimal Balance { get; set; }
    }

    public class GetAllCashFlowEntriesQueryHandler : IRequestHandler<GetAllCashFlowEntriesQuery, CashFlowEntryPagedResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllCashFlowEntriesQueryHandler> _logger;

        public GetAllCashFlowEntriesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCashFlowEntriesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CashFlowEntryPagedResultDto> Handle(GetAllCashFlowEntriesQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Usuário não encontrado.");
                ValidationException.ThrowException("Member", "This Member does not exist.");
            }

            int churchId = member.ChurchId;

            // Query base com filtros
            var query = _unitOfWork.CashFlowEntries.Query()
                .AsNoTracking()
                .Include(e => e.Member)
                .Include(e => e.Category)
                .Include(e => e.Church)
                .Where(e => e.ChurchId == churchId);

            if (request.Amount.HasValue)
                query = query.Where(e => e.Amount == request.Amount.Value);

            if (request.Date.HasValue)
                query = query.Where(e => e.Date.Date == request.Date.Value.Date);

            if (request.StartDate.HasValue)
                query = query.Where(e => e.Date.Date >= request.StartDate.Value.Date);

            if (request.EndDate.HasValue)
                query = query.Where(e => e.Date.Date <= request.EndDate.Value.Date);

            if (request.Type.HasValue)
                query = query.Where(e => e.Type == request.Type.Value);

            if (request.CategoryId.HasValue)
                query = query.Where(e => e.CategoryId == request.CategoryId.Value);

            // Saldo consolidado com os mesmos filtros
            var income = await query.Where(e => e.Type == CashFlowType.Income).SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0m;
            var expense = await query.Where(e => e.Type == CashFlowType.Expense).SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0m;
            var balance = income - expense;

            // Paginação
            var totalCount = await query.CountAsync(cancellationToken);

            var items = query
                .OrderByDescending(e => e.Date)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(CashFlowEntryDto.New)
                .ToList();

            var pagedResult = new PagedResultDto<CashFlowEntryDto>(items, request.PageNumber, request.PageSize, totalCount);

            return new CashFlowEntryPagedResultDto
            {
                Result = pagedResult,
                Balance = balance
            };
        }
    }
}
