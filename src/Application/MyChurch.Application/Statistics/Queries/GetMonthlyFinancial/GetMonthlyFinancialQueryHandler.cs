using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure;
using System.Globalization;

namespace MyChurch.Application.Statistics.Queries.GetMonthlyFinancial
{
    public class GetMonthlyFinancialQueryHandler : IRequestHandler<GetMonthlyFinancialQuery, List<MonthlyFinancialDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MyChurchDbContext _context;

        public GetMonthlyFinancialQueryHandler(IUnitOfWork unitOfWork, MyChurchDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<List<MonthlyFinancialDto>> Handle(GetMonthlyFinancialQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Membro não encontrado.");

            int churchId = request.ChurchId ?? member.ChurchId;

            var cashFlowEntries = await _context.Set<Domain.Entities.CashFlowEntry>()
                .Where(c => c.ChurchId == churchId)
                .Select(c => new { c.Date, c.Amount, c.Type })
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var result = new List<MonthlyFinancialDto>();
            var culture = new CultureInfo("pt-BR");

            for (int i = request.Months - 1; i >= 0; i--)
            {
                var targetMonth = now.AddMonths(-i);
                var monthStart = new DateTime(targetMonth.Year, targetMonth.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                var revenue = cashFlowEntries
                    .Where(c => c.Type == Domain.Enum.CashFlowType.Income &&
                               c.Date >= monthStart && 
                               c.Date < monthEnd)
                    .Sum(c => c.Amount);

                var expenses = cashFlowEntries
                    .Where(c => c.Type == Domain.Enum.CashFlowType.Expense &&
                               c.Date >= monthStart && 
                               c.Date < monthEnd)
                    .Sum(c => c.Amount);

                result.Add(new MonthlyFinancialDto
                {
                    Year = targetMonth.Year,
                    Month = targetMonth.Month,
                    MonthName = culture.DateTimeFormat.GetMonthName(targetMonth.Month),
                    Revenue = revenue,
                    Expenses = expenses,
                    Balance = revenue - expenses
                });
            }

            return result;
        }
    }
}
