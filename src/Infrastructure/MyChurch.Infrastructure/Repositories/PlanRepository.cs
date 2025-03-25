using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class PlanRepository : GenericRepository<Plan>, IPlanRepository
    {
        public PlanRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
