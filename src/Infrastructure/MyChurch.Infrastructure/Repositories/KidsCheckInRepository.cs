using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class KidsCheckInRepository : GenericRepository<KidsCheckIn>, IKidsCheckInRepository
    {
        public KidsCheckInRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
