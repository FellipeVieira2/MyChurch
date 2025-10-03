using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class VisitorStatusHistoryRepository : GenericRepository<VisitorStatusHistory>, IVisitorStatusHistoryRepository
    {
        public VisitorStatusHistoryRepository(MyChurchDbContext context) : base(context) {}
    }
}
