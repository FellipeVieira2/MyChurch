using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Plan.Commands.DeletePlan
{
    public class DeletePlanCommand : JwtMemberDto, IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeletePlanCommandHandler : IRequestHandler<DeletePlanCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeletePlanCommandHandler> _logger;

        public DeletePlanCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeletePlanCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting plan with ID {PlanId}", request.Id);

            // Check if the user has appropriate role (platform admin)
            if (request.Role != "PlatformAdmin")
                ValidationException.ThrowException("Authorization", "Apenas administradores da plataforma podem excluir planos.");

            var plan = await _unitOfWork.Plans.Query()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (plan == null)
                ValidationException.ThrowException("Plan", "Plano não encontrado.");

            // Check if the plan is in use by any subscriptions
            var inUse = await _unitOfWork.Subscriptions.Query()
                .AnyAsync(s => s.PlanId == request.Id && s.EndDate > DateTime.UtcNow, cancellationToken);

            if (inUse)
                ValidationException.ThrowException("Plan", "Este plano está em uso por igrejas ativas e não pode ser excluído.");

            _unitOfWork.Plans.Delete(plan);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}