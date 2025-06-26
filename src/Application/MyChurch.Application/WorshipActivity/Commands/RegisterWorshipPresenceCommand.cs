using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Application.Dtos;
using MyChurch.Application.Engagement;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class RegisterWorshipPresenceCommand : JwtMemberDto, IRequest<bool>
    {
        public int WorshipServiceId { get; set; }
        // UserId já vem do JwtMemberDto
    }

    public class RegisterWorshipPresenceCommandHandler : IRequestHandler<RegisterWorshipPresenceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEngagementService _engagementService;

        public RegisterWorshipPresenceCommandHandler(IUnitOfWork unitOfWork, IEngagementService engagementService)
        {
            _unitOfWork = unitOfWork;
            _engagementService = engagementService;
        }

        public async Task<bool> Handle(RegisterWorshipPresenceCommand request, CancellationToken cancellationToken)
        {
            // Verifica se já existe presença registrada
            var alreadyPresent = await _unitOfWork.WorshipPresences.Query()
                .AnyAsync(p => p.WorshipServiceId == request.WorshipServiceId && p.MemberId == request.UserId, cancellationToken);
            if (!alreadyPresent)
            {
                var presence = new WorshipPresence
                {
                    WorshipServiceId = request.WorshipServiceId,
                    MemberId = request.UserId,
                    Timestamp = DateTime.UtcNow
                };
                _unitOfWork.WorshipPresences.Create(presence);
                await _unitOfWork.CommitAsync();

                // Adiciona pontos de engajamento
                var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
                if (member != null)
                {
                    await _engagementService.AddPointsForActionAsync(member.Id, member.ChurchId, Domain.Enum.EngagementEventType.WorshipPresence, presence.WorshipServiceId.ToString());
                }
            }
            return true;
        }
    }
}
