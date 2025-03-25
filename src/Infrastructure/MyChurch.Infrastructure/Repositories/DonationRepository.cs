using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class DonationRepository : GenericRepository<Donation>, IDonationRepository
    {
        public DonationRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
