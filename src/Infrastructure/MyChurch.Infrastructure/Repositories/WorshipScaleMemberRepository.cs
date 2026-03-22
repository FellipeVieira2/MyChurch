using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class WorshipScaleMemberRepository : GenericRepository<WorshipScaleMember>, IWorshipScaleMemberRepository
    {
        public WorshipScaleMemberRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
