using MediatR;
using Microsoft.EntityFrameworkCore;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Donation.Queries.GetAllPaidDonations
{
    public class GetAllPaidDonationsQuery : JwtMemberDto, IRequest<PagedResultDto<PaidDonationDto>>
    {
        public string? Description { get; set; }
        public decimal? Value { get; set; }
        public DateTime? Date { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetAllPaidDonationsQueryHandler : IRequestHandler<GetAllPaidDonationsQuery, PagedResultDto<PaidDonationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllPaidDonationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<PaidDonationDto>> Handle(GetAllPaidDonationsQuery request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o UserId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            var query = _unitOfWork.Donations.Query()
                .Include(d => d.Payments)
                .Include(d => d.Member)
                .Where(d => d.MemberId == request.UserId && d.Member.ChurchId == loggedMember.ChurchId &&
                            d.Payments.Any(p =>
                                p.PaymentStatus == PaymentStatus.Completed.ToString() ||
                                p.PaymentStatus == PaymentStatus.Received.ToString() ||
                                p.PaymentStatus == "RECEIVED" ||
                                p.PaymentStatus == "CONFIRMED"
                            ));

            if (request.Value.HasValue)
                query = query.Where(d => d.Amount == request.Value.Value);

            if (request.Date.HasValue)
                query = query.Where(d => d.Date.Date == request.Date.Value.Date);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(d => d.Date)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = items.Select(d =>
            {
                var payment = d.Payments
                    .Where(p =>
                        p.PaymentStatus == PaymentStatus.Completed.ToString() ||
                        p.PaymentStatus == PaymentStatus.Received.ToString() ||
                        p.PaymentStatus == "RECEIVED" ||
                        p.PaymentStatus == "CONFIRMED"
                    )
                    .OrderByDescending(p => p.Date)
                    .FirstOrDefault();

                return new PaidDonationDto
                {
                    DonationId = d.Id,
                    Amount = d.Amount,
                    Date = d.Date,
                    Status = payment?.PaymentStatus,
                };
            }).ToList();

            return new PagedResultDto<PaidDonationDto>
            {
                TotalCount = total,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                Items = result
            };
        }
    }
}