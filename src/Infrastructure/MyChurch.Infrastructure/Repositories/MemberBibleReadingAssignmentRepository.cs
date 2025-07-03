using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Repositories
{
    public class MemberBibleReadingAssignmentRepository : GenericRepository<MemberBibleReadingAssignment>, IMemberBibleReadingAssignmentRepository
    {
        public MemberBibleReadingAssignmentRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
