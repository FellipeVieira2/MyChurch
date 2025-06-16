using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class FeedPostImageRepository : GenericRepository<FeedPostImage>, IFeedPostImageRepository
    {
        public FeedPostImageRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
