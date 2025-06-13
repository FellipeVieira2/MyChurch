using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class PrayerRequestRepository : GenericRepository<PrayerRequest>, IPrayerRequestRepository
    {
        public PrayerRequestRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
