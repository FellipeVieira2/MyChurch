using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class FamilyRepository : GenericRepository<Family>, IFamilyRepository
    {
        public FamilyRepository(MyChurchDbContext context) : base(context) { }
    }
}
