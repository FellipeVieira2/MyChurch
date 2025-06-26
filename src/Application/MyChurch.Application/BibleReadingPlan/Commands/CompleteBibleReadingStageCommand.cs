using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;
using MyChurch.Domain.Exceptions;
using MyChurch.Application.Engagement;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.BibleReadingPlan.Commands
{
    public class CompleteBibleReadingStageCommand : JwtMemberDto, IRequest<Unit>
    {
        public int PlanId { get; set; }
        public int StageId { get; set; }
    }

    public class CompleteBibleReadingStageCommandHandler : IRequestHandler<CompleteBibleReadingStageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompleteBibleReadingStageCommandHandler> _logger;
        private readonly IEngagementService _engagementService;

        public CompleteBibleReadingStageCommandHandler(IUnitOfWork unitOfWork, ILogger<CompleteBibleReadingStageCommandHandler> logger, IEngagementService engagementService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _engagementService = engagementService;
        }

        public async Task<Unit> Handle(CompleteBibleReadingStageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Marking stage as completed for Member: {MemberId}, Plan: {PlanId}, Stage: {StageId}", 
                request.UserId, request.PlanId, request.StageId);

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

            // Verificar se a etapa existe e pertence ao plano
            var stage = await _unitOfWork.BibleReadingPlanStages.Query()
                .FirstOrDefaultAsync(s => s.Id == request.StageId && s.BibleReadingPlanId == request.PlanId, cancellationToken);

            if (stage == null)
            {
                _logger.LogWarning("Stage not found with ID: {StageId} for Plan: {PlanId}", request.StageId, request.PlanId);
                ValidationException.ThrowException("StageId", "Stage not found or does not belong to the specified plan.");
            }

            // Verificar se o membro já completou esta etapa
            var existingProgress = await _unitOfWork.MemberBibleReadingProgresses.GetProgressForStageAsync(request.UserId, request.StageId);

            if (existingProgress != null)
            {
                _logger.LogInformation("Member {MemberId} has already completed stage {StageId}", request.UserId, request.StageId);
                return Unit.Value; // Já está concluído, nada a fazer
            }

            // Registrar a conclusão da etapa
            var progress = new MemberBibleReadingProgress
            {
                MemberId = request.UserId,
                BibleReadingPlanId = request.PlanId,
                BibleReadingPlanStageId = request.StageId,
                DateCompleted = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };

            _unitOfWork.MemberBibleReadingProgresses.Create(progress);
            
            // Conceder pontos de fé ao membro (opcional, dependendo dos requisitos)
            member.TotalFaithPoints += 2; // Valor exemplo, pode ser ajustado conforme necessário
            
            // Atualizar último dia de atividade para o streak (se aplicável)
            member.LastCompletedActivityDate = DateTime.UtcNow.Date;
            
            // Incrementar streak se aplicável (completou atividades em dias consecutivos)
            if (member.LastCompletedActivityDate.HasValue && 
                member.LastCompletedActivityDate.Value.Date == DateTime.UtcNow.Date.AddDays(-1))
            {
                member.DevotionalStreak++;
                _logger.LogInformation("Increased devotional streak for member {MemberId} to {Streak}", 
                    member.Id, member.DevotionalStreak);
            }
            
            _unitOfWork.Members.Update(member);
            await _unitOfWork.CommitAsync();

            // Adiciona pontos de engajamento
            await _engagementService.AddPointsForActionAsync(member.Id, member.ChurchId, Domain.Enum.EngagementEventType.BibleReadingStageCompleted, progress.BibleReadingPlanStageId.ToString());

            _logger.LogInformation("Stage {StageId} marked as completed for Member {MemberId}", request.StageId, request.UserId);
            
            return Unit.Value;
        }
    }
}