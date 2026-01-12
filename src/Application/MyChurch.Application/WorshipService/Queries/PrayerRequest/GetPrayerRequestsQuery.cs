using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using System.Text.Json.Serialization;

namespace MyChurch.Application.WorshipService.Queries.PrayerRequest
{
    public class GetPrayerRequestsQuery : JwtMemberDto, IRequest<List<PrayerRequestDto>>
    {
        [JsonIgnore]
        public int WorshipServiceId { get; set; }
    }

    public class GetPrayerRequestsQueryHandler : IRequestHandler<GetPrayerRequestsQuery, List<PrayerRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetPrayerRequestsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<PrayerRequestDto>> Handle(GetPrayerRequestsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.PrayerRequests.Query()
                .Include(x => x.Member)
                .Where(x => x.WorshipServiceId == request.WorshipServiceId);

            var isAdmin = Enum.TryParse<UserRole>(request.Role, true, out var role) && role == UserRole.Admin;
            if (!isAdmin)
            {
                query = query.Where(x => x.MemberId == request.UserId);
            }

            var list = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
            return [.. list.Select(PrayerRequestDto.New)];
        }
    }
}
