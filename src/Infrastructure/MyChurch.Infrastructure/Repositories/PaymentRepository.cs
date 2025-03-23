using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Infrastructure.Context;

namespace MyChurch.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
