using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Application.Dtos;
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

        public RegisterWorshipPresenceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            }
            return true;
        }
    }
}
