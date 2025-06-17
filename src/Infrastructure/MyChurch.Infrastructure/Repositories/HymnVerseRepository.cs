using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Repositories
{
    public class HymnVerseRepository : GenericRepository<HymnVerse>, IHymnVerseRepository
    {
        public HymnVerseRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
