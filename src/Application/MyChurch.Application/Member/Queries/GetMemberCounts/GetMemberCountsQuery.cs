using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Threading.Tasks;

namespace MyChurch.Application.Member.Queries.GetMemberCounts
{
    public class GetMemberCountsQuery : JwtMemberDto, IRequest<MemberCountDto> { }

    public class GetMemberCountsQueryHandler : IRequestHandler<GetMemberCountsQuery, MemberCountDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetMemberCountsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<MemberCountDto> Handle(GetMemberCountsQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            int churchId = member.ChurchId;
            var total = await _unitOfWork.Members.Query().CountAsync(m => m.ChurchId == churchId, cancellationToken);
            var totalActive = await _unitOfWork.Members.Query().CountAsync(m => m.ChurchId == churchId && m.IsActive, cancellationToken);
            var totalInactive = total - totalActive;
            return new MemberCountDto { Total = total, TotalActive = totalActive, TotalInactive = totalInactive };
        }
    }
}