using MediatR;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MyChurch.Application.Family.Commands
{
    public class AcceptSpouseConnectionRequestCommand : JwtMemberDto, IRequest<bool>
    {
        public int InvitationId { get; set; }
    }

    public class AcceptSpouseConnectionRequestCommandHandler : IRequestHandler<AcceptSpouseConnectionRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AcceptSpouseConnectionRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(AcceptSpouseConnectionRequestCommand request, CancellationToken cancellationToken)
        {
            var invited = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (invited == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var invitation = await _unitOfWork.FamilyInvitations.Query().FirstOrDefaultAsync(x => x.Id == request.InvitationId, cancellationToken);
            if (invitation == null || invitation.Status != InvitationStatus.Pending)
                ValidationException.ThrowException("FamilyInvitation", "Convite não encontrado ou já respondido.");
            if (invitation.InvitedMemberId != invited.Id)
                ValidationException.ThrowException("FamilyInvitation", "Apenas o membro convidado pode aceitar o convite.");
            var inviter = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == invitation.InviterMemberId, cancellationToken);
            if (inviter == null)
                ValidationException.ThrowException("Member", "Membro que convidou não encontrado.");
            if (inviter.ChurchId != invited.ChurchId)
                ValidationException.ThrowException("Member", "Os membros devem ser da mesma igreja.");
            var family = new Domain.Entities.Family
            {
                ChurchId = invitation.ChurchId,
                FamilyName = $"Família {inviter.Name.Split(' ').Last()} & {invited.Name.Split(' ').Last()}",
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.Families.Create(family);
            await _unitOfWork.CommitAsync();

            inviter.FamilyId = family.Id;
            invited.FamilyId = family.Id;

            _unitOfWork.Members.Update(inviter);
            _unitOfWork.Members.Update(invited);

            invitation.Status = InvitationStatus.Accepted;
            invitation.RespondedAt = DateTime.UtcNow;

            _unitOfWork.FamilyInvitations.Update(invitation);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
