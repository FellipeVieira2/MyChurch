using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ChildRepository : GenericRepository<Child>, IChildRepository
    {
        public ChildRepository(MyChurchDbContext context) : base(context) { }
    }
}
