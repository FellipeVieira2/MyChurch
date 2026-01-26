using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Departments.Services;
using MyChurch.Application.Dtos;
using MyChurch.Application.Plans.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Reports.Queries.GetDepartmentFinancialReport
{
    public class GetDepartmentFinancialReportQuery : JwtMemberDto, IRequest<DepartmentFinancialReportDto>
    {
        public int? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeGeneral { get; set; } = true;
    }

    public class DepartmentFinancialReportItemDto
    {
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = "Geral";
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance => TotalIncome - TotalExpense;
    }

    public class DepartmentFinancialReportDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<DepartmentFinancialReportItemDto> Items { get; set; } = new();
    }

    public class GetDepartmentFinancialReportQueryHandler : IRequestHandler<GetDepartmentFinancialReportQuery, DepartmentFinancialReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentAccessService _departmentAccess;
        private readonly IPlanLimitService _planLimits;

        public GetDepartmentFinancialReportQueryHandler(IUnitOfWork unitOfWork, IDepartmentAccessService departmentAccess, IPlanLimitService planLimits)
        {
            _unitOfWork = unitOfWork;
            _departmentAccess = departmentAccess;
            _planLimits = planLimits;
        }

        public async Task<DepartmentFinancialReportDto> Handle(GetDepartmentFinancialReportQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            await _planLimits.EnsureDepartmentReportsAllowedAsync(member.ChurchId, cancellationToken);

            var query = _unitOfWork.CashFlowEntries.Query()
                .AsNoTracking()
                .Include(e => e.Department)
                .Where(e => e.ChurchId == member.ChurchId);

            // período
            if (request.StartDate.HasValue)
                query = query.Where(e => e.Date.Date >= request.StartDate.Value.Date);
            if (request.EndDate.HasValue)
                query = query.Where(e => e.Date.Date <= request.EndDate.Value.Date);

            // permissão por dept
            var isAdmin = member.Role == UserRole.Admin;

            if (!isAdmin)
            {
                var allowedDepartments = await _departmentAccess.GetAccessibleDepartmentIdsAsync(member.Id, cancellationToken);

                query = query.Where(e =>
                    (request.IncludeGeneral && e.DepartmentId == null) ||
                    (e.DepartmentId.HasValue && allowedDepartments.Contains(e.DepartmentId.Value)));

                if (request.DepartmentId.HasValue)
                {
                    if (!allowedDepartments.Contains(request.DepartmentId.Value))
                        ValidationException.ThrowException("Department", "Sem permissão para visualizar este departamento.");

                    query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);
                }
            }
            else
            {
                if (request.DepartmentId.HasValue)
                    query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);
                else if (!request.IncludeGeneral)
                    query = query.Where(e => e.DepartmentId != null);
            }

            var grouped = await query
                .GroupBy(e => new { e.DepartmentId, DepartmentName = e.Department != null ? e.Department.Name : "Geral" })
                .Select(g => new DepartmentFinancialReportItemDto
                {
                    DepartmentId = g.Key.DepartmentId,
                    DepartmentName = g.Key.DepartmentName,
                    TotalIncome = g.Where(x => x.Type == CashFlowType.Income).Sum(x => (decimal?)x.Amount) ?? 0m,
                    TotalExpense = g.Where(x => x.Type == CashFlowType.Expense).Sum(x => (decimal?)x.Amount) ?? 0m
                })
                .OrderBy(x => x.DepartmentName)
                .ToListAsync(cancellationToken);

            return new DepartmentFinancialReportDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Items = grouped
            };
        }
    }
}
