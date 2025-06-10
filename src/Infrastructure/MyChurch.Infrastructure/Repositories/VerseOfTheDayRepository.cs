using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class VerseOfTheDayRepository : GenericRepository<VerseOfTheDay>, IVerseOfTheDayRepository
    {
        public VerseOfTheDayRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
