using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class WorshipPresenceRepository : GenericRepository<WorshipPresence>, IWorshipPresenceRepository
    {
        public WorshipPresenceRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}