using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Common.Models;
using MyChurch.Application.PlatformAdmin.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.PlatformAdmin.Queries.GetChurchKpis
{
    public class GetChurchKpisQuery : IRequest<PaginatedList<ChurchKpiDto>>
    {
        public string? Search { get; set; }
        public bool? IsVerified { get; set; }

        public DateTime? StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "DonationsTotalAmount";
        public string SortDirection { get; set; } = "desc";
    }

    public class GetChurchKpisQueryHandler : IRequestHandler<GetChurchKpisQuery, PaginatedList<ChurchKpiDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetChurchKpisQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PaginatedList<ChurchKpiDto>> Handle(GetChurchKpisQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber <= 0) request.PageNumber = 1;
            if (request.PageSize <= 0) request.PageSize = 20;

            var start = request.StartDateUtc?.ToUniversalTime();
            var end = request.EndDateUtc?.ToUniversalTime();

            var churches = _uow.Churchs.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.Trim().ToLowerInvariant();
                churches = churches.Where(c => (c.Name ?? string.Empty).ToLower().Contains(s));
            }

            if (request.IsVerified.HasValue)
                churches = churches.Where(c => c.IsVerified == request.IsVerified.Value);

            var members = _uow.Members.Query().AsNoTracking();

            IQueryable<MyChurch.Domain.Entities.Donation> donations = _uow.Donations.Query().AsNoTracking().Include(d => d.Member);
            if (start.HasValue) donations = donations.Where(d => d.Date >= start.Value);
            if (end.HasValue) donations = donations.Where(d => d.Date <= end.Value);

            IQueryable<MyChurch.Domain.Entities.TransferHistory> transfers = _uow.TransferHistories.Query().AsNoTracking();
            if (start.HasValue) transfers = transfers.Where(t => t.RequestedAt >= start.Value);
            if (end.HasValue) transfers = transfers.Where(t => t.RequestedAt <= end.Value);

            var query = churches.Select(c => new ChurchKpiDto
            {
                ChurchId = c.Id,
                ChurchName = c.Name,
                IsVerified = c.IsVerified,
                CreatedAtUtc = c.Created,

                MembersCount = members.Count(m => m.ChurchId == c.Id),
                ActiveMembersCount = members.Count(m => m.ChurchId == c.Id && m.IsActive),

                DonationsCount = donations.Count(d => d.Member != null && d.Member.ChurchId == c.Id),
                DonationsTotalAmount = donations.Where(d => d.Member != null && d.Member.ChurchId == c.Id).Sum(d => (decimal?)d.Amount) ?? 0m,
                DonationsPlatformFeeTotal = donations.Where(d => d.Member != null && d.Member.ChurchId == c.Id).Sum(d => (decimal?)d.PlatformFee) ?? 0m,

                TransfersCount = transfers.Count(t => t.ChurchId == c.Id),
                TransfersTotalAmount = transfers.Where(t => t.ChurchId == c.Id).Sum(t => (decimal?)t.Amount) ?? 0m,
                TransfersPendingCount = transfers.Count(t => t.ChurchId == c.Id && t.Status == "Pending"),
                TransfersFailedCount = transfers.Count(t => t.ChurchId == c.Id && t.Status == "Failed"),

                GeneratedAtUtc = DateTime.UtcNow
            });

            query = ApplySorting(query, request);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<ChurchKpiDto>(items, total, request.PageNumber, request.PageSize);
        }

        private static IQueryable<ChurchKpiDto> ApplySorting(IQueryable<ChurchKpiDto> query, GetChurchKpisQuery request)
        {
            var desc = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return request.SortBy?.ToLowerInvariant() switch
            {
                "memberscount" => desc ? query.OrderByDescending(x => x.MembersCount) : query.OrderBy(x => x.MembersCount),
                "activememberscount" => desc ? query.OrderByDescending(x => x.ActiveMembersCount) : query.OrderBy(x => x.ActiveMembersCount),
                "donationscount" => desc ? query.OrderByDescending(x => x.DonationsCount) : query.OrderBy(x => x.DonationsCount),
                "donationsplatformfeetotal" => desc ? query.OrderByDescending(x => x.DonationsPlatformFeeTotal) : query.OrderBy(x => x.DonationsPlatformFeeTotal),
                "transferscount" => desc ? query.OrderByDescending(x => x.TransfersCount) : query.OrderBy(x => x.TransfersCount),
                "transferstotalamount" => desc ? query.OrderByDescending(x => x.TransfersTotalAmount) : query.OrderBy(x => x.TransfersTotalAmount),
                "createdatutc" => desc ? query.OrderByDescending(x => x.CreatedAtUtc) : query.OrderBy(x => x.CreatedAtUtc),
                _ => desc ? query.OrderByDescending(x => x.DonationsTotalAmount) : query.OrderBy(x => x.DonationsTotalAmount)
            };
        }
    }
}
