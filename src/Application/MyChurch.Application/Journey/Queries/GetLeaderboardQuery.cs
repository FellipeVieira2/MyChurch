using MediatR;
using Microsoft.EntityFrameworkCore;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Journey.Queries
{
    public class GetLeaderboardQuery : JwtMemberDto, IRequest<PagedResultDto<LeaderboardDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class GetLeaderboardQueryHandler : IRequestHandler<GetLeaderboardQuery, PagedResultDto<LeaderboardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLeaderboardQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<LeaderboardDto>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (currentUser == null)
            {
                ValidationException.ThrowException("User", "User not found to determine the church.");
            }

            var membersQuery = _unitOfWork.Members.Query()
                .Where(m => m.ChurchId == currentUser.ChurchId);

            var totalCount = await membersQuery.CountAsync(cancellationToken);

            var members = await membersQuery
                .OrderByDescending(m => m.TotalFaithPoints)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new LeaderboardDto
                {
                    MemberId = m.Id,
                    MemberName = m.Name,
                    TotalFaithPoints = m.TotalFaithPoints,
                    DevotionalStreak = m.DevotionalStreak
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<LeaderboardDto>(members, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
