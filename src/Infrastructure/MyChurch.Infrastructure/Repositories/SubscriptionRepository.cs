using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
