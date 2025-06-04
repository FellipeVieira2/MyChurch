using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Repositories
{
    public class HymnRepository : GenericRepository<Hymn>, IHymnRepository
    {
        public HymnRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
