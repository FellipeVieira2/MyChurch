using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class CashFlowEntryRepository : GenericRepository<CashFlowEntry>, ICashFlowEntryRepository
    {
        public CashFlowEntryRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
