using MyChurch.Domain.Entities.Bible;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyChurch.Domain.Contracts
{
    public interface IMemberBibleReadingProgressRepository : IGenericRepository<MemberBibleReadingProgress>
    {
        Task<MemberBibleReadingProgress> GetProgressForStageAsync(int memberId, int stageId);
        Task<IEnumerable<MemberBibleReadingProgress>> GetMemberProgressForPlanAsync(int memberId, int planId);
        Task<int> GetMemberCompletedStagesCountAsync(int memberId, int planId);
        Task<BibleReadingPlanStage> GetNextStageForMemberAsync(int memberId, int planId);
    }
}