using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class UserActionHistoryRepository : GenericRepository<UserActionHistory>, IUserActionHistoryRepository
    {
        public UserActionHistoryRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
