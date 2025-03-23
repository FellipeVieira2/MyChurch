using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Infrastructure.Context;

namespace MyChurch.Infrastructure.Repositories
{
    public class PlanRepository : GenericRepository<Plan>, IPlanRepository
    {
        public PlanRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
