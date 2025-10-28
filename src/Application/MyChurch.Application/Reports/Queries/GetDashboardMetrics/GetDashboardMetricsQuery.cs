using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Application.Reports.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Reports.Queries.GetDashboardMetrics
{
    public class GetDashboardMetricsQuery : JwtMemberDto, IRequest<DashboardMetricsDto>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class GetDashboardMetricsQueryHandler : IRequestHandler<GetDashboardMetricsQuery, DashboardMetricsDto>
    {
        private readonly IUnitOfWork _uow;

        public GetDashboardMetricsQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DashboardMetricsDto> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
        {
            var member = await _uow.Members.Query().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null) throw new Exception("Member not found");

            var churchId = member.ChurchId;
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfYear = new DateTime(now.Year, 1, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);

            // ?? MEMBROS
            var allMembers = await _uow.Members.Query().Where(m => m.ChurchId == churchId).ToListAsync(cancellationToken);
            var totalMembers = allMembers.Count;
            var activeMembers = allMembers.Count(m => m.IsActive);
            var newMembersThisMonth = allMembers.Count(m => m.Created >= startOfMonth);
            var newMembersThisYear = allMembers.Count(m => m.Created >= startOfYear);
            var newMembersLastMonth = allMembers.Count(m => m.Created >= startOfLastMonth && m.Created < startOfMonth);
            var memberGrowth = newMembersLastMonth > 0 ? ((newMembersThisMonth - newMembersLastMonth) / (decimal)newMembersLastMonth) * 100 : 0;

            // ?? FINANÇAS
            var donations = await _uow.Donations.Query()
                .Where(d => d.Member.ChurchId == churchId)
                .ToListAsync(cancellationToken);
            var donationsThisMonth = donations.Where(d => d.Date >= startOfMonth).ToList();
            var donationsThisYear = donations.Where(d => d.Date >= startOfYear).ToList();
            var donationsLastMonth = donations.Where(d => d.Date >= startOfLastMonth && d.Date < startOfMonth).ToList();

            var totalDonationsThisMonth = donationsThisMonth.Sum(d => d.Amount);
            var totalDonationsThisYear = donationsThisYear.Sum(d => d.Amount);
            var totalDonationsLastMonth = donationsLastMonth.Sum(d => d.Amount);
            var donationGrowth = totalDonationsLastMonth > 0 ? ((totalDonationsThisMonth - totalDonationsLastMonth) / totalDonationsLastMonth) * 100 : 0;
            var avgDonation = donations.Any() ? donations.Average(d => d.Amount) : 0;
            var totalDonorsThisMonth = donationsThisMonth.Select(d => d.MemberId).Distinct().Count();

            // ?? EVENTOS
            var events = await _uow.Events.Query()
                .Where(e => e.ChurchId == churchId)
                .ToListAsync(cancellationToken);
            var eventsThisMonth = events.Count(e => e.Date >= startOfMonth);
            var upcomingEvents = events.Count(e => e.Date > now);

            // ?? CULTOS
            var worshipServices = await _uow.WorshipServices.Query()
                .Include(w => w.Presences)
                .Include(w => w.Event)
                .Where(w => w.Event.ChurchId == churchId)
                .ToListAsync(cancellationToken);
            var worshipThisMonth = worshipServices.Where(w => w.StartTime >= startOfMonth).ToList();
            var avgWorshipAttendance = worshipThisMonth.Any() ? worshipThisMonth.Average(w => w.Presences.Count) : 0;

            // ?? VISITANTES
            var visitors = await _uow.Visitors.Query().ToListAsync(cancellationToken);
            var newVisitorsThisMonth = visitors.Count(v => v.CreatedAt >= startOfMonth);
            var visitorsConverted = visitors.Count(v => v.Status == MyChurch.Domain.Enum.VisitorStatus.Integrated);
            var conversionRate = visitors.Count > 0 ? (visitorsConverted / (decimal)visitors.Count) * 100 : 0;

            // ?? ENGAJAMENTO
            var activeBibleReading = await _uow.MemberBibleReadingAssignments.Query()
                .Where(a => allMembers.Select(m => m.Id).Contains(a.MemberId))
                .Select(a => a.MemberId)
                .Distinct()
                .CountAsync(cancellationToken);

            var prayerRequests = await _uow.PrayerRequests.Query()
                .Where(p => p.Member.ChurchId == churchId)
                .CountAsync(cancellationToken);

            var groupMembers = await _uow.GroupMembers.Query()
                .Include(gm => gm.Group)
                .Where(gm => gm.Group.ChurchId == churchId)
                .Select(gm => gm.MemberId)
                .Distinct()
                .CountAsync(cancellationToken);

            // ?? TOP DONORS
            var topDonors = donations
                .GroupBy(d => new { d.MemberId, d.Member.Name })
                .Select(g => new TopDonorDto
                {
                    MemberId = g.Key.MemberId ?? 0,
                    MemberName = g.Key.Name ?? "Anônimo",
                    TotalDonated = g.Sum(d => d.Amount)
                })
                .OrderByDescending(t => t.TotalDonated)
                .Take(5)
                .ToList();

            // ?? MOST ENGAGED
            var mostEngaged = allMembers
                .OrderByDescending(m => m.EngagementScore)
                .Take(5)
                .Select(m => new MostEngagedMemberDto
                {
                    MemberId = m.Id,
                    MemberName = m.Name,
                    EngagementScore = m.EngagementScore
                })
                .ToList();

            return new DashboardMetricsDto
            {
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                InactiveMembers = totalMembers - activeMembers,
                NewMembersThisMonth = newMembersThisMonth,
                NewMembersThisYear = newMembersThisYear,
                MemberGrowthPercentage = memberGrowth,

                TotalDonationsThisMonth = totalDonationsThisMonth,
                TotalDonationsThisYear = totalDonationsThisYear,
                AverageDonationAmount = avgDonation,
                TotalDonorsThisMonth = totalDonorsThisMonth,
                DonationGrowthPercentage = donationGrowth,

                TotalEventsThisMonth = eventsThisMonth,
                UpcomingEvents = upcomingEvents,

                TotalWorshipServicesThisMonth = worshipThisMonth.Count,
                AverageWorshipAttendance = (decimal)avgWorshipAttendance,

                TotalVisitors = visitors.Count,
                NewVisitorsThisMonth = newVisitorsThisMonth,
                VisitorsConverted = visitorsConverted,
                VisitorConversionRate = conversionRate,

                ActiveBibleReadingMembers = activeBibleReading,
                TotalPrayerRequests = prayerRequests,
                ActiveGroupMembers = groupMembers,

                TopDonors = topDonors,
                MostEngagedMembers = mostEngaged
            };
        }
    }
}
