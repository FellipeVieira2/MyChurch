using MyChurch.Domain.Entities.Bible;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyChurch.Domain.Contracts
{
    public interface IBibleReadingPlanRepository : IGenericRepository<BibleReadingPlan>
    {
        Task<IEnumerable<BibleReadingPlan>> GetAllDefaultAndPublicPlansAsync(int? churchId = null);
        Task<BibleReadingPlan> GetPlanWithStagesAsync(int planId);
    }
}