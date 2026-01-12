using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class DepartmentMemberRepository : GenericRepository<DepartmentMember>, IDepartmentMemberRepository
    {
        public DepartmentMemberRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
