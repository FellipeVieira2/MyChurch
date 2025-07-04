using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class SlideRepository : GenericRepository<Slide>, ISlideRepository
    {
        public SlideRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}