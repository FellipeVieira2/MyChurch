using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Infrastructure.Context;

namespace MyChurch.Infrastructure.Repositories
{
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
