using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.PlatformAdmin.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.PlatformAdmin.Queries.GetPlatformDashboard
{
    public class GetPlatformDashboardQuery : IRequest<PlatformDashboardDto>
    {
        public DateTime? StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
    }

    public class GetPlatformDashboardQueryHandler : IRequestHandler<GetPlatformDashboardQuery, PlatformDashboardDto>
    {
        private readonly IUnitOfWork _uow;

        public GetPlatformDashboardQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlatformDashboardDto> Handle(GetPlatformDashboardQuery request, CancellationToken cancellationToken)
        {
            var start = request.StartDateUtc?.ToUniversalTime();
            var end = request.EndDateUtc?.ToUniversalTime();

            var churchesQuery = _uow.Churchs.Query().AsNoTracking();
            var membersQuery = _uow.Members.Query().AsNoTracking();

            var donationsQuery = _uow.Donations.Query().AsNoTracking();
            if (start.HasValue) donationsQuery = donationsQuery.Where(d => d.Date >= start.Value);
            if (end.HasValue) donationsQuery = donationsQuery.Where(d => d.Date <= end.Value);

            var transfersQuery = _uow.TransferHistories.Query().AsNoTracking();
            if (start.HasValue) transfersQuery = transfersQuery.Where(t => t.RequestedAt >= start.Value);
            if (end.HasValue) transfersQuery = transfersQuery.Where(t => t.RequestedAt <= end.Value);

            var totalChurches = await churchesQuery.CountAsync(cancellationToken);
            var verifiedChurches = await churchesQuery.CountAsync(c => c.IsVerified, cancellationToken);

            var totalMembers = await membersQuery.CountAsync(cancellationToken);
            var activeMembers = await membersQuery.CountAsync(m => m.IsActive, cancellationToken);

            var donationsCount = await donationsQuery.CountAsync(cancellationToken);
            var donationsTotalAmount = await donationsQuery.SumAsync(d => (decimal?)d.Amount, cancellationToken) ?? 0m;
            var donationsPlatformFeeTotal = await donationsQuery.SumAsync(d => (decimal?)d.PlatformFee, cancellationToken) ?? 0m;

            var transfersCount = await transfersQuery.CountAsync(cancellationToken);
            var transfersTotalAmount = await transfersQuery.SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;
            var transfersPendingCount = await transfersQuery.CountAsync(t => t.Status == "Pending", cancellationToken);
            var transfersFailedCount = await transfersQuery.CountAsync(t => t.Status == "Failed", cancellationToken);

            return new PlatformDashboardDto
            {
                TotalChurches = totalChurches,
                VerifiedChurches = verifiedChurches,
                UnverifiedChurches = Math.Max(0, totalChurches - verifiedChurches),
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                DonationsCount = donationsCount,
                DonationsTotalAmount = donationsTotalAmount,
                DonationsPlatformFeeTotal = donationsPlatformFeeTotal,
                TransfersCount = transfersCount,
                TransfersTotalAmount = transfersTotalAmount,
                TransfersPendingCount = transfersPendingCount,
                TransfersFailedCount = transfersFailedCount,
                GeneratedAtUtc = DateTime.UtcNow
            };
        }
    }
}
