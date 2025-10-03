using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Common.Models;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.CashFlow.Queries.GetAllCashFlowCategories
{
    public class GetAllCashFlowCategoriesQuery : JwtMemberDto, IRequest<PaginatedList<CashFlowCategoryDto>>
    {
        /// <summary>Número da página (começa em 1)</summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>Tamanho da página (máximo 100)</summary>
        public int PageSize { get; set; } = 20;

        /// <summary>Campo para ordenação (Name, Description)</summary>
        public string? SortBy { get; set; } = "Name";

        /// <summary>Direção da ordenação (asc ou desc)</summary>
        public string SortDirection { get; set; } = "asc";

        /// <summary>Filtro por nome (contém)</summary>
        public string? Name { get; set; }
    }

    public class GetAllCashFlowCategoriesQueryHandler : IRequestHandler<GetAllCashFlowCategoriesQuery, PaginatedList<CashFlowCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllCashFlowCategoriesQueryHandler> _logger;

        public GetAllCashFlowCategoriesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCashFlowCategoriesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaginatedList<CashFlowCategoryDto>> Handle(GetAllCashFlowCategoriesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Getting cash flow categories - Page: {PageNumber}, PageSize: {PageSize}",
                request.PageNumber,
                request.PageSize);

            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("User {UserId} not found", request.UserId);
                ValidationException.ThrowException("Member", "This Member does not exist.");
            }

            int churchId = member.ChurchId;

            // Query base com filtros
            var query = _unitOfWork.CashFlowCategories.Query()
                .AsNoTracking()
                .Include(e => e.Church)
                .Where(e => e.ChurchId == churchId);

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{request.Name}%"));
            }

            // Aplica ordenação
            query = ApplySorting(query, request);

            // Conta total
            var totalCount = await query.CountAsync(cancellationToken);

            var entities = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = entities.Select(CashFlowCategoryDto.New).ToList();

            var result = new PaginatedList<CashFlowCategoryDto>(
                items,
                totalCount,
                request.PageNumber,
                request.PageSize);

            _logger.LogInformation(
                "Retrieved {Count} categories from {TotalCount} total",
                result.Items.Count,
                result.TotalCount);

            return result;
        }

        private static IQueryable<Domain.Entities.CashFlowCategory> ApplySorting(
            IQueryable<Domain.Entities.CashFlowCategory> query,
            GetAllCashFlowCategoriesQuery request)
        {
            var isDescending = request.SortDirection?.ToLower() == "desc";
            
            return request.SortBy?.ToLower() switch
            {
                "description" => isDescending 
                    ? query.OrderByDescending(c => c.Description) 
                    : query.OrderBy(c => c.Description),
                _ => isDescending 
                    ? query.OrderByDescending(c => c.Name) 
                    : query.OrderBy(c => c.Name)
            };
        }
    }
}
