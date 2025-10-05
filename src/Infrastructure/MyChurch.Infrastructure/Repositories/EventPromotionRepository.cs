using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class EventPromotionRepository : GenericRepository<EventPromotion>, IEventPromotionRepository
    {
        public EventPromotionRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
