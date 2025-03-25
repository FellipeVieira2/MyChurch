using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {
        public MemberRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
