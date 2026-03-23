using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class KidsScaleMemberRepository : GenericRepository<KidsScaleMember>, IKidsScaleMemberRepository
    {
        public KidsScaleMemberRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
