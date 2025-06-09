using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class CreditCardInfoRepository : GenericRepository<CreditCardInfo>, ICreditCardInfoRepository
    {
        public CreditCardInfoRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
