using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.BibleReadingPlan.Commands
{
    public class AssignBibleReadingPlanCommand : JwtMemberDto, IRequest<Unit>
    {
        public int PlanId { get; set; }
    }

    public class AssignBibleReadingPlanCommandHandler : IRequestHandler<AssignBibleReadingPlanCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AssignBibleReadingPlanCommandHandler> _logger;

        public AssignBibleReadingPlanCommandHandler(IUnitOfWork unitOfWork, ILogger<AssignBibleReadingPlanCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(AssignBibleReadingPlanCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Assigning Bible Reading Plan {PlanId} to Member {MemberId}", request.PlanId, request.UserId);

            // Verificar se o membro existe
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Member not found with ID: {MemberId}", request.UserId);
                ValidationException.ThrowException("UserId", "Member not found.");
            }

            // Verificar se o plano existe
            var plan = await _unitOfWork.BibleReadingPlans.Query()
                .FirstOrDefaultAsync(p => p.Id == request.PlanId, cancellationToken);

            if (plan == null)
            {
                _logger.LogWarning("Bible reading plan not found with ID: {PlanId}", request.PlanId);
                ValidationException.ThrowException("PlanId", "Bible reading plan not found.");
            }

            // Verificar se o plano é público, padrão ou pertence à igreja do membro
            bool canAccess = plan.IsPublic || plan.IsDefault || (plan.ChurchId.HasValue && plan.ChurchId == member.ChurchId);
            
            if (!canAccess)
            {
                _logger.LogWarning("Member {MemberId} does not have access to plan {PlanId}", request.UserId, request.PlanId);
                ValidationException.ThrowException("PlanId", "You do not have access to this reading plan.");
            }

            // Nota: Não há nada específico para "atribuir" o plano ao membro além de verificar o acesso.
            // O progresso será criado naturalmente quando o membro começar a ler e marcar etapas como concluídas.
            // No entanto, poderíamos criar um registro de "MemberBibleReadingAssignment" se quisermos
            // rastrear explicitamente quais planos um membro escolheu seguir.

            _logger.LogInformation("Bible Reading Plan {PlanId} successfully assigned to Member {MemberId}", request.PlanId, request.UserId);
            
            return Unit.Value;
        }
    }
}