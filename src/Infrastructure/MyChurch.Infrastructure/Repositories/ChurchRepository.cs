using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ChurchRepository : GenericRepository<Church>, IChurchRepository
    {
        public ChurchRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
