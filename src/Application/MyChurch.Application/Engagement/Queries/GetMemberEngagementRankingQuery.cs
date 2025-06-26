using MediatR;
using MyChurch.Application.Dtos;
using Mychurch.Common.Utils.Objects;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Engagement.Queries
{
    public class GetMemberEngagementRankingQuery : JwtMemberDto, IRequest<PagedResultDto<MemberEngagementRankingDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetMemberEngagementRankingQueryHandler : IRequestHandler<GetMemberEngagementRankingQuery, PagedResultDto<MemberEngagementRankingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetMemberEngagementRankingQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<MemberEngagementRankingDto>> Handle(GetMemberEngagementRankingQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null || (member.Role != UserRole.Admin))
                throw new UnauthorizedAccessException("Acesso restrito a Admin.");

            var query = _unitOfWork.Members.Query()
                .Where(m => m.ChurchId == member.ChurchId && m.IsActive)
                .OrderByDescending(m => m.EngagementScore);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new MemberEngagementRankingDto(m.Id, m.Name, m.Photo, m.EngagementScore))
                .ToListAsync(cancellationToken);

            return new PagedResultDto<MemberEngagementRankingDto>(items, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
