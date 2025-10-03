using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using Mychurch.Common.Utils.Objects;
using MyChurch.Domain.Enum;
using MyChurch.Application.Dtos; // added

namespace MyChurch.Application.Visitors.Queries
{
    public class GetVisitorsQuery : JwtMemberDto, IRequest<PagedResultDto<VisitorListItemDto>>
    {
        public VisitorStatus? Status { get; set; }
        public int? MinScore { get; set; }
        public int? DaysSinceLastVisitGreaterThan { get; set; }
        public bool? NeedsFollowUp { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class VisitorListItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int Score { get; set; }
        public VisitorStatus? Status { get; set; }
        public DateTime? LastVisitAt { get; set; }
        public bool NeedsFollowUp { get; set; }
    }

    public class GetVisitorsQueryHandler : IRequestHandler<GetVisitorsQuery, PagedResultDto<VisitorListItemDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetVisitorsQueryHandler(IUnitOfWork uow){ _uow = uow; }
        public async Task<PagedResultDto<VisitorListItemDto>> Handle(GetVisitorsQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Visitors.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.Trim().ToLower();
                query = query.Where(v => (v.Name ?? "").ToLower().Contains(s) || (v.Email ?? "").ToLower().Contains(s) || (v.Phone ?? "").Contains(s));
            }
            if (request.Status.HasValue)
                query = query.Where(v => v.Status == request.Status);
            if (request.MinScore.HasValue)
                query = query.Where(v => v.Score >= request.MinScore.Value);
            if (request.DaysSinceLastVisitGreaterThan.HasValue)
            {
                var cutoff = DateTime.UtcNow.AddDays(-request.DaysSinceLastVisitGreaterThan.Value);
                query = query.Where(v => v.LastVisitAt == null || v.LastVisitAt < cutoff);
            }
            if (request.NeedsFollowUp.HasValue)
                query = query.Where(v => v.NeedsFollowUp == request.NeedsFollowUp.Value);

            // auto mark follow up rule (not persisted) : if LastVisitAt >14 days and status != Inactive
            var total = await query.CountAsync(cancellationToken);
            var list = await query.OrderByDescending(v => v.LastVisitAt)
                .Skip((request.Page-1)*request.PageSize)
                .Take(request.PageSize)
                .Select(v => new VisitorListItemDto{
                    Id = v.Id,
                    Name = v.Name,
                    Email = v.Email,
                    Phone = v.Phone,
                    Score = v.Score,
                    Status = v.Status,
                    LastVisitAt = v.LastVisitAt,
                    NeedsFollowUp = v.NeedsFollowUp || (v.LastVisitAt != null && v.LastVisitAt < DateTime.UtcNow.AddDays(-14) && v.Status != VisitorStatus.Inactive)
                }).ToListAsync(cancellationToken);

            return new PagedResultDto<VisitorListItemDto>{ Items = list, PageNumber = request.Page, PageSize = request.PageSize, TotalCount = total };
        }
    }
}
