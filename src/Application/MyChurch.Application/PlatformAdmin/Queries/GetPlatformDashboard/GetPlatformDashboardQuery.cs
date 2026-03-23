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
            var now = DateTime.UtcNow;
            var processedStatuses = new[] { "Completed", "Received", "Confirmed" };

            var churchesQuery = _uow.Churchs.Query().AsNoTracking();
            var membersQuery = _uow.Members.Query().AsNoTracking();
            var subscriptionsQuery = _uow.Subscriptions.Query().AsNoTracking();

            var donationsQuery = _uow.Donations.Query().AsNoTracking();
            if (start.HasValue) donationsQuery = donationsQuery.Where(d => d.Date >= start.Value);
            if (end.HasValue) donationsQuery = donationsQuery.Where(d => d.Date <= end.Value);

            var processedTransactionsQuery = _uow.Payments.Query()
                .AsNoTracking()
                .Where(p => p.DonationId.HasValue && processedStatuses.Contains(p.PaymentStatus));
            if (start.HasValue) processedTransactionsQuery = processedTransactionsQuery.Where(p => p.Date >= start.Value);
            if (end.HasValue) processedTransactionsQuery = processedTransactionsQuery.Where(p => p.Date <= end.Value);

            var transfersQuery = _uow.TransferHistories.Query().AsNoTracking();
            if (start.HasValue) transfersQuery = transfersQuery.Where(t => t.RequestedAt >= start.Value);
            if (end.HasValue) transfersQuery = transfersQuery.Where(t => t.RequestedAt <= end.Value);

            var activeChurches = await subscriptionsQuery
                .Where(s => s.StartDate <= now && s.EndDate >= now)
                .Select(s => s.ChurchId)
                .Distinct()
                .CountAsync(cancellationToken);

            var subscriberChurches = await subscriptionsQuery
                .Select(s => s.ChurchId)
                .Distinct()
                .CountAsync(cancellationToken);

            var totalChurches = await churchesQuery.CountAsync(cancellationToken);
            var verifiedChurches = await churchesQuery.CountAsync(c => c.IsVerified, cancellationToken);

            var totalMembers = await membersQuery.CountAsync(cancellationToken);
            var activeMembers = await membersQuery.CountAsync(m => m.IsActive, cancellationToken);

            var donationsCount = await donationsQuery.CountAsync(cancellationToken);
            var donationsTotalAmount = await donationsQuery.SumAsync(d => (decimal?)d.Amount, cancellationToken) ?? 0m;
            var donationsPlatformFeeTotal = await donationsQuery.SumAsync(d => (decimal?)d.PlatformFee, cancellationToken) ?? 0m;

            var processedTransactionsCount = await processedTransactionsQuery.CountAsync(cancellationToken);
            var processedAmount = await processedTransactionsQuery.SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;
            var processedCommissionAmount = await processedTransactionsQuery.SumAsync(p => (decimal?)p.Donation!.PlatformFee, cancellationToken) ?? 0m;

            var transfersCount = await transfersQuery.CountAsync(cancellationToken);
            var transfersTotalAmount = await transfersQuery.SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;
            var transfersPendingCount = await transfersQuery.CountAsync(t => t.Status == "Pending", cancellationToken);
            var transfersFailedCount = await transfersQuery.CountAsync(t => t.Status == "Failed", cancellationToken);

            return new PlatformDashboardDto
            {
                ActiveChurches = activeChurches,
                SubscriberChurches = subscriberChurches,
                TotalChurches = totalChurches,
                VerifiedChurches = verifiedChurches,
                UnverifiedChurches = Math.Max(0, totalChurches - verifiedChurches),
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                DonationsCount = donationsCount,
                DonationsTotalAmount = donationsTotalAmount,
                DonationsPlatformFeeTotal = donationsPlatformFeeTotal,
                ProcessedTransactionsCount = processedTransactionsCount,
                ProcessedAmount = processedAmount,
                ProcessedCommissionAmount = processedCommissionAmount,
                TransfersCount = transfersCount,
                TransfersTotalAmount = transfersTotalAmount,
                TransfersPendingCount = transfersPendingCount,
                TransfersFailedCount = transfersFailedCount,
                GeneratedAtUtc = DateTime.UtcNow
            };
        }
    }
}
