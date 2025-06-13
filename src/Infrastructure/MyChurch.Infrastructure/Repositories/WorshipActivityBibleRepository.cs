using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class WorshipActivityBibleRepository : GenericRepository<WorshipActivityBible>, IWorshipActivityBibleRepository
    {
        public WorshipActivityBibleRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
