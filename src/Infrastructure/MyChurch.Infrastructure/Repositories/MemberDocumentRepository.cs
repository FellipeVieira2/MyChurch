using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class MemberDocumentRepository : GenericRepository<MemberDocument>, IMemberDocumetRepository
    {
        public MemberDocumentRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
