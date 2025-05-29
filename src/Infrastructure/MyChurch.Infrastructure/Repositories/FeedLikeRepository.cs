using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class FeedLikeRepository : GenericRepository<FeedLike>, IFeedLikeRepository
    {
        public FeedLikeRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
