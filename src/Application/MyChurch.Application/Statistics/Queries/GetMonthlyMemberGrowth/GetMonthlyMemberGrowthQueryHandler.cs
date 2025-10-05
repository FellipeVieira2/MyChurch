using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Globalization;

namespace MyChurch.Application.Statistics.Queries.GetMonthlyMemberGrowth
{
    public class GetMonthlyMemberGrowthQueryHandler : IRequestHandler<GetMonthlyMemberGrowthQuery, List<MonthlyMemberGrowthDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMonthlyMemberGrowthQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MonthlyMemberGrowthDto>> Handle(GetMonthlyMemberGrowthQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Membro não encontrado.");

            int churchId = request.ChurchId ?? member.ChurchId;

            var members = await _unitOfWork.Members.Query()
                .Where(m => m.ChurchId == churchId)
                .Select(m => new { m.Created, m.IsActive })
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var result = new List<MonthlyMemberGrowthDto>();
            var culture = new CultureInfo("pt-BR");

            for (int i = request.Months - 1; i >= 0; i--)
            {
                var targetMonth = now.AddMonths(-i);
                var monthStart = new DateTime(targetMonth.Year, targetMonth.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                var newMembers = members.Count(m => 
                    m.Created >= monthStart && m.Created < monthEnd);

                var totalMembers = members.Count(m => m.Created < monthEnd);
                
                var inactiveMembers = members.Count(m => 
                    m.Created < monthEnd && !m.IsActive);

                result.Add(new MonthlyMemberGrowthDto
                {
                    Year = targetMonth.Year,
                    Month = targetMonth.Month,
                    MonthName = culture.DateTimeFormat.GetMonthName(targetMonth.Month),
                    NewMembers = newMembers,
                    TotalMembers = totalMembers,
                    InactiveMembers = inactiveMembers
                });
            }

            return result;
        }
    }
}
