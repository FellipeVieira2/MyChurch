using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Repositories
{
    public class ChapterRepository : GenericRepository<Chapter>, IChapterRepository
    {
        public ChapterRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
