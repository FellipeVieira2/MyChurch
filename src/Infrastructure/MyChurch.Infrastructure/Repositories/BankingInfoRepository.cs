using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class BankingInfoRepository : GenericRepository<BankingInfo>, IBankingInfoRepository
    {
        public BankingInfoRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
