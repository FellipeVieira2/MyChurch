using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Repositories
{
    public class BibleReadingPlanStageRepository : GenericRepository<BibleReadingPlanStage>, IBibleReadingPlanStageRepository
    {
        public BibleReadingPlanStageRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}