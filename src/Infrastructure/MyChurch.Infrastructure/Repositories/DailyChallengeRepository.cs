using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class DailyChallengeRepository : GenericRepository<DailyChallenge>, IDailyChallengeRepository
    {
        public DailyChallengeRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
