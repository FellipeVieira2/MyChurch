using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Statistics.Queries.GetChurchStatistics
{
    public class GetChurchStatisticsQueryHandler : IRequestHandler<GetChurchStatisticsQuery, ChurchStatisticsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MyChurchDbContext _context;
        private readonly ILogger<GetChurchStatisticsQueryHandler> _logger;

        public GetChurchStatisticsQueryHandler(
            IUnitOfWork unitOfWork,
            MyChurchDbContext context,
            ILogger<GetChurchStatisticsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _logger = logger;
        }

        public async Task<ChurchStatisticsDto> Handle(GetChurchStatisticsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Gerando estatísticas para UserId: {UserId}", request.UserId);

            // Buscar o membro logado para obter o ChurchId
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Membro não encontrado. UserId: {UserId}", request.UserId);
                ValidationException.ThrowException("Member", "Membro não encontrado.");
            }

            int churchId = request.ChurchId ?? member.ChurchId;

            var now = DateTime.UtcNow;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = currentMonthStart.AddMonths(-1);

            // Estatísticas de membros
            var allMembers = await _unitOfWork.Members.Query()
                .Where(m => m.ChurchId == churchId)
                .ToListAsync(cancellationToken);

            var totalMembers = allMembers.Count;
            var activeMembers = allMembers.Count(m => m.IsActive);
            var inactiveMembers = totalMembers - activeMembers;
            var baptizedMembers = allMembers.Count(m => m.IsBaptized);
            var tithers = allMembers.Count(m => m.IsTither);

            var newMembersThisMonth = allMembers.Count(m => 
                m.Created >= currentMonthStart && m.Created < currentMonthStart.AddMonths(1));
            
            var newMembersLastMonth = allMembers.Count(m => 
                m.Created >= lastMonthStart && m.Created < currentMonthStart);

            var memberGrowthPercentage = newMembersLastMonth > 0
                ? ((decimal)(newMembersThisMonth - newMembersLastMonth) / newMembersLastMonth) * 100
                : newMembersThisMonth > 0 ? 100 : 0;

            // Estatísticas financeiras
            var cashFlowEntries = await _context.Set<Domain.Entities.CashFlowEntry>()
                .Where(c => c.ChurchId == churchId)
                .ToListAsync(cancellationToken);

            var revenueThisMonth = cashFlowEntries
                .Where(c => c.Type == Domain.Enum.CashFlowType.Income && 
                           c.Date >= currentMonthStart && 
                           c.Date < currentMonthStart.AddMonths(1))
                .Sum(c => c.Amount);

            var revenueLastMonth = cashFlowEntries
                .Where(c => c.Type == Domain.Enum.CashFlowType.Income && 
                           c.Date >= lastMonthStart && 
                           c.Date < currentMonthStart)
                .Sum(c => c.Amount);

            var expensesThisMonth = cashFlowEntries
                .Where(c => c.Type == Domain.Enum.CashFlowType.Expense && 
                           c.Date >= currentMonthStart && 
                           c.Date < currentMonthStart.AddMonths(1))
                .Sum(c => c.Amount);

            var revenueGrowthPercentage = revenueLastMonth > 0
                ? ((revenueThisMonth - revenueLastMonth) / revenueLastMonth) * 100
                : revenueThisMonth > 0 ? 100 : 0;

            // Estatísticas de presença em cultos
            var worshipServicesIds = await _context.Set<Domain.Entities.WorshipService>()
                .Where(w => w.ChurchId == churchId && w.StartTime >= currentMonthStart)
                .Select(w => w.Id)
                .ToListAsync(cancellationToken);

            var worshipPresences = await _context.Set<Domain.Entities.WorshipPresence>()
                .Where(wp => worshipServicesIds.Contains(wp.WorshipServiceId))
                .CountAsync(cancellationToken);

            var totalWorshipsThisMonth = worshipServicesIds.Count;

            var averageAttendancePercentage = totalWorshipsThisMonth > 0 && activeMembers > 0
                ? ((decimal)worshipPresences / (totalWorshipsThisMonth * activeMembers)) * 100
                : 0;

            // Estatísticas de eventos
            var eventsThisMonth = await _unitOfWork.Events.Query()
                .Where(e => e.ChurchId == churchId && 
                           e.Date >= currentMonthStart && 
                           e.Date < currentMonthStart.AddMonths(1))
                .CountAsync(cancellationToken);

            // Estatísticas de ministérios
            var activeMinistries = await _context.Set<Domain.Entities.Ministry>()
                .Where(m => m.ChurchId == churchId && m.IsActive)
                .CountAsync(cancellationToken);

            // Estatísticas de grupos
            var activeGroups = await _context.Set<Domain.Entities.Group>()
                .Where(g => g.ChurchId == churchId && g.IsActive)
                .CountAsync(cancellationToken);

            // Estatísticas de pedidos de oração (não lidos = ativos)
            var activePrayerRequests = await _unitOfWork.PrayerRequests.Query()
                .Where(pr => pr.Member.ChurchId == churchId && !pr.IsRead)
                .CountAsync(cancellationToken);

            // Engajamento médio
            var averageEngagementScore = activeMembers > 0
                ? allMembers.Where(m => m.IsActive).Average(m => (decimal)m.EngagementScore)
                : 0;

            // Visitantes (simplificado - sem conversão pois não temos os campos)
            var visitorsThisMonth = await _context.Set<Domain.Entities.Visitor>()
                .Where(v => v.LastVisitAt >= currentMonthStart)
                .CountAsync(cancellationToken);

            var statistics = new ChurchStatisticsDto
            {
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                InactiveMembers = inactiveMembers,
                NewMembersThisMonth = newMembersThisMonth,
                NewMembersLastMonth = newMembersLastMonth,
                MemberGrowthPercentage = Math.Round(memberGrowthPercentage, 2),
                BaptizedMembers = baptizedMembers,
                BaptizedPercentage = totalMembers > 0 ? Math.Round(((decimal)baptizedMembers / totalMembers) * 100, 2) : 0,
                Tithers = tithers,
                TithersPercentage = totalMembers > 0 ? Math.Round(((decimal)tithers / totalMembers) * 100, 2) : 0,
                AverageAttendancePercentage = Math.Round(averageAttendancePercentage, 2),
                TotalAttendancesThisMonth = worshipPresences,
                RevenueThisMonth = revenueThisMonth,
                RevenueLastMonth = revenueLastMonth,
                RevenueGrowthPercentage = Math.Round(revenueGrowthPercentage, 2),
                ExpensesThisMonth = expensesThisMonth,
                BalanceThisMonth = revenueThisMonth - expensesThisMonth,
                EventsThisMonth = eventsThisMonth,
                ActiveMinistries = activeMinistries,
                ActiveGroups = activeGroups,
                ActivePrayerRequests = activePrayerRequests,
                AverageEngagementScore = Math.Round(averageEngagementScore, 2),
                VisitorsThisMonth = visitorsThisMonth,
                VisitorConversionRate = 0 // Não temos dados de conversão na entidade Visitor atual
            };

            _logger.LogInformation(
                "Estatísticas geradas: {TotalMembers} membros, {ActiveMembers} ativos, Receita: {Revenue}",
                statistics.TotalMembers,
                statistics.ActiveMembers,
                statistics.RevenueThisMonth);

            return statistics;
        }
    }
}
