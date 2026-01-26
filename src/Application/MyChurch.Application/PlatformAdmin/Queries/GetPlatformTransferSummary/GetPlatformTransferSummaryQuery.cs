using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.PlatformAdmin.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.PlatformAdmin.Queries.GetPlatformTransferSummary
{
    public class GetPlatformTransferSummaryQuery : IRequest<PlatformTransferSummaryDto>
    {
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
    }

    public class GetPlatformTransferSummaryQueryHandler : IRequestHandler<GetPlatformTransferSummaryQuery, PlatformTransferSummaryDto>
    {
        private readonly IUnitOfWork _uow;

        public GetPlatformTransferSummaryQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlatformTransferSummaryDto> Handle(GetPlatformTransferSummaryQuery request, CancellationToken cancellationToken)
        {
            var start = request.StartDateUtc.ToUniversalTime().Date;
            var end = request.EndDateUtc.ToUniversalTime().Date.AddDays(1).AddTicks(-1);

            var q = _uow.TransferHistories.Query().AsNoTracking()
                .Where(t => t.RequestedAt >= start && t.RequestedAt <= end);

            var transfersCount = await q.CountAsync(cancellationToken);
            var totalAmount = await q.SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;

            var pendingCount = await q.CountAsync(t => t.Status == "Pending", cancellationToken);
            var failedCount = await q.CountAsync(t => t.Status == "Failed", cancellationToken);
            var completedCount = await q.CountAsync(t => t.Status == "Completed", cancellationToken);

            var daily = await q
                .GroupBy(t => t.RequestedAt.Date)
                .Select(g => new PlatformTransferSeriesItemDto
                {
                    DateUtc = g.Key,
                    Count = g.Count(),
                    Amount = g.Sum(x => x.Amount),
                    PendingCount = g.Count(x => x.Status == "Pending"),
                    FailedCount = g.Count(x => x.Status == "Failed"),
                    CompletedCount = g.Count(x => x.Status == "Completed")
                })
                .OrderBy(x => x.DateUtc)
                .ToListAsync(cancellationToken);

            return new PlatformTransferSummaryDto
            {
                StartDateUtc = start,
                EndDateUtc = end,
                TransfersCount = transfersCount,
                TotalAmount = totalAmount,
                PendingCount = pendingCount,
                FailedCount = failedCount,
                CompletedCount = completedCount,
                DailySeries = daily
            };
        }
    }
}
