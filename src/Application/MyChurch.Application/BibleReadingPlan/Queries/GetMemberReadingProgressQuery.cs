using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.BibleReadingPlan.Queries
{
    public class GetMemberReadingProgressQuery : JwtMemberDto, IRequest<BibleReadingPlanProgressDto>
    {
        public int PlanId { get; set; }
    }

    public class GetMemberReadingProgressQueryHandler : IRequestHandler<GetMemberReadingProgressQuery, BibleReadingPlanProgressDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMemberReadingProgressQueryHandler> _logger;

        public GetMemberReadingProgressQueryHandler(IUnitOfWork unitOfWork, ILogger<GetMemberReadingProgressQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BibleReadingPlanProgressDto> Handle(GetMemberReadingProgressQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting reading progress for Member: {MemberId} and Plan: {PlanId}", request.UserId, request.PlanId);

            // Verificar se o membro existe
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Member not found with ID: {MemberId}", request.UserId);
                ValidationException.ThrowException("UserId", "Member not found.");
            }

            // Buscar o plano com todas as etapas
            var plan = await _unitOfWork.BibleReadingPlans.GetPlanWithStagesAsync(request.PlanId);
            if (plan == null)
            {
                _logger.LogWarning("Bible reading plan not found with ID: {PlanId}", request.PlanId);
                ValidationException.ThrowException("PlanId", "Bible reading plan not found.");
            }

            // Buscar o progresso do membro neste plano
            var memberProgress = await _unitOfWork.MemberBibleReadingProgresses.GetMemberProgressForPlanAsync(request.UserId, request.PlanId);
            var completedStageIds = memberProgress.Select(p => p.BibleReadingPlanStageId).ToHashSet();

            // Buscar a próxima etapa não concluída
            var nextStage = await _unitOfWork.MemberBibleReadingProgresses.GetNextStageForMemberAsync(request.UserId, request.PlanId);

            // Criar DTOs para as etapas com informação de conclusão
            var stageDtos = plan.BibleReadingPlanStages
                .OrderBy(s => s.Order)
                .Select(s => 
                {
                    var progress = memberProgress.FirstOrDefault(p => p.BibleReadingPlanStageId == s.Id);
                    
                    return new BibleReadingPlanStageDto
                    {
                        Id = s.Id,
                        BibleReadingPlanId = s.BibleReadingPlanId,
                        Order = s.Order,
                        Description = s.Description,
                        VerseReferences = s.VerseReferences,
                        IsCompleted = completedStageIds.Contains(s.Id),
                        DateCompleted = progress?.DateCompleted
                    };
                })
                .ToList();

            // Criar o DTO do plano
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
                Stages = stageDtos
            };

            // Criar o DTO de progresso
            var progressDto = new BibleReadingPlanProgressDto
            {
                Plan = planDto,
                CompletedStages = completedStageIds.Count,
                TotalStages = plan.BibleReadingPlanStages.Count,
                Stages = stageDtos,
                NextStage = nextStage != null ? stageDtos.FirstOrDefault(s => s.Id == nextStage.Id) : null
            };

            return progressDto;
        }
    }
}