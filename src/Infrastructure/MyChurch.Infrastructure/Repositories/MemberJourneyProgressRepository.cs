using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class MemberJourneyProgressRepository : GenericRepository<MemberJourneyProgress>, IMemberJourneyProgressRepository
    {
        public MemberJourneyProgressRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
