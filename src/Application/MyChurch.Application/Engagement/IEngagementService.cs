using System.Threading.Tasks;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Engagement
{
    public interface IEngagementService
    {
        Task AddPointsForActionAsync(int memberId, int churchId, EngagementEventType eventType, string eventReferenceId);
        Task CalculateAndUpdateMemberScoreAsync(int memberId);
    }
}
