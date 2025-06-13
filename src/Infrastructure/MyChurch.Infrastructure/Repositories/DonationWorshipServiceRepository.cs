using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class DonationWorshipServiceRepository : GenericRepository<DonationWorshipService>, IDonationWorshipServiceRepository
    {
        public DonationWorshipServiceRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
