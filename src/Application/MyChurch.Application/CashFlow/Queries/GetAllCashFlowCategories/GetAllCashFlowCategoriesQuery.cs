using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Application.CashFlow.Queries.GetAllCashFlowEntries;
using MyChurch.Application.Dtos;
using Mychurch.Common.Utils.Objects;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.CashFlow.Queries.GetAllCashFlowCategories
{
    public class GetAllCashFlowCategoriesQuery : JwtMemberDto, IRequest<PagedResultDto<CashFlowCategoryDto>>
    {

        /// <summary>Número da página (começa em 1)</summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>Tamanho da página</summary>
        public int PageSize { get; set; } = 20;

        public class GetAllCashFlowEntriesQueryHandler : IRequestHandler<GetAllCashFlowCategoriesQuery, PagedResultDto<CashFlowCategoryDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILogger<GetAllCashFlowEntriesQueryHandler> _logger;

            public GetAllCashFlowEntriesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCashFlowEntriesQueryHandler> logger)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
            }

            public async Task<PagedResultDto<CashFlowCategoryDto>> Handle(GetAllCashFlowCategoriesQuery request, CancellationToken cancellationToken)
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
                var query = _unitOfWork.CashFlowCategories.Query()
                    .AsNoTracking()
                    .Include(e => e.Church)
                    .Where(e => e.ChurchId == churchId);

                var totalCount = await query.CountAsync(cancellationToken);

                var items = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(CashFlowCategoryDto.New)
                    .ToList();

                var pagedResult = new PagedResultDto<CashFlowCategoryDto>(items, request.PageNumber, request.PageSize, totalCount);

                return pagedResult;
            }
        }
    }
}
