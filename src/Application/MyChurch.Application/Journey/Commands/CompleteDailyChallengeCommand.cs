using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Journey.Commands
{
    public class CompleteDailyChallengeCommand : JwtMemberDto, IRequest<Unit>
    {
        [JsonIgnore]
        public int DailyChallengeId { get; set; }
    }

    public class CompleteDailyChallengeCommandHandler : IRequestHandler<CompleteDailyChallengeCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompleteDailyChallengeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CompleteDailyChallengeCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                ValidationException.ThrowException("Member", "Member not found.");
            }

            var dailyChallenge = await _unitOfWork.DailyChallenges.Query()
                .FirstOrDefaultAsync(dc => dc.Id == request.DailyChallengeId, cancellationToken);

            if (dailyChallenge == null)
            {
                ValidationException.ThrowException("DailyChallenge", "Daily Challenge not found.");
            }

            // 2. Update Faith Points
            member.TotalFaithPoints += dailyChallenge.FaithPointsAwarded;

            // 3. Update Devotional Streak
            if (member.LastCompletedActivityDate.HasValue && member.LastCompletedActivityDate.Value.Date == DateTime.UtcNow.AddDays(-1).Date)
            {
                member.DevotionalStreak++;
            }
            else if (!member.LastCompletedActivityDate.HasValue || member.LastCompletedActivityDate.Value.Date != DateTime.UtcNow.Date)
            {
                member.DevotionalStreak = 1;
            }
            member.LastCompletedActivityDate = DateTime.UtcNow;

            // 4. Update Faith Level
            await UpdateFaithLevelAsync(member, cancellationToken);

            _unitOfWork.Members.Update(member);

            await _unitOfWork.CommitAsync();

            return Unit.Value;
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
    }
}
