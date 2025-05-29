using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class FeedPostRepository : GenericRepository<FeedPost>, IFeedPostRepository
    {
        public FeedPostRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
