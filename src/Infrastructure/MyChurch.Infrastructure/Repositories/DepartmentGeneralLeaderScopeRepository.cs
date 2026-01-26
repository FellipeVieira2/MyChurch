using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class DepartmentGeneralLeaderScopeRepository : GenericRepository<DepartmentGeneralLeaderScope>, IDepartmentGeneralLeaderScopeRepository
    {
        public DepartmentGeneralLeaderScopeRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
