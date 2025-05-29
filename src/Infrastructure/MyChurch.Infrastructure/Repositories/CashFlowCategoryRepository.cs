using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class CashFlowCategoryRepository : GenericRepository<CashFlowCategory>, ICashFlowCategoryRepository
    {
        public CashFlowCategoryRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
