using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class PlatformUserRepository : GenericRepository<PlatformUser>, IPlatformUserRepository
    {
        public PlatformUserRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
