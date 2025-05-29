using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class EventRecurrenceRepository : GenericRepository<EventRecurrence>, IEventRecurrenceRepository
    {
        public EventRecurrenceRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
