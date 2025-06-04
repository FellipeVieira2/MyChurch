using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Repositories
{
    public class VerseRepository : GenericRepository<Verse>, IVerseRepository
    {
        public VerseRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
