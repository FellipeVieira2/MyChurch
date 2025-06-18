using MediatR;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Family.Commands
{
    public class DeclineSpouseConnectionRequestCommand : JwtMemberDto, IRequest<bool>
    {
        public int InvitationId { get; set; }
    }

    public class DeclineSpouseConnectionRequestCommandHandler : IRequestHandler<DeclineSpouseConnectionRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeclineSpouseConnectionRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeclineSpouseConnectionRequestCommand request, CancellationToken cancellationToken)
        {
            var invited = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (invited == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var invitation = await _unitOfWork.FamilyInvitations.Query().FirstOrDefaultAsync(x => x.Id == request.InvitationId, cancellationToken);
            if (invitation == null || invitation.Status != InvitationStatus.Pending)
                ValidationException.ThrowException("FamilyInvitation", "Convite não encontrado ou já respondido.");
            if (invitation.InvitedMemberId != invited.Id)
                ValidationException.ThrowException("FamilyInvitation", "Apenas o membro convidado pode recusar o convite.");
            invitation.Status = InvitationStatus.Declined;
            invitation.RespondedAt = DateTime.UtcNow;
            _unitOfWork.FamilyInvitations.Update(invitation);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
