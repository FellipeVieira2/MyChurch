using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Enum;
using System.Collections.Generic;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Family.Queries
{
    public class GetPendingFamilyInvitationsQuery : JwtMemberDto, IRequest<List<FamilyInvitationDto>>
    {
    }

    public class GetPendingFamilyInvitationsQueryHandler : IRequestHandler<GetPendingFamilyInvitationsQuery, List<FamilyInvitationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetPendingFamilyInvitationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<FamilyInvitationDto>> Handle(GetPendingFamilyInvitationsQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var invitations = await _unitOfWork.FamilyInvitations.Query()
                .Where(x => x.InvitedMemberId == request.UserId && x.Status == InvitationStatus.Pending)
                .ToListAsync(cancellationToken);
            return invitations.Select(i => new FamilyInvitationDto
            {
                InvitationId = i.Id,
                InviterMemberId = i.InviterMemberId,
                InvitedMemberId = i.InvitedMemberId,
                Status = i.Status.ToString(),
                CreatedAt = i.CreatedAt
            }).ToList();
        }
    }
}
