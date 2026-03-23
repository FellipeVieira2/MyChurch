using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class DiaconateScaleMemberRepository : GenericRepository<DiaconateScaleMember>, IDiaconateScaleMemberRepository
    {
        public DiaconateScaleMemberRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
