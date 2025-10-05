using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ChurchScheduleRepository : GenericRepository<ChurchSchedule>, IChurchScheduleRepository
    {
        public ChurchScheduleRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
