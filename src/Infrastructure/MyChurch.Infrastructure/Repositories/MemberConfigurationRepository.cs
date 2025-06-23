using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class MemberConfigurationRepository : GenericRepository<MemberConfiguration>, IMemberConfigurationRepository
    {
        public MemberConfigurationRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}