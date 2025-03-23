using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Infrastructure.Context;

namespace MyChurch.Infrastructure.Repositories
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
