using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ChildPickupAuthorizationRepository : GenericRepository<ChildPickupAuthorization>, IChildPickupAuthorizationRepository
    {
        public ChildPickupAuthorizationRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
