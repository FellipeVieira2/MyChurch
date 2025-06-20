using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Infrastructure.Repositories;

namespace MyChurch.Infrastructure.Repositories
{
    public class JourneyRepository : GenericRepository<Journey>, IJourneyRepository
    {
        public JourneyRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
