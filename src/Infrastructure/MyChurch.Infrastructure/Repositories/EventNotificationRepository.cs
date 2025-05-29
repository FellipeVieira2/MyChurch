using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class EventNotificationRepository : GenericRepository<EventNotification>, IEventNotificationRepository
    {
        public EventNotificationRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
