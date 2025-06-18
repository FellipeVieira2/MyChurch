using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class GroupResourceRepository : GenericRepository<GroupResource>, IGroupResourceRepository
    {
        public GroupResourceRepository(MyChurchDbContext context) : base(context) { }
    }
}
