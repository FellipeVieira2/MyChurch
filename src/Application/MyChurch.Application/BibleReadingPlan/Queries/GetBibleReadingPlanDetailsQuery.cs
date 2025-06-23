using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.BibleReadingPlan.Queries
{
    public class GetBibleReadingPlanDetailsQuery : JwtMemberDto, IRequest<BibleReadingPlanDto>
    {
        public int PlanId { get; set; }
    }

    public class GetBibleReadingPlanDetailsQueryHandler : IRequestHandler<GetBibleReadingPlanDetailsQuery, BibleReadingPlanDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetBibleReadingPlanDetailsQueryHandler> _logger;

        public GetBibleReadingPlanDetailsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetBibleReadingPlanDetailsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BibleReadingPlanDto> Handle(GetBibleReadingPlanDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting Bible reading plan details for PlanId: {PlanId}", request.PlanId);

            var plan = await _unitOfWork.BibleReadingPlans.GetPlanWithStagesAsync(request.PlanId);

            if (plan == null)
            {
                _logger.LogWarning("Bible reading plan not found with ID: {PlanId}", request.PlanId);
                ValidationException.ThrowException("PlanId", "Bible reading plan not found.");
                return null; // This won't execute due to the exception, but included for clarity
            }

            // Verificar se o usuário tem acesso a este plano
            if (!plan.IsDefault && !plan.IsPublic && plan.ChurchId.HasValue)
            {
                var member = await _unitOfWork.Members.Query()
                    .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

                if (member == null || member.ChurchId != plan.ChurchId)
                {
                    _logger.LogWarning("User {UserId} does not have access to plan {PlanId}", request.UserId, request.PlanId);
                    ValidationException.ThrowException("PlanId", "You do not have access to this reading plan.");
                }
            }

            var planDto = new BibleReadingPlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                DurationInDays = plan.DurationInDays,
                IsDefault = plan.IsDefault,
                IsPublic = plan.IsPublic,
                ChurchId = plan.ChurchId,
                Created = plan.Created,
                Stages = plan.BibleReadingPlanStages.Select(s => new BibleReadingPlanStageDto
                {
                    Id = s.Id,
                    BibleReadingPlanId = s.BibleReadingPlanId,
                    Order = s.Order,
                    Description = s.Description,
                    VerseReferences = s.VerseReferences
                })
                .OrderBy(s => s.Order)
                .ToList()
            };

            return planDto;
        }
    }
}