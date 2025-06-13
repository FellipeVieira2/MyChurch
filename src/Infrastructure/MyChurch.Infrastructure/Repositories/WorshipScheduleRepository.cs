using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class WorshipScheduleRepository : GenericRepository<WorshipScheduleItem>, IWorshipScheduleRepository
    {
        public WorshipScheduleRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
