using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
