using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Church.Queries.GetChurch
{
    public class GetChurchDashboardQuery : JwtMemberDto, IRequest<ChurchDashboardDto>
    {
    }

    public class ChurchDashboardDto
    {
        public int TotalActiveMembers { get; set; }
        public int TotalEvents { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal MemberGrowthPercent { get; set; }
        public decimal EventGrowthPercent { get; set; }
        public decimal FinancialGrowthPercent { get; set; }
        public decimal AverageDonationTicket { get; set; }
    }

    public class GetChurchDashboardQueryHandler : IRequestHandler<GetChurchDashboardQuery, ChurchDashboardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetChurchDashboardQueryHandler> _logger;

        public GetChurchDashboardQueryHandler(IUnitOfWork unitOfWork, ILogger<GetChurchDashboardQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ChurchDashboardDto> Handle(GetChurchDashboardQuery request, CancellationToken cancellationToken)
        {
            // Busca o membro logado (otimizado - sem Include)
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .Select(m => new { m.Id, m.ChurchId })
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Usuário não encontrado.");
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            }

            var churchId = member.ChurchId;
            var now = DateTime.UtcNow;
            var firstDayThisMonth = new DateTime(now.Year, now.Month, 1);
            var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);

            // ⚡ QUERIES OTIMIZADAS - Sem Include desnecessários

            // Membros ativos
            var totalActiveMembers = await _unitOfWork.Members.Query()
                .CountAsync(m => m.ChurchId == churchId && m.IsActive, cancellationToken);

            // Total de eventos
            var totalEvents = await _unitOfWork.Events.Query()
                .CountAsync(e => e.ChurchId == churchId, cancellationToken);

            // 🔥 SALDO REAL = CashFlow (inclui doações automáticas)
            var cashFlowBalance = await _unitOfWork.CashFlowEntries.Query()
                .Where(e => e.ChurchId == churchId)
                .SumAsync(e => e.Type == CashFlowType.Income ? e.Amount : -e.Amount, cancellationToken);

            var currentBalance = cashFlowBalance;

            // Crescimento de membros
            var membersThisMonth = await _unitOfWork.Members.Query()
                .CountAsync(m => m.ChurchId == churchId && m.Created >= firstDayThisMonth, cancellationToken);

            var membersLastMonth = await _unitOfWork.Members.Query()
                .CountAsync(m => m.ChurchId == churchId &&
                                m.Created >= firstDayLastMonth &&
                                m.Created < firstDayThisMonth, cancellationToken);

            var memberGrowthPercent = CalculateGrowthPercent(membersLastMonth, membersThisMonth);

            // Crescimento de eventos
            var eventsThisMonth = await _unitOfWork.Events.Query()
                .CountAsync(e => e.ChurchId == churchId && e.Date >= firstDayThisMonth, cancellationToken);

            var eventsLastMonth = await _unitOfWork.Events.Query()
                .CountAsync(e => e.ChurchId == churchId &&
                                e.Date >= firstDayLastMonth &&
                                e.Date < firstDayThisMonth, cancellationToken);

            var eventGrowthPercent = CalculateGrowthPercent(eventsLastMonth, eventsThisMonth);

            // Crescimento financeiro (baseado em CashFlow)
            var cashFlowThisMonth = await _unitOfWork.CashFlowEntries.Query()
                .Where(e => e.ChurchId == churchId && e.Date >= firstDayThisMonth)
                .SumAsync(e => e.Type == CashFlowType.Income ? e.Amount : -e.Amount, cancellationToken);

            var cashFlowLastMonth = await _unitOfWork.CashFlowEntries.Query()
                .Where(e => e.ChurchId == churchId &&
                           e.Date >= firstDayLastMonth &&
                           e.Date < firstDayThisMonth)
                .SumAsync(e => e.Type == CashFlowType.Income ? e.Amount : -e.Amount, cancellationToken);

            var financialGrowthPercent = CalculateGrowthPercent(cashFlowLastMonth, cashFlowThisMonth);

            // Ticket médio de doação (apenas doações confirmadas)
            var paidDonations = await _unitOfWork.Donations.Query()
                .Where(d => d.Member.ChurchId == churchId &&
                           d.Payments.Any(p =>
                               p.PaymentStatus == PaymentStatus.Completed.ToString() ||
                               p.PaymentStatus == PaymentStatus.Received.ToString() ||
                               p.PaymentStatus == "RECEIVED" ||
                               p.PaymentStatus == "CONFIRMED"))
                .Select(d => d.Amount)
                .ToListAsync(cancellationToken);

            var averageDonationTicket = paidDonations.Any() ? paidDonations.Average() : 0;

            return new ChurchDashboardDto
            {
                TotalActiveMembers = totalActiveMembers,
                TotalEvents = totalEvents,
                CurrentBalance = currentBalance,
                MemberGrowthPercent = memberGrowthPercent,
                EventGrowthPercent = eventGrowthPercent,
                FinancialGrowthPercent = financialGrowthPercent,
                AverageDonationTicket = averageDonationTicket
            };
        }

        private decimal CalculateGrowthPercent(decimal previous, decimal current)
        {
            if (previous == 0)
                return current > 0 ? 100 : 0;

            return ((current - previous) / previous) * 100;
        }
    }
}