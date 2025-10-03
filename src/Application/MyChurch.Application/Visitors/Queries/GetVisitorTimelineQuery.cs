using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Visitors.Queries
{
    public class GetVisitorTimelineQuery : JwtMemberDto, IRequest<VisitorTimelineDto>
    {
        public int VisitorId { get; set; }
    }
    public class VisitorTimelineDto
    {
        public int VisitorId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public VisitorStatus? Status { get; set; }
        public int Score { get; set; }
        public DateTime? LastVisitAt { get; set; }
        public bool NeedsFollowUp { get; set; }
        public List<StatusHistoryItem> StatusHistory { get; set; } = new();
        public List<VisitItem> Visits { get; set; } = new();
        public List<DonationItem> Donations { get; set; } = new();
    }
    public record StatusHistoryItem(DateTime ChangedAt, VisitorStatus OldStatus, VisitorStatus NewStatus, int? ChangedBy, string? Note);
    public record VisitItem(DateTime Timestamp, int WorshipServiceId, string? WorshipTitle);
    public record DonationItem(DateTime Date, decimal Amount, string? PaymentStatus);

    public class GetVisitorTimelineQueryHandler : IRequestHandler<GetVisitorTimelineQuery, VisitorTimelineDto>
    {
        private readonly IUnitOfWork _uow;
        public GetVisitorTimelineQueryHandler(IUnitOfWork uow){ _uow = uow; }
        public async Task<VisitorTimelineDto> Handle(GetVisitorTimelineQuery request, CancellationToken cancellationToken)
        {
            var visitor = await _uow.Visitors.Query().FirstOrDefaultAsync(v => v.Id == request.VisitorId, cancellationToken);
            if (visitor == null) throw new Exception("Visitor not found");

            var statusHistory = await _uow.VisitorStatusHistories.Query()
                .Where(h => h.VisitorId == request.VisitorId)
                .OrderByDescending(h => h.ChangedAt)
                .Select(h => new StatusHistoryItem(h.ChangedAt, h.OldStatus, h.NewStatus, h.ChangedByMemberId, h.Note))
                .ToListAsync(cancellationToken);

            var visits = await _uow.WorshipPresences.Query()
                .Where(p => p.VisitorId == request.VisitorId)
                .OrderByDescending(p => p.Timestamp)
                .Select(p => new VisitItem(p.Timestamp, p.WorshipServiceId, null))
                .ToListAsync(cancellationToken);

            var donations = await _uow.Donations.Query()
                .Where(d => d.VisitorId == request.VisitorId)
                .OrderByDescending(d => d.Date)
                .Select(d => new DonationItem(d.Date, d.Amount, d.Payments.OrderByDescending(p=>p.Date).Select(p=>p.PaymentStatus).FirstOrDefault()))
                .ToListAsync(cancellationToken);

            return new VisitorTimelineDto
            {
                VisitorId = visitor.Id,
                Name = visitor.Name,
                Email = visitor.Email,
                Phone = visitor.Phone,
                Status = visitor.Status,
                Score = visitor.Score,
                LastVisitAt = visitor.LastVisitAt,
                NeedsFollowUp = visitor.NeedsFollowUp,
                StatusHistory = statusHistory,
                Visits = visits,
                Donations = donations
            };
        }
    }
}
