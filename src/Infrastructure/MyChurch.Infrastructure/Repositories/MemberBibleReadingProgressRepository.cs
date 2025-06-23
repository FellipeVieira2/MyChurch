using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChurch.Infrastructure.Repositories
{
    public class MemberBibleReadingProgressRepository : GenericRepository<MemberBibleReadingProgress>, IMemberBibleReadingProgressRepository
    {
        public MemberBibleReadingProgressRepository(MyChurchDbContext context) : base(context)
        {
        }

        public async Task<MemberBibleReadingProgress> GetProgressForStageAsync(int memberId, int stageId)
        {
            return await _context.MemberBibleReadingProgresses
                .FirstOrDefaultAsync(p => p.MemberId == memberId && p.BibleReadingPlanStageId == stageId);
        }

        public async Task<IEnumerable<MemberBibleReadingProgress>> GetMemberProgressForPlanAsync(int memberId, int planId)
        {
            return await _context.MemberBibleReadingProgresses
                .Include(p => p.BibleReadingPlanStage)
                .Where(p => p.MemberId == memberId && p.BibleReadingPlanId == planId)
                .OrderBy(p => p.BibleReadingPlanStage.Order)
                .ToListAsync();
        }

        public async Task<int> GetMemberCompletedStagesCountAsync(int memberId, int planId)
        {
            return await _context.MemberBibleReadingProgresses
                .CountAsync(p => p.MemberId == memberId && p.BibleReadingPlanId == planId);
        }

        public async Task<BibleReadingPlanStage> GetNextStageForMemberAsync(int memberId, int planId)
        {
            // Get all stages for the plan
            var allStages = await _context.BibleReadingPlanStages
                .Where(s => s.BibleReadingPlanId == planId)
                .OrderBy(s => s.Order)
                .ToListAsync();

            // Get stages already completed by the member
            var completedStageIds = await _context.MemberBibleReadingProgresses
                .Where(p => p.MemberId == memberId && p.BibleReadingPlanId == planId)
                .Select(p => p.BibleReadingPlanStageId)
                .ToListAsync();

            // Find the first stage that the member hasn't completed yet
            return allStages.FirstOrDefault(s => !completedStageIds.Contains(s.Id));
        }
    }
}