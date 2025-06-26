using System;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Engagement
{
    public class EngagementService : IEngagementService
    {
        private readonly IEngagementEventRepository _engagementEventRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EngagementService(IEngagementEventRepository engagementEventRepository, IUnitOfWork unitOfWork)
        {
            _engagementEventRepository = engagementEventRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddPointsForActionAsync(int memberId, int churchId, EngagementEventType eventType, string eventReferenceId)
        {
            int points = eventType switch
            {
                EngagementEventType.WorshipPresence => 10,
                EngagementEventType.GroupAttendance => 8,
                EngagementEventType.DonationMade => 12,
                EngagementEventType.BibleReadingStageCompleted => 6,
                EngagementEventType.JourneyStageCompleted => 6,
                EngagementEventType.FeedPostCreated => 2,
                EngagementEventType.FeedPostLiked => 1,
                _ => 0
            };

            var engagementEvent = new EngagementEvent
            {
                Id = Guid.NewGuid(),
                MemberId = memberId,
                ChurchId = churchId,
                Points = points,
                EventType = eventType,
                EventReferenceId = eventReferenceId,
                CreatedAt = DateTime.UtcNow
            };

            _engagementEventRepository.Create(engagementEvent);
            await _unitOfWork.CommitAsync();
            await CalculateAndUpdateMemberScoreAsync(memberId);
        }

        public async Task CalculateAndUpdateMemberScoreAsync(int memberId)
        {
            int score = await _engagementEventRepository.CalculateScoreForMemberAsync(memberId, 90);
            var member = await _unitOfWork.Members.ById(memberId);
            member.SetEngagementScore(score);
            _unitOfWork.Members.Update(member);
            await _unitOfWork.CommitAsync();
        }
    }
}
