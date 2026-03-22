using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Common.Models;
using MyChurch.Application.Departments.Services;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.CashFlow.Queries.GetAllCashFlowEntries
{
    public class GetAllCashFlowEntriesQuery : JwtMemberDto, IRequest<CashFlowEntryPagedResult>
    {
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public CashFlowType? Type { get; set; }
        public int? CategoryId { get; set; }
        public int? DepartmentId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "Date";
        public string SortDirection { get; set; } = "desc";
    }

    public class CashFlowEntryPagedResult
    {
        public PaginatedList<CashFlowEntryDto> Entries { get; set; } = null!;
        public decimal Balance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
    }

    public class GetAllCashFlowEntriesQueryHandler : IRequestHandler<GetAllCashFlowEntriesQuery, CashFlowEntryPagedResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllCashFlowEntriesQueryHandler> _logger;
        private readonly IDepartmentAccessService _departmentAccess;

        public GetAllCashFlowEntriesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCashFlowEntriesQueryHandler> logger, IDepartmentAccessService departmentAccess)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _departmentAccess = departmentAccess;
        }

        public async Task<CashFlowEntryPagedResult> Handle(GetAllCashFlowEntriesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Getting cash flow entries - Page: {PageNumber}, Period: {StartDate} to {EndDate}",
                request.PageNumber,
                request.StartDate,
                request.EndDate);

            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("User {UserId} not found", request.UserId);
                ValidationException.ThrowException("Member", "This Member does not exist.");
            }

            if (!UserRoleAccess.CanViewFinancialModule(member.Role))
                ValidationException.ThrowException("Member", "This profile does not have access to the financial module.");

            int churchId = member.ChurchId;

            var query = _unitOfWork.CashFlowEntries.Query()
                .AsNoTracking()
                .Include(e => e.Member)
                .Include(e => e.Category)
                .Include(e => e.Church)
                .Where(e => e.ChurchId == churchId);

            // Permissões por departamento:
            // - Admin vê tudo (pode filtrar DepartmentId específico)
            // - Demais usuários: apenas DepartmentId que ele participa + lançamentos gerais (DepartmentId null)
            if (!UserRoleAccess.CanManageFinancialModule(member.Role))
            {
                var allowedDepartments = await _departmentAccess.GetAccessibleDepartmentIdsAsync(member.Id, cancellationToken);

                query = query.Where(e => e.DepartmentId == null || (e.DepartmentId.HasValue && allowedDepartments.Contains(e.DepartmentId.Value)));

                if (request.DepartmentId.HasValue)
                {
                    // Se o usuário pediu um dept específico, valida se ele tem acesso
                    if (!allowedDepartments.Contains(request.DepartmentId.Value))
                        ValidationException.ThrowException("Department", "Sem permissão para visualizar este departamento.");

                    query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);
                }
            }
            else
            {
                if (request.DepartmentId.HasValue)
                    query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);
            }

            query = ApplyFilters(query, request);

            var income = await query
                .Where(e => e.Type == CashFlowType.Income)
                .SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0m;

            var expense = await query
                .Where(e => e.Type == CashFlowType.Expense)
                .SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0m;

            var balance = income - expense;

            query = ApplySorting(query, request);

            var totalCount = await query.CountAsync(cancellationToken);

            var entities = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = entities.Select(CashFlowEntryDto.New).ToList();

            var paginatedEntries = new PaginatedList<CashFlowEntryDto>(
                items,
                totalCount,
                request.PageNumber,
                request.PageSize);

            _logger.LogInformation(
                "Retrieved {Count} entries from {TotalCount} total - Balance: {Balance:C}",
                paginatedEntries.Items.Count,
                paginatedEntries.TotalCount,
                balance);

            return new CashFlowEntryPagedResult
            {
                Entries = paginatedEntries,
                Balance = balance,
                TotalIncome = income,
                TotalExpense = expense
            };
        }

        private static IQueryable<Domain.Entities.CashFlowEntry> ApplyFilters(
            IQueryable<Domain.Entities.CashFlowEntry> query,
            GetAllCashFlowEntriesQuery request)
        {
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

            return query;
        }

        private static IQueryable<Domain.Entities.CashFlowEntry> ApplySorting(
            IQueryable<Domain.Entities.CashFlowEntry> query,
            GetAllCashFlowEntriesQuery request)
        {
            var isDescending = request.SortDirection?.ToLower() == "desc";

            return request.SortBy?.ToLower() switch
            {
                "amount" => isDescending ? query.OrderByDescending(e => e.Amount) : query.OrderBy(e => e.Amount),
                "type" => isDescending ? query.OrderByDescending(e => e.Type) : query.OrderBy(e => e.Type),
                _ => isDescending ? query.OrderByDescending(e => e.Date) : query.OrderBy(e => e.Date)
            };
        }
    }
}
