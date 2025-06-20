using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class JourneyStageRepository : GenericRepository<JourneyStage>, IJourneyStageRepository
    {
        public JourneyStageRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
