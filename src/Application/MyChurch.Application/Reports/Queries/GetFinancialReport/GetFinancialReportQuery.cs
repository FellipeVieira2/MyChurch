using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Application.Plans.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using Mychurch.Common.Services;

namespace MyChurch.Application.Reports.Queries.GetFinancialReport
{
    public class GetFinancialReportQuery : JwtMemberDto, IRequest<FinancialReportDto>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? DepartmentId { get; set; }
    }

    public class GetFinancialReportQueryHandler : IRequestHandler<GetFinancialReportQuery, FinancialReportDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPlanLimitService _planLimits;

        public GetFinancialReportQueryHandler(IUnitOfWork uow, IPlanLimitService planLimits)
        {
            _uow = uow;
            _planLimits = planLimits;
        }

        public async Task<FinancialReportDto> Handle(GetFinancialReportQuery request, CancellationToken cancellationToken)
        {
            var member = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Member", "Only admins can access financial reports.");

            var churchId = member.ChurchId;

            if (request.DepartmentId.HasValue)
                await _planLimits.EnsureDepartmentReportsAllowedAsync(churchId, cancellationToken);

            var start = request.StartDate.ToUniversalTime();
            var end = request.EndDate.ToUniversalTime();
            if (end < start) (start, end) = (end, start);

            var donationsQuery = _uow.Donations.Query()
                .AsNoTracking()
                .Include(d => d.Member)
                .Where(d => d.Member != null && d.Member.ChurchId == churchId)
                .Where(d => d.Date >= start && d.Date <= end);

            if (request.DepartmentId.HasValue)
                donationsQuery = donationsQuery.Where(d => d.DepartmentId == request.DepartmentId.Value);

            var totalDonations = await donationsQuery.SumAsync(d => (decimal?)d.Amount, cancellationToken) ?? 0m;

            var cashFlowQuery = _uow.CashFlowEntries.Query()
                .AsNoTracking()
                .Where(e => e.ChurchId == churchId)
                .Where(e => e.Date >= start && e.Date <= end);

            if (request.DepartmentId.HasValue)
                cashFlowQuery = cashFlowQuery.Where(e => e.DepartmentId == request.DepartmentId.Value);

            var totalIncome = await cashFlowQuery
                .Where(e => e.Type == CashFlowType.Income)
                .SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0m;

            var totalExpenses = await cashFlowQuery
                .Where(e => e.Type == CashFlowType.Expense)
                .SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0m;

            return new FinancialReportDto
            {
                StartDate = start,
                EndDate = end,
                TotalDonations = totalDonations,
                TotalTithes = 0m,
                TotalOfferings = 0m,
                OtherIncome = 0m,
                TotalIncome = totalIncome + totalDonations,
                TotalExpenses = totalExpenses,
                NetBalance = (totalIncome + totalDonations) - totalExpenses
            };
        }
    }
}
