using MediatR;
using MyChurch.Application.Dtos;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace MyChurch.Application.Engagement.Queries
{
    public class GetCommunityEngagementScoreQuery : JwtMemberDto, IRequest<CommunityEngagementScoreDto>
    {
    }

    public class GetCommunityEngagementScoreQueryHandler : IRequestHandler<GetCommunityEngagementScoreQuery, CommunityEngagementScoreDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCommunityEngagementScoreQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CommunityEngagementScoreDto> Handle(GetCommunityEngagementScoreQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null || (member.Role != UserRole.Admin))
                throw new UnauthorizedAccessException("Acesso restrito a Admin.");

            var query = _unitOfWork.Members.Query().Where(m => m.ChurchId == member.ChurchId && m.IsActive);
            var avg = await query.AverageAsync(m => (double?)m.EngagementScore, cancellationToken) ?? 0;
            return new CommunityEngagementScoreDto((int)avg);
        }
    }
}
