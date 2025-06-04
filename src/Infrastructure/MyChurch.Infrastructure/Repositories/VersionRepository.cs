using MyChurch.Domain.Contracts;

namespace MyChurch.Infrastructure.Repositories
{
    public class VersionRepository : GenericRepository<Domain.Entities.Bible.Version>, IVersionRepository
    {
        public VersionRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
