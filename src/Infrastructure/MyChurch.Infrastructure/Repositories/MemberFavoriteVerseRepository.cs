using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Repositories
{
    public class MemberFavoriteVerseRepository : GenericRepository<MemberFavoriteVerse>, IMemberFavoriteVerseRepository
    {
        public MemberFavoriteVerseRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
