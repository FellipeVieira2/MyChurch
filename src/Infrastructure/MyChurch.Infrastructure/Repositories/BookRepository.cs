
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
