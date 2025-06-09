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
        public decimal? CurrentBalance { get; set; }
        public decimal? MemberGrowthPercent { get; set; }
        public decimal? EventGrowthPercent { get; set; }
        public decimal? FinancialGrowthPercent { get; set; }
        public decimal? AverageDonationTicket { get; set; }
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
            // Busca o membro logado
            var member = await _unitOfWork.Members.Query()
                .Include(m => m.Church)
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null || member.Church == null)
            {
                _logger.LogWarning("Usuário não encontrado ou sem igreja vinculada.");
                throw new UnauthorizedAccessException("Usuário não encontrado ou sem igreja vinculada.");
            }

            var churchId = member.ChurchId;

            // Busca a igreja com relacionamentos necessários
            var church = await _unitOfWork.Churchs.Query()
                .Include(c => c.Members)
                .Include(c => c.Events)
                .Include(c => c.CashFlowEntries)
                .FirstOrDefaultAsync(c => c.Id == churchId, cancellationToken);

            if (church == null)
                throw new Exception("Igreja não encontrada.");

            var now = DateTime.UtcNow;
            var firstDayThisMonth = new DateTime(now.Year, now.Month, 1);
            var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);

            // Membros ativos
            var totalActiveMembers = church.Members?.Count(m => m.IsActive) ?? 0;

            // Eventos
            var totalEvents = church.Events?.Count() ?? 0;

            // Saldo atual
            var currentBalance = (church.CashFlowEntries?.Where(e => e.Type == CashFlowType.Income).Sum(e => e.Amount) ?? 0)
                               - (church.CashFlowEntries?.Where(e => e.Type == CashFlowType.Expense).Sum(e => e.Amount) ?? 0);

            // Crescimento de membros
            var membersLastMonth = church.Members?.Count(m => m.Created >= firstDayLastMonth && m.Created < firstDayThisMonth) ?? 0;
            var membersThisMonth = church.Members?.Count(m => m.Created >= firstDayThisMonth) ?? 0;
            var totalMembersBefore = church.Members?.Count(m => m.Created < firstDayThisMonth) ?? 0;
            var memberGrowthPercent = totalMembersBefore > 0
                ? (decimal)membersThisMonth / totalMembersBefore * 100
                : (membersThisMonth > 0 ? 100 : 0);

            // Crescimento de eventos
            var eventsLastMonth = church.Events?.Count(e => e.Date >= firstDayLastMonth && e.Date < firstDayThisMonth) ?? 0;
            var eventsThisMonth = church.Events?.Count(e => e.Date >= firstDayThisMonth) ?? 0;
            var totalEventsBefore = church.Events?.Count(e => e.Date < firstDayThisMonth) ?? 0;
            var eventGrowthPercent = totalEventsBefore > 0
                ? (decimal)eventsThisMonth / totalEventsBefore * 100
                : (eventsThisMonth > 0 ? 100 : 0);

            // Crescimento/decrescimento financeiro
            var entradasLastMonth = church.CashFlowEntries?.Where(e => e.Type == CashFlowType.Income && e.Date >= firstDayLastMonth && e.Date < firstDayThisMonth).Sum(e => e.Amount) ?? 0;
            var entradasThisMonth = church.CashFlowEntries?.Where(e => e.Type == CashFlowType.Income && e.Date >= firstDayThisMonth).Sum(e => e.Amount) ?? 0;
            var saidasLastMonth = church.CashFlowEntries?.Where(e => e.Type == CashFlowType.Expense && e.Date >= firstDayLastMonth && e.Date < firstDayThisMonth).Sum(e => e.Amount) ?? 0;
            var saidasThisMonth = church.CashFlowEntries?.Where(e => e.Type == CashFlowType.Expense && e.Date >= firstDayThisMonth).Sum(e => e.Amount) ?? 0;
            var saldoLastMonth = entradasLastMonth - saidasLastMonth;
            var saldoThisMonth = entradasThisMonth - saidasThisMonth;
            var financialGrowthPercent = saldoLastMonth != 0
                ? ((saldoThisMonth - saldoLastMonth) / Math.Abs(saldoLastMonth)) * 100
                : (saldoThisMonth > 0 ? 100 : 0);

            // Ticket médio de doação
            var paidDonations = await _unitOfWork.Donations.Query()
                .Include(d => d.Payments)
                .Where(d => d.Member.ChurchId == churchId &&
                            d.Payments.Any(p =>
                                p.PaymentStatus == PaymentStatus.Completed.ToString() ||
                                p.PaymentStatus == PaymentStatus.Received.ToString() ||
                                p.PaymentStatus == "RECEIVED"
                            ))
                .ToListAsync(cancellationToken);

            var averageDonationTicket = paidDonations.Any()
                ? paidDonations.Average(d => d.Amount)
                : 0;

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
    }
}