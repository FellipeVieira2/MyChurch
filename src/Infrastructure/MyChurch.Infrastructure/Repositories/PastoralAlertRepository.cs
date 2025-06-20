using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class PastoralAlertRepository : GenericRepository<PastoralAlert>, IPastoralAlertRepository
    {
        public PastoralAlertRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
