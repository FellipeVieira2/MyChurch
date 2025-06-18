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

namespace MyChurch.Application.Family.Commands
{
    public class SendSpouseConnectionRequestCommand : JwtMemberDto, IRequest<int>
    {
        public int InvitedMemberId { get; set; }
    }

    public class SendSpouseConnectionRequestCommandHandler : IRequestHandler<SendSpouseConnectionRequestCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SendSpouseConnectionRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(SendSpouseConnectionRequestCommand request, CancellationToken cancellationToken)
        {
            var inviter = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (inviter == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var invited = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.InvitedMemberId, cancellationToken);
            if (invited == null)
                ValidationException.ThrowException("Member", "Membro convidado não encontrado.");
            if (inviter.ChurchId != invited.ChurchId)
                ValidationException.ThrowException("Member", "Só é possível convidar membros da mesma igreja.");
            var existing = await _unitOfWork.FamilyInvitations.Query()
                .FirstOrDefaultAsync(x => x.InviterMemberId == inviter.Id && x.InvitedMemberId == invited.Id && x.Status == InvitationStatus.Pending, cancellationToken);
            if (existing != null)
                ValidationException.ThrowException("FamilyInvitation", "Já existe um convite pendente para este membro.");
            var invitation = new FamilyInvitation
            {
                ChurchId = inviter.ChurchId,
                InviterMemberId = inviter.Id,
                InvitedMemberId = invited.Id,
                Status = InvitationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.FamilyInvitations.Create(invitation);
            await _unitOfWork.CommitAsync();
            return invitation.Id;
        }
    }
}
