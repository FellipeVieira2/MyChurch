using DotnetGeminiSDK.Client.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Application.Engagement;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Journey.Commands
{
    public class CompleteJourneyStageCommand : JwtMemberDto, IRequest<Unit>
    {
        [JsonIgnore]
        public int JourneyStageId { get; set; }
        public string? MemberResponse { get; set; } // Resposta do membro para tarefas ou reflexões
    }

    public class CompleteJourneyStageCommandHandler : IRequestHandler<CompleteJourneyStageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeminiClient _geminiClient;
        private readonly IEngagementService _engagementService;

        public CompleteJourneyStageCommandHandler(IUnitOfWork unitOfWork, IGeminiClient geminiClient, IEngagementService engagementService)
        {
            _unitOfWork = unitOfWork;
            _geminiClient = geminiClient;
            _engagementService = engagementService;
        }

        public async Task<Unit> Handle(CompleteJourneyStageCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                ValidationException.ThrowException("Member", "Member not found.");
            }

            var journeyStage = await _unitOfWork.JourneyStages.Query()
                .FirstOrDefaultAsync(s => s.Id == request.JourneyStageId, cancellationToken);

            if (journeyStage == null)
            {
                ValidationException.ThrowException("JourneyStage", "Journey Stage not found.");
            }

            var alreadyCompleted = await _unitOfWork.MemberJourneyProgresses.Query()
                .AnyAsync(p => p.MemberId == request.UserId && p.JourneyStageId == request.JourneyStageId, cancellationToken);

            if (alreadyCompleted) return Unit.Value;

            _unitOfWork.MemberJourneyProgresses.Create(new MemberJourneyProgress
            {
                MemberId = request.UserId,
                JourneyStageId = request.JourneyStageId,
                CompletedAt = DateTime.UtcNow
            });

            // Se a etapa não requer verificação, concede os pontos e atualiza o progresso imediatamente.
            if (!journeyStage.RequiresLeaderVerification)
            {
                member.TotalFaithPoints += journeyStage.FaithPointsAwarded;

                if (member.LastCompletedActivityDate.HasValue && member.LastCompletedActivityDate.Value.Date == DateTime.UtcNow.AddDays(-1).Date)
                {
                    member.DevotionalStreak++;
                }
                else if (!member.LastCompletedActivityDate.HasValue || member.LastCompletedActivityDate.Value.Date != DateTime.UtcNow.Date)
                {
                    member.DevotionalStreak = 1;
                }
                member.LastCompletedActivityDate = DateTime.UtcNow;

                await UpdateFaithLevelAsync(member, cancellationToken);
                _unitOfWork.Members.Update(member);
                await CheckForAchievements(member, cancellationToken);

                // Adiciona pontos de engajamento
                await _engagementService.AddPointsForActionAsync(member.Id, member.ChurchId, EngagementEventType.JourneyStageCompleted, journeyStage.Id.ToString());
            }
            else
            {
                // Se requer verificação, cria um alerta para o líder.
                var alert = new PastoralAlert
                {
                    ChurchId = member.ChurchId,
                    MemberId = member.Id,
                    Message = $"O membro {member.Name} completou a tarefa '{journeyStage.Title}' e aguarda sua verificação.",
                    Source = "Jornada da Fé - Verificação Pendente",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.PastoralAlerts.Create(alert);
            }

            if (journeyStage.Type == JourneyStageType.Task && !string.IsNullOrWhiteSpace(request.MemberResponse))
            {
                await AnalyzeAndAlert(request.MemberResponse, member, journeyStage, cancellationToken);
            }

            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }

        private async Task AnalyzeAndAlert(string text, Domain.Entities.Member member, JourneyStage stage, CancellationToken cancellationToken)
        {
            var prompt = $@"...prompt para análise de sentimento..."""; // Prompt omitido para brevidade
            var response = await _geminiClient.TextPrompt(prompt);
            var rawJson = CleanGeminiResponse(response.Candidates.First().Content.Parts.First().Text);
            var sentimentResult = JsonSerializer.Deserialize<SentimentAnalysisResultInternal>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (sentimentResult?.Is_Concerning == true)
            {
                var alert = new PastoralAlert
                {
                    ChurchId = member.ChurchId,
                    MemberId = member.Id,
                    Message = $"Alerta de sentimento negativo na etapa '{stage.Title}': '{text}'",
                    Source = "Jornada da Fé",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.PastoralAlerts.Create(alert);
            }
        }

        private string CleanGeminiResponse(string rawText)
        {
            return Regex.Replace(rawText, @"^```(json)?|```$", string.Empty, RegexOptions.Multiline).Trim();
        }

        private async Task CheckForAchievements(Domain.Entities.Member member, CancellationToken cancellationToken)
        {
            var achievements = await _unitOfWork.Achievements.Query().ToListAsync(cancellationToken);
            var memberAchievements = await _unitOfWork.MemberAchievements.Query()
                .Where(ma => ma.MemberId == member.Id)
                .Select(ma => ma.AchievementId)
                .ToListAsync(cancellationToken);

            var streakAchievement = achievements.FirstOrDefault(a => a.Type == AchievementType.DevotionalStreak && member.DevotionalStreak >= a.Threshold);
            if (streakAchievement != null && !memberAchievements.Contains(streakAchievement.Id))
            {
                var newAchievement = new MemberAchievement
                {
                    MemberId = member.Id,
                    AchievementId = streakAchievement.Id,
                    AwardedAt = DateTime.UtcNow
                };
                _unitOfWork.MemberAchievements.Create(newAchievement);
            }
        }

        private async Task UpdateFaithLevelAsync(Domain.Entities.Member member, CancellationToken cancellationToken)
        {
            var allLevels = await _unitOfWork.FaithLevels.Query().OrderBy(l => l.PointsRequired).ToListAsync(cancellationToken);

            if (!allLevels.Any()) return;

            var newLevel = allLevels.LastOrDefault(l => member.TotalFaithPoints >= l.PointsRequired);

            if (newLevel != null && newLevel.Id != member.FaithLevelId)
            {
                member.FaithLevelId = newLevel.Id;
            }
        }

        private class SentimentAnalysisResultInternal
        {
            public bool Is_Concerning { get; set; }
        }
    }
}
