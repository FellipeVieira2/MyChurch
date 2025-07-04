using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class PresentationRepository : GenericRepository<Presentation>, IPresentationRepository
    {
        public PresentationRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}