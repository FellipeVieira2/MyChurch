using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.PlatformAdmin.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.PlatformAdmin.Queries.GetPlatformDonationSummary
{
    public class GetPlatformDonationSummaryQuery : IRequest<PlatformDonationSummaryDto>
    {
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
    }

    public class GetPlatformDonationSummaryQueryHandler : IRequestHandler<GetPlatformDonationSummaryQuery, PlatformDonationSummaryDto>
    {
        private readonly IUnitOfWork _uow;

        public GetPlatformDonationSummaryQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlatformDonationSummaryDto> Handle(GetPlatformDonationSummaryQuery request, CancellationToken cancellationToken)
        {
            var start = request.StartDateUtc.ToUniversalTime().Date;
            var end = request.EndDateUtc.ToUniversalTime().Date.AddDays(1).AddTicks(-1);

            var q = _uow.Donations.Query().AsNoTracking()
                .Where(d => d.Date >= start && d.Date <= end);

            var donationsCount = await q.CountAsync(cancellationToken);
            var totalAmount = await q.SumAsync(d => (decimal?)d.Amount, cancellationToken) ?? 0m;
            var totalPlatformFee = await q.SumAsync(d => (decimal?)d.PlatformFee, cancellationToken) ?? 0m;

            var daily = await q
                .GroupBy(d => d.Date.Date)
                .Select(g => new PlatformDonationSeriesItemDto
                {
                    DateUtc = g.Key,
                    Count = g.Count(),
                    Amount = g.Sum(x => x.Amount),
                    PlatformFee = g.Sum(x => x.PlatformFee)
                })
                .OrderBy(x => x.DateUtc)
                .ToListAsync(cancellationToken);

            return new PlatformDonationSummaryDto
            {
                StartDateUtc = start,
                EndDateUtc = end,
                DonationsCount = donationsCount,
                TotalAmount = totalAmount,
                TotalPlatformFee = totalPlatformFee,
                DailySeries = daily
            };
        }
    }
}
