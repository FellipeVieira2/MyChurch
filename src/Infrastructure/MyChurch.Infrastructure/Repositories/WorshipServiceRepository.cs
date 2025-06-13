using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class WorshipServiceRepository : GenericRepository<WorshipService>, IWorshipServiceRepository
    {
        public WorshipServiceRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
