using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class WorshipActivityRepository : GenericRepository<WorshipActivity>, IWorshipActivityRepository
    {
        public WorshipActivityRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
