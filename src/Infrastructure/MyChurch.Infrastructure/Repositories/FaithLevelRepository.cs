using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class FaithLevelRepository : GenericRepository<FaithLevel>, IFaithLevelRepository
    {
        public FaithLevelRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
