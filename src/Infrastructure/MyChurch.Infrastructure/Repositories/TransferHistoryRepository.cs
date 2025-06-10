using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class TransferHistoryRepository : GenericRepository<TransferHistory>, ITransferHistoryRepository
    {
        public TransferHistoryRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
