using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Journey.Commands
{
    public class VerifyJourneyStageCommand : JwtMemberDto, IRequest<Unit>
    {
        public int MemberJourneyProgressId { get; set; }
    }

    public class VerifyJourneyStageCommandHandler : IRequestHandler<VerifyJourneyStageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerifyJourneyStageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(VerifyJourneyStageCommand request, CancellationToken cancellationToken)
        {
            var progress = await _unitOfWork.MemberJourneyProgresses.Query()
                .Include(p => p.JourneyStage) // Inclui a etapa para obter os pontos
                .FirstOrDefaultAsync(p => p.Id == request.MemberJourneyProgressId, cancellationToken);

            if (progress == null || progress.IsVerified)
            {
                // Ou já foi verificado ou não existe
                return Unit.Value;
            }

            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == progress.MemberId, cancellationToken);

            if (member == null)
            {
                ValidationException.ThrowException("Member", "Member not found.");
            }

            progress.IsVerified = true;

            // Conceder pontos, atualizar streak e nível
            member.TotalFaithPoints += progress.JourneyStage.FaithPointsAwarded;
            // ... (lógica de streak e nível omitida para brevidade, mas seria adicionada aqui)

            _unitOfWork.Members.Update(member);
            _unitOfWork.MemberJourneyProgresses.Update(progress);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
