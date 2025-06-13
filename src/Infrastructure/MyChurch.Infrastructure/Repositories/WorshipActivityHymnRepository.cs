using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class WorshipActivityHymnRepository : GenericRepository<WorshipActivityHymn>, IWorshipActivityHymnRepository
    {
        public WorshipActivityHymnRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
